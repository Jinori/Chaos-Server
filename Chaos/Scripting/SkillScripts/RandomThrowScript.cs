#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
#endregion

namespace Chaos.Scripting.SkillScripts
{
    public class RandomThrowScript : SkillScriptBase
    {
        private readonly Animation ThrowAnimation = new()
        {
            AnimationSpeed = 100,
            TargetAnimation = 123
        };

        public RandomThrowScript(Skill subject)
            : base(subject) { }

        public override void OnUse(ActivationContext context)
        {
            var mapInstance = context.Source.MapInstance;
            var sourceMap = context.SourceMap;

            if (context.Source is Aisling aisling)
                if (!aisling.Equipment.ContainsByTemplateKey("koboldwhacker"))
                    return;

            var throwDirection = context.Source.Direction;
            var thrownPoint = context.Source.DirectionalOffset(throwDirection);
            // You could adjust which Aislings get thrown, e.g. all in range, or a single target, etc.
            // Here, for example, we just retrieve Aislings at the caster's position.
            var thrownCreatures = sourceMap.GetEntitiesAtPoints<Creature>(thrownPoint).Where(x => x.Id != context.Source.Id).ToList();

            foreach (var creature in thrownCreatures)
            {
                // Skip GM-hide or dead Aislings
                if (creature.IsGodModeEnabled() || !creature.IsAlive)
                    continue;

                // Find a truly random walkable point on the map
                Point randomSpot;
                do
                {
                    randomSpot = mapInstance.Template.Bounds.GetRandomPoint();
                }
                while (!mapInstance.IsWalkable(randomSpot, CreatureType.Normal));

                // Show the impact animation at the new spot
                sourceMap.ShowAnimation(ThrowAnimation.GetPointEffectAnimation(randomSpot));

                // Finally, warp the Aisling to the random point
                creature.WarpTo(randomSpot);
            }
        }
    }
}