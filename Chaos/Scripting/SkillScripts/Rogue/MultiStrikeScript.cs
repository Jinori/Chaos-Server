using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class MultiStrikeScript(Skill subject) : ConfigurableSkillScriptBase(subject),
                                                MultistrikeAbilityComponent<Creature>.IAbilityComponentOptions,
                                                DamageAbilityComponent.IDamageComponentOptions,
                                                AssassinStrikeComponent.IDamageComponentOptions
{
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public ushort? AnimationSpeed { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; } = ApplyAttackDamageScript.Create();
    public int? BaseDamage { get; init; }
    public BodyAnimation BodyAnimation { get; init; }

    private ActivationContext? Context { get; set; }
    public decimal? DamageMultiplierPerTarget { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }
    public Element? Element { get; init; }

    public int? ExclusionRange { get; init; }
    public TargetFilter Filter { get; init; }
    public int? ManaCost { get; init; }
    public bool? MoreDmgLowTargetHp { get; init; }
    public bool MustHaveTargets { get; init; }
    public decimal? PctHpDamage { get; init; }
    public decimal PctManaCost { get; init; }
    public decimal? PctOfHealth { get; init; }
    public decimal? PctOfHealthMultiplier { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }
    public int Range { get; init; }
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public byte? Sound { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }
    public bool? SurroundingTargets { get; init; }

    private List<Creature>? Targets { get; set; }
    private IIntervalTimer StrikeTimer { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(310), false);

    private bool IsSameSideOrNoWallBetween(Creature source, Creature target)
    {
        var sourcePoint = new Point(source.X, source.Y);
        var targetPoint = new Point(target.X, target.Y);
        var path = sourcePoint.GetDirectPath(targetPoint);

        // Check if there's a wall between source and target
        var wallPoints = path.Where(point => source.MapInstance.IsWall(point))
                             .ToList();

        if (!wallPoints.Any())

            // No walls between source and target
            return true;

        // Check if source and target are on the same side of the first wall encountered
        var firstWallPoint = wallPoints.First();

        // A line between source and firstWallPoint
        var sourceToWallPath = sourcePoint.GetDirectPath(firstWallPoint);
        var sourceRegion = sourceToWallPath.All(point => !source.MapInstance.IsWall(point));

        // A line between target and firstWallPoint
        var targetToWallPath = targetPoint.GetDirectPath(firstWallPoint);
        var targetRegion = targetToWallPath.All(point => !source.MapInstance.IsWall(point));

        // If both regions are true, source and target are on the same side of the wall
        return sourceRegion && targetRegion;
    }

    public override void OnUse(ActivationContext context)
    {
        Context = context;

        Targets = context.SourceMap
                         .GetEntitiesWithinRange<Creature>(context.Source, 5)
                         .Where(creature => IsSameSideOrNoWallBetween(context.Source, creature) && context.Source.IsHostileTo(creature))
                         .OrderBy(creature => creature.ManhattanDistanceFrom(context.Source))
                         .Take(5)
                         .ToList();
    }

    public override void Update(TimeSpan delta)
    {
        StrikeTimer.Update(delta);

        if (StrikeTimer.IntervalElapsed)
        {
            if ((Targets == null) || (Context?.Source == null))
                return;

            if (Targets.Count <= 0)
                return;

            var target = Targets.FirstOrDefault();

            if (target != null)
            {
                // Define priority order: back, left, right, front
                var directions = new[]
                {
                    target.Direction.Reverse(),
                    target.Direction.RotateLeft(),
                    target.Direction.RotateRight(),
                    target.Direction
                };

                // Find the first accessible direction
                Point? destinationPoint = null;

                foreach (var direction in directions)
                {
                    var potentialPoint = target.DirectionalOffset(direction);

                    if (target.MapInstance.IsWalkable(potentialPoint, CreatureType.Normal)
                        && !target.MapInstance.IsBlockingReactor(potentialPoint))
                    {
                        destinationPoint = potentialPoint;

                        break;
                    }
                }

                // Warp to the selected point and execute the skill
                if (destinationPoint != null)
                {
                    Context.Source.WarpTo(destinationPoint.Value);
                    var newDirection = target.DirectionalRelationTo(Context.Source);
                    Context.Source.Turn(newDirection);

                    new ComponentExecutor(Context).WithOptions(this)
                                                  .ExecuteAndCheck<MultistrikeAbilityComponent<Creature>>()
                                                  ?.Execute<AssassinStrikeComponent>();
                }
            }

            if (target != null)
                Targets.Remove(target);
        }
    }
}