using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class IdentifyItemComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var item = context.SourceAisling?.Inventory.FirstOrDefault();

        if (item is null)
        {
            context.SourceAisling?.SendActiveMessage("You should put the item in-front of your others.");
            return;
        }
        
        context.SourceAisling?.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "Name: "
            + item?.DisplayName
            + "\nWeight: "
            + item?.Template.Weight
            +"\nSkill Damage: "
            +item?.Modifiers?.FlatSkillDamage
            +"\nSpell Damage: "
            +item?.Modifiers?.FlatSpellDamage
            + "\nHealth: "
            + item?.Template.Modifiers?.MaximumHp
            + "\nMana: "
            + item?.Template.Modifiers?.MaximumMp
            + "\nAttack Speed %: "
            + item?.Template.Modifiers?.AtkSpeedPct
            + "\nMagic Resistance: "
            + item?.Template.Modifiers?.MagicResistance
            + "\nStrength: "
            + item?.Template.Modifiers?.Str
            + "\nIntelligence: "
            + item?.Template.Modifiers?.Int
            + "\nWisdom: "
            + item?.Template.Modifiers?.Wis
            + "\nConstitution: "
            + item?.Template.Modifiers?.Con
            + "\nDexterity: "
            + item?.Template.Modifiers?.Dex
            + "\nDMG: "
            + item?.Template.Modifiers?.Dmg
            + "\nHIT: "
            + item?.Template.Modifiers?.Hit);
    }
}