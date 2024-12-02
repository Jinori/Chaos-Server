using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.EquipmentSetBonus
{
    public class SacredSetBonusScript : ConfigurableItemScriptBase
    {
        private const int TWO_SET_BONUS_MANA = 1000;
        private const int THREE_SET_BONUS_SPELL_DAMAGE = 15; // Percentage as integer
        private const int FOUR_SET_BONUS_COOLDOWN_REDUCTION = 10; // Percentage as integer

        private static readonly HashSet<string> SacredSetItems = new(StringComparer.OrdinalIgnoreCase)
        {
            "sacredearrings",
            "sacrednecklace",
            "sacredgreaves",
            "sacredring"
        };

        public SacredSetBonusScript(Item subject) : base(subject) { }

        public override void OnEquipped(Aisling source)
        {
            // Check if the equipped item is a ring before adding the bonus
            if (Subject.Template.TemplateKey == "sacredring")
            {
                // Check if there's already a duplicate ring equipped
                var hasDuplicateItem = source.Equipment
                    .Count(item => item.Template.TemplateKey == "sacredring") > 1;

                // If there is already a duplicate ring, do not add the bonus
                if (hasDuplicateItem)
                    return;
            }

            AddSetBonuses(source); // Add bonuses dynamically based on equipped items
        }

        public override void OnUnEquipped(Aisling source)
        {
            // Check if the unequipping item is a ring before removing the bonus
            if (Subject.Template.TemplateKey == "sacredring")
            {
                // Check how many rings are still equipped
                var equippedItemCount = source.Equipment
                    .Count(item => item.Template.TemplateKey == "sacredring");

                // If there's still at least one ring equipped, don't remove the bonus
                if (equippedItemCount == 1)
                    return;
            }

            RemoveSetBonuses(source); // Remove bonuses dynamically based on equipped items
        }

        private void AddSetBonuses(Aisling source)
        {
            var equippedSetCount = GetEquippedSacredItems(source);

            if (equippedSetCount == 2)
            {
                ApplyBonus(source, new Attributes { MaximumMp = TWO_SET_BONUS_MANA });
            }
            else if (equippedSetCount == 3)
            {
                ApplyBonus(source, new Attributes { SpellDamagePct = THREE_SET_BONUS_SPELL_DAMAGE });
            }
            else if (equippedSetCount == 4)
            {
                ApplyBonus(source, new Attributes { CooldownReductionPct = FOUR_SET_BONUS_COOLDOWN_REDUCTION });
            }
        }

        private void RemoveSetBonuses(Aisling source)
        {
            var equippedSetCount = GetEquippedSacredItems(source);

            if (equippedSetCount == 1)
            {
                RemoveBonus(source, new Attributes { MaximumMp = TWO_SET_BONUS_MANA });
            }
            else if (equippedSetCount == 2)
            {
                RemoveBonus(source, new Attributes { SpellDamagePct = THREE_SET_BONUS_SPELL_DAMAGE });
            }
            else if (equippedSetCount == 3)
            {
                RemoveBonus(source, new Attributes { CooldownReductionPct = FOUR_SET_BONUS_COOLDOWN_REDUCTION });
            }
        }

        private void ApplyBonus(Aisling source, Attributes bonus)
        {
            source.StatSheet.AddBonus(bonus);
            source.Client.SendAttributes(StatUpdateType.Full);
        }

        private void RemoveBonus(Aisling source, Attributes bonus)
        {
            source.StatSheet.SubtractBonus(bonus);
            source.Client.SendAttributes(StatUpdateType.Full);
        }

        private static int GetEquippedSacredItems(Aisling source)
        {
            var uniqueItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in source.Equipment)
            {
                if (SacredSetItems.Contains(item.Template.TemplateKey))
                {
                    uniqueItems.Add(item.Template.TemplateKey);
                }
            }

            return uniqueItems.Count;
        }
    }
}
