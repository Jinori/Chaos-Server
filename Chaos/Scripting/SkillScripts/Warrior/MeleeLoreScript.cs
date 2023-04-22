using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class MeleeLoreScript : BasicSkillScriptBase
{
    /// <inheritdoc />
    public MeleeLoreScript(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);

        var item = context.SourceAisling?.Inventory.FirstOrDefault();

        if (item is null)
        {
            context.SourceAisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "You should seek to put the item first in your hands.");

            return;
        }

        if (item.Template.ScriptVars.Count <= 0)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Using this as a weapon would be nice..");

            return;
        }

        if (item.Template.ScriptVars.Count > 0)
        {
            var weapon = item?.Template!.ScriptVars["equipment"]!.Get<EquipmentType>("Weapon");

            if (weapon is null)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Using this as a weapon would be nice..");

                return;
            }

            context.SourceAisling?.Client.SendServerMessage(
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

    #region ScriptVars
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    #endregion
}