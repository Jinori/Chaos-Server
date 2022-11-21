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
    public class AoPoisonScript : BasicSpellScriptBase
    {
        protected int? manaSpent { get; init; }


        public AoPoisonScript(Spell subject) : base(subject)
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

            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            if (context.Target.Effects.Contains("poison"))
            {
                context.Target.Effects.Dispel("poison");
                context.TargetAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{context.Source.Name} has healed your poison. You feel fine now.");
                context.TargetAisling?.Client.SendAttributes(StatUpdateType.Full);
                ShowAnimation(context, affectedPoints);
                PlaySound(context, affectedPoints);
            }
            else
            {
                context.TargetAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This target isn't affected by poison.");
                return;
            }
        }
    }
}
