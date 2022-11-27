using Chaos.Common.Definitions;
using Chaos.Factories;
using Chaos.Factories.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts
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
            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);

            var effect = EffectFactory.Create("stench");
            context.Source.Effects.Apply(context.Source, effect);
        }
    }
}
