using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
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

    private readonly IEffectFactory EffectFactory;
    private HashSet<uint> HitTargetIds = new(); // Using HashSet<uint> to track target IDs

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);
    
    private Creature Target { get; set; } = null!;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 242
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromHours(1000), false);

    private IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);

    /// <inheritdoc />
    public override byte Icon => 112;

    /// <inheritdoc />
    public override string Name => "Chain Lightning";

    public ChainLightningEffect(IEffectFactory effectFactory)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    private void BounceToNextTarget(Creature source, Creature target)
    {
        // Add the target's ID to the HitTargets set
        HitTargetIds.Add(target.Id);

        var manaBonus = Convert.ToInt32(Source.StatSheet.EffectiveMaximumMp * 0.04m);

        // Base damage calculation
        var baseDamage = Source.StatSheet.EffectiveInt * 32 + 1000 + manaBonus;

        var addedFromPct = baseDamage * (source.StatSheet.EffectiveSpellDamagePct / 100m);
        baseDamage += Convert.ToInt32(source.StatSheet.EffectiveFlatSpellDamage + addedFromPct);

        // Calculate reduction based on the number of hits
        var numberOfHits = HitTargetIds.Count;
        var reductionPercentage = Math.Min(0.90, 0.10 * (numberOfHits - 1)); // 10% reduction per hit, max 90%
        var finalDamage = baseDamage * (1 - reductionPercentage); // Apply the reduction

        // Apply the reduced damage
        ApplyDamageScript.ApplyDamage(
            Source,
            target,
            this,
            (int)finalDamage,
            Element.Wind);

        target.ShowHealth();

        // Find nearby targets for the next bounce
        var nearbyTargets = target.MapInstance
                                  .GetEntitiesWithinRange<Creature>(target, 2)
                                  .OrderBy(t => t.ManhattanDistanceFrom(target))
                                  .Where(t => !HitTargetIds.Contains(t.Id) && ShouldApply(Source, t) && !t.MapInstance.IsWall(t))
                                  .ToList();

        if (nearbyTargets.Count != 0)
        {
            var nextTarget = nearbyTargets.First();
            var effect = EffectFactory.Create("chainlightning");

            // Assuming that your effect has a property or method to set the number of bounces or pass additional data
            if (effect is ChainLightningEffect chainLightningEffect)
                chainLightningEffect.HitTargetIds = new HashSet<uint>(HitTargetIds);

            nextTarget.Effects.Apply(Source, effect);
            nextTarget.Animate(Animation);
        }

        // Terminate the effect after bouncing
        target.Effects.Terminate("Chain Lightning");
    }

    public override void OnApplied() { }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => BounceToNextTarget(Source, Target);

    public override bool ShouldApply(Creature source, Creature target)
    {
        Source = source;
        Target = target;

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