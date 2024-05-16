using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class InspectItemComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var item = context.SourceAisling?.Inventory.FirstOrDefault();
        var options = vars.GetOptions<IInspectItemComponentOptions>();

        if (item is null)
        {
            context.SourceAisling?.SendActiveMessage("Put an item in the first slot of your inventory.");

            return;
        }

        if (options.OutputType != null)
        {
            var message = $"Name: {item.DisplayName}"
                          + $"\nLevel: {item.Level}"
                          + $"\nWeight: {item.Template.Weight}"
                          + $"\nAC: {item.Template.Modifiers?.Ac}"
                          + $"\nMax Stack: {item.Template.MaxStacks}"
                          + $"\nSkill Damage: {item.Modifiers.FlatSkillDamage}"
                          + $"\nSpell Damage: {item.Modifiers.FlatSpellDamage}"
                          + $"\nHealth: {item.Modifiers?.MaximumHp}"
                          + $"\nMana: {item.Modifiers?.MaximumMp}"
                          + $"\nAttack Speed %: {item.Modifiers?.AtkSpeedPct}"
                          + $"\nMagic Resistance: {item.Modifiers?.MagicResistance}"
                          + $"\nStrength: {item.Modifiers?.Str}"
                          + $"\nIntelligence: {item.Modifiers?.Int}"
                          + $"\nWisdom: {item.Modifiers?.Wis}"
                          + $"\nConstitution: {item.Modifiers?.Con}"
                          + $"\nDexterity: {item.Modifiers?.Dex}"
                          + $"\nDMG: {item.Modifiers?.Dmg}"
                          + $"\nHIT: {item.Modifiers?.Hit}"
                          + $"\nSkill Pct Damage: {item.Modifiers?.SkillDamagePct}"
                          + $"\nSpell Pct Damage: {item.Modifiers?.SpellDamagePct}";

            context.SourceAisling?.SendServerMessage(options.OutputType.Value, message);
        }
    }

    public interface IInspectItemComponentOptions
    {
        ServerMessageType? OutputType { get; init; }
    }
}

internal class InspectItemComponentImpl : InspectItemComponent { }