using Chaos.Common.Definitions;
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
    public class AoCradhScript : BasicSpellScriptBase
    {
        protected int? manaSpent { get; init; }
        protected string? cradhNameToRemove { get; init; }


        public AoCradhScript(Spell subject) : base(subject)
        {
        }

        public override void OnUse(SpellContext context)
        {
            //Status Related
            if (context.Source.Status.HasFlag(Status.Suain))
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your hands are frozen.");
                return;
            }

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

            if (cradhNameToRemove is not null)
            {
                ShowBodyAnimation(context);

                var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
                var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

                ShowAnimation(context, affectedPoints);
                PlaySound(context, affectedPoints);
                context.Target.Effects.Dispel(cradhNameToRemove);
                context.TargetAisling?.Client.SendAttributes(StatUpdateType.Full);
            }
        }
    }
}
