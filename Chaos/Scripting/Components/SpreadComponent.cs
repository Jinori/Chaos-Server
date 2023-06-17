using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components
{
    public class SpreadComponent : IConditionalComponent
    {
        /// <inheritdoc />
        public virtual bool Execute(ActivationContext context, ComponentVars vars)
        {
            // Getting necessary options
            var animationOptions = vars.GetOptions<AnimationComponent.IAnimationComponentOptions>();
            var damageOptions = vars.GetOptions<DamageComponent.IDamageComponentOptions>();
            var baseDmg = damageOptions.BaseDamage ?? 0; // safely handle nullable int
            var options = vars.GetOptions<ISpreadComponentOptions>();

            // Getting targets
            var targets = vars.GetTargets<MapEntity>();
            var splashTargets = new List<Creature>();

            // Check each target for nearby creatures
            foreach (var splashCheck in targets)
            {
                var creaturesWithinRange = context.TargetMap
                                                  .GetEntitiesWithinRange<Creature>(splashCheck, options.SpreadDistance);

                // Apply chance to add creature to splashTargets
                foreach (var creatureToSplash in creaturesWithinRange)
                {
                    if (IntegerRandomizer.RollChance(options.SpreadChance))
                    {
                        splashTargets.Add(creatureToSplash);
                    }
                }
            }

            // If no targets were found, return false
            if (splashTargets.Count == 0)
                return false;
            
            // Apply damage to all splashTargets
            foreach (var target in splashTargets)
            {
                if (animationOptions.Animation != null)
                    target.Animate(animationOptions.Animation);
                
                damageOptions.ApplyDamageScript.ApplyDamage(
                    context.Source,
                    target,
                    options.SourceScript,
                    baseDmg,
                    damageOptions.Element);
            }

            return true;
        }

        public interface ISpreadComponentOptions
        {
            int SpreadDistance { get; init; }
            int SpreadChance { get; init; }
            IScript SourceScript { get; init; }
        }
    }
}