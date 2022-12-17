using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.Warrior
{
    public class MeleeLoreScript : BasicSkillScriptBase
    {
        public MeleeLoreScript(Skill subject) : base(subject)
        {
        }

        public override void OnUse(SkillContext context)
        {
            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);

            var item = context.SourceAisling?.Inventory.FirstOrDefault();


            if (item is null)
            {
                context.SourceAisling?.Client.SendServerMessage(Common.Definitions.ServerMessageType.OrangeBar1, "You should seek to put the item first in your hands.");
                return;
            }

            if (item?.Template.ScriptVars.Count <= 0)
            {
                context.SourceAisling?.Client.SendServerMessage(Common.Definitions.ServerMessageType.OrangeBar1, "Using this as a weapon would be nice..");
                return;
            }

            if (item?.Template.ScriptVars.Count > 0)
            {
                var weapon = item?.Template!.ScriptVars["equipment"]!.Select(x => x.Key.EqualsI("equipmentType") && x.Value.Equals("weapon"));
                if (weapon is null)
                {
                    context.SourceAisling?.Client.SendServerMessage(Common.Definitions.ServerMessageType.OrangeBar1, "Using this as a weapon would be nice..");
                    return;
                }
                if (weapon is not null)
                {
                    context.SourceAisling?.Client.SendServerMessage(Common.Definitions.ServerMessageType.ScrollWindow,
                        "Name: " + item?.DisplayName
                        + "\nWeight: " + item?.Template.Weight
                        + "\nHealth: " + item?.Template.Modifiers?.MaximumHp
                        + "\nMana: " + item?.Template.Modifiers?.MaximumMp
                        + "\nAttack Speed %: " + item?.Template.Modifiers?.AtkSpeedPct
                        + "\nMagic Resistance: " + item?.Template.Modifiers?.MagicResistance
                        + "\nStrength: " + item?.Template.Modifiers?.Str
                        + "\nIntelligence: " + item?.Template.Modifiers?.Int
                        + "\nWisdom: " + item?.Template.Modifiers?.Wis
                        + "\nConstitution: " + item?.Template.Modifiers?.Con
                        + "\nDexterity: " + item?.Template.Modifiers?.Dex
                        + "\nDMG: " + item?.Template.Modifiers?.Dmg
                        + "\nHIT: " + item?.Template.Modifiers?.Hit);
                }
            }
        }
    }
}
