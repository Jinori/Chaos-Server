using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class InspectItemComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var item = context.SourceAisling?.Inventory.FirstOrDefault();
        var options = vars.GetOptions<IInspectItemComponentOptions>();

        if (item is null)
        {
            context.SourceAisling?.SendActiveMessage("You should put the item in front of your others.");
            return;
        }

        if (options.OutputType != null)
        {
            var message = $"Name: {item?.DisplayName}" +
                          $"\nWeight: {item?.Template.Weight}" +
                          $"\nMax Stack: {item?.Template.MaxStacks}" +
                          $"\nSkill Damage: {item?.Modifiers?.FlatSkillDamage}" +
                          $"\nSpell Damage: {item?.Modifiers?.FlatSpellDamage}" +
                          $"\nHealth: {item?.Template.Modifiers?.MaximumHp}" +
                          $"\nMana: {item?.Template.Modifiers?.MaximumMp}" +
                          $"\nAttack Speed %: {item?.Template.Modifiers?.AtkSpeedPct}" +
                          $"\nMagic Resistance: {item?.Template.Modifiers?.MagicResistance}" +
                          $"\nStrength: {item?.Template.Modifiers?.Str}" +
                          $"\nIntelligence: {item?.Template.Modifiers?.Int}" +
                          $"\nWisdom: {item?.Template.Modifiers?.Wis}" +
                          $"\nConstitution: {item?.Template.Modifiers?.Con}" +
                          $"\nDexterity: {item?.Template.Modifiers?.Dex}" +
                          $"\nDMG: {item?.Template.Modifiers?.Dmg}" +
                          $"\nHIT: {item?.Template.Modifiers?.Hit}" +
                          $"\nSkill Pct Damage: {item?.Template.Modifiers?.SkillDamagePct}" +
                          $"\nSpell Pct Damage: {item?.Template.Modifiers?.SpellDamagePct}";

            context.SourceAisling?.SendServerMessage(options.OutputType.Value, message);
        }
    }
    
    public interface IInspectItemComponentOptions
    {
        ServerMessageType? OutputType { get; init; }
    }
    
}