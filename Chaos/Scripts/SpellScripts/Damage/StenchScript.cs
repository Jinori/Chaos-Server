using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts.Damage
{
    public class StenchScript : BasicSpellScriptBase
    {
        private readonly IEffectFactory EffectFactory;

        public StenchScript(Spell subject, IEffectFactory effectFactory) : base(subject)
        {
            EffectFactory = effectFactory;
        }

        public override void OnUse(SpellContext context)
        {
            if (context.SourceAisling!.Effects.Contains("stench"))
            {
                context.SourceAisling?.Effects.Terminate("stench");
                return;
            }
            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);

            var effect = EffectFactory.Create("stench");
            context.Source.Effects.Apply(context.Source, effect);
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A foul stench rolls off your body...");
        }
    }
}
