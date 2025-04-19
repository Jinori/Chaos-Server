using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public sealed class ChainLightningEffect : ContinuousAnimationEffectBase
{
    private const int MAX_BOUNCES = 5;
    
    private readonly HashSet<uint> HitTargetIds = [];

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 242
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromHours(1000), false);

    private IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);

    /// <inheritdoc />
    public override byte Icon => 112;

    /// <inheritdoc />
    public override string Name => "Chain Lightning";

    private void BounceToNextTarget()
    {
        HitTargetIds.Add(Subject.Id);
        
        var nearbyTargets = Subject.MapInstance
                                   .GetEntitiesWithinRange<Creature>(Subject, 4)
                                   .OrderBy(t => t.ManhattanDistanceFrom(Subject))
                                   .Where(t => !HitTargetIds.Contains(t.Id) && ShouldApply(Source, t))
                                   .ToList();

        if (nearbyTargets.Count != 0)
        {
            var nextTarget = nearbyTargets.First();
            
            nextTarget.Effects.Apply(Source, this, SourceScript);
        }
        
        Subject.Effects.Terminate("Chain Lightning");
    }

    public override void OnApplied()
    {
        Subject.Animate(Animation);
        var manaBonus = Convert.ToInt32(Source.StatSheet.EffectiveMaximumMp * 0.04m);
        
        var baseDamage = Source.StatSheet.EffectiveInt * 24 + 100 + manaBonus;
        
        var numberOfHits = HitTargetIds.Count;
        var reductionPercentage = 0.10 * numberOfHits;
        var finalDamage = baseDamage * (1 - reductionPercentage);

        ApplyDamageScript.ApplyDamage(
            Source,
            Subject,
            SourceScript ?? this,
            (int)finalDamage,
            Element.Wind);
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => BounceToNextTarget();

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (HitTargetIds.Count >= MAX_BOUNCES)
            return false;

        if (!target.IsHostileTo(source))
            return false;

        if (target.Effects.Contains("Chain Lightning"))
            return false;

        if (HitTargetIds.Contains(target.Id))
            return false;

        return true;
    }
}