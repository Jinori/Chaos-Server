using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Geometry.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts
{
    public class SelfReviveScript : ReviveScript
    {
        public SelfReviveScript(Spell subject) : base(subject)
        {
        }
        public override bool CanUse(SpellContext context)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can only cast this spell when you are dead.");
            return context.Source.Equals(context.Target) && !context.Source.IsAlive;
        }
    }

    public class ReviveScript : BasicSpellScriptBase
    {
        protected int? manaSpent { get; init; }

        public override bool CanUse(SpellContext context)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You cannot use this spell while dead.");
            return context.Source.IsAlive;
        }

        public ReviveScript(Spell subject) : base(subject)
        {
        }
        public override void OnUse(SpellContext context)
        {
            if (manaSpent.HasValue)
            {
                //Require mana
                if (context.Source.StatSheet.CurrentMp < manaSpent.Value)
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                    return;
                }
                //Subtract mana and update user
                context.Source.StatSheet.SubtractMp(manaSpent.Value);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);
            if (!context.Target.IsAlive)
            {

                //Let's restore their hp/mp to %20
                context.Target.StatSheet.AddHealthPct(20);
                context.Target.StatSheet.AddManaPct(20);

                //Refresh the users health bar
                context.TargetAisling?.Client.SendAttributes(StatUpdateType.Vitality);

                //Let's tell the player they have been revived
                context.TargetAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived.");
                ShowAnimation(context, affectedPoints);
                ShowBodyAnimation(context);
            }
            else
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This target isn't dead.");
                return;
            }

        }
    }
}
