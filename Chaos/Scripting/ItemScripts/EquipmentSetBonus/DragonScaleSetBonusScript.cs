using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus;

public class DragonScaleSetBonusScript : ConfigurableItemScriptBase
{
    private const int TWO_SET_BONUS_HEALTH = 500;
    private const int THREE_SET_BONUS_ATTACK_SPEED = 12; // Percentage as integer

    private static readonly HashSet<string> DragonScaleSetItems = new(StringComparer.OrdinalIgnoreCase)
    {
        "dragonscalesword",
        "dragonscaledagger",
        "dragonscaleclaws",
        "dragonscalegauntlet",
        "dragonscalering"
    };

    public DragonScaleSetBonusScript(Item subject)
        : base(subject) { }

    public override void OnEquipped(Aisling source)
    {
        // Check if the currently equipped item is a gauntlet or a ring
        if (Subject.Template.TemplateKey == "dragonscalegauntlet" || Subject.Template.TemplateKey == "dragonscalering")
        {
            // Check if the item is a duplicate (i.e., if another gauntlet or ring is already equipped)
            var hasDuplicateItem = source.Equipment
                .Count(item => item.Template.TemplateKey == Subject.Template.TemplateKey) > 1;

            // If there's already a duplicate item (like two gauntlets or two rings), do not add the bonus for this item
            if (hasDuplicateItem)
                return;
        }

        AddSetBonuses(source); // Add bonuses dynamically
    }

    public override void OnUnEquipped(Aisling source)
    {
        // Check if the unequipping item is a gauntlet or a ring
        if (Subject.Template.TemplateKey == "dragonscalegauntlet" || Subject.Template.TemplateKey == "dragonscalering")
        {
            // Check how many of the same item are still equipped
            var equippedItemCount = source.Equipment
                .Count(item => item.Template.TemplateKey == Subject.Template.TemplateKey);

            // If there's still at least one of the same item (gauntlet or ring) equipped, don't remove the bonus
            if (equippedItemCount == 1)
                return;
        }

        RemoveSetBonuses(source); // Remove bonuses dynamically
    }
    private void AddSetBonuses(Aisling source)
    {
        var equippedSetCount = GetEquippedDragonScaleItems(source);

        if (equippedSetCount == 2)
        {
            var healthBonus = new Attributes { MaximumHp = TWO_SET_BONUS_HEALTH };
            source.StatSheet.AddBonus(healthBonus);
        }

        if (equippedSetCount == 3)
        {
            var attackSpeedBonus = new Attributes { AtkSpeedPct = THREE_SET_BONUS_ATTACK_SPEED };
            source.StatSheet.AddBonus(attackSpeedBonus);
        }
        source.Client.SendAttributes(StatUpdateType.Full);
    }

    private void RemoveSetBonuses(Aisling source)
    {
        var equippedSetCount = GetEquippedDragonScaleItems(source);

        if (equippedSetCount == 1)
        {
            var healthBonus = new Attributes { MaximumHp = TWO_SET_BONUS_HEALTH };
            source.StatSheet.SubtractBonus(healthBonus);
        }

        if (equippedSetCount == 2)
        {
            var attackSpeedBonus = new Attributes { AtkSpeedPct = THREE_SET_BONUS_ATTACK_SPEED };
            source.StatSheet.SubtractBonus(attackSpeedBonus);
        }

        source.Client.SendAttributes(StatUpdateType.Full);
    }

    private static int GetEquippedDragonScaleItems(Aisling source)
    {
        var uniqueItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in source.Equipment)
        {
            if (DragonScaleSetItems.Contains(item.Template.TemplateKey))
            {
                uniqueItems.Add(item.Template.TemplateKey);
            }
        }

        return uniqueItems.Count;
    }

}
