using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class HealingAuraEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    private Creature SourceOfEffect { get; set; } = null!;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 969,
        Priority = 70
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    protected IApplyHealScript ApplyHealScript { get; } = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 57;

    /// <inheritdoc />
    public override string Name => "Healing Aura";

    /// <inheritdoc />
  protected override void OnIntervalElapsed()
{
    // Define a 4x4 area centered around the subject
    var rectangle = new Rectangle(
        Subject.X - 2, // Offset to center the 4x4 area
        Subject.Y - 2,
        5,
        5);

    // Get all points within the rectangle
    var points = rectangle.GetPoints()
                          .Cast<IPoint>()
                          .ToArray();

    // Filter points by line of sight and wall detection
    var validPoints = points.FilterByLineOfSight(Subject, Subject.MapInstance)
                            .Where(point => !Subject.MapInstance.IsWall(point) && !Subject.MapInstance.IsBlockingReactor(point))
                            .ToList();

    // Heal all valid friendly players standing on the valid points
    var targets = Subject.MapInstance
                         .GetEntitiesAtPoints<Aisling>(validPoints)
                         .Where(target => target.IsFriendlyTo(Subject))
                         .ToList();

    foreach (var target in targets)
    {
        // Step 1: Calculate the base heal amount using the source's WIS and 2% of the target's max HP
        var wis = SourceOfEffect.StatSheet.Wis;
        var targetMaxHp = target.StatSheet.EffectiveMaximumHp;
        var baseHealAmount = (wis * 5) + (targetMaxHp * 0.02); // WIS multiplier + 2% of target's max HP

        // Step 2: Add the source's EffectiveHealBonus to the base heal amount
        var healBonus = SourceOfEffect.StatSheet.EffectiveHealBonus;
        var totalHealWithBonus = baseHealAmount + healBonus;

        // Step 3: Multiply the result by the source's EffectiveHealBonusPct
        var healBonusPct = SourceOfEffect.StatSheet.EffectiveHealBonusPct / 100f;
        var finalHealAmount = totalHealWithBonus * (1 + healBonusPct); // Add 1 to include the base amount

        ApplyHealScript.ApplyHeal(
            SourceOfEffect,
            target,
            ApplyHealScript,
            (int)finalHealAmount);

        // Notify the target
        target.Client.SendAttributes(StatUpdateType.Vitality);
        target.Animate(Animation);
    }
}



    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        // Prevents applying to invulnerable targets
        if (target.Effects.Contains("Healing Aura"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The target is already affected by Healing Aura.");

            return false;
        }

        return true;
    }
}