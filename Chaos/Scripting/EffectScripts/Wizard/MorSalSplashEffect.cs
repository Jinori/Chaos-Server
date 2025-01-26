using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class MorSalSplashEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(8);

    private Creature SourceOfEffect { get; set; } = null!;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 234
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(20000), false);

    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 58
    };

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 38;

    /// <inheritdoc />
    public override string Name => "Mor Sal Splash";

    public override void OnApplied()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Powerful water surrounds you.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.MapInstance != SourceOfEffect.MapInstance)
        {
            Subject.Effects.Terminate("MorSalSplash");

            return;
        }

        // Animate the subject of the effect
        Subject.Animate(Animation);

        // Get the points around the subject where the effect is applied
        var options = new AoeShapeOptions
        {
            Source = new Point(Subject.X, Subject.Y),
            Range = 1
        };

        var points = AoeShape.AllAround.ResolvePoints(options);

        // Retrieve and filter targets at those points
        var targets = Subject.MapInstance
                             .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                             .WithFilter(SourceOfEffect, TargetFilter.HostileOnly)
                             .Where(x => !Subject.Equals(x) && !x.MapInstance.IsWall(x))
                             .ToList();

        // Apply damage to each valid target
        foreach (var target in targets)
        {
            if (target.StatSheet.DefenseElement == Element.Water)
                continue;

            ApplyDamageScript.ApplyDamage(
                SourceOfEffect,
                target,
                this,
                (SourceOfEffect.StatSheet.Level + SourceOfEffect.StatSheet.EffectiveInt) * 5 + 100,
                Element.Water);

            // Show target health status and animate the effect
            target.ShowHealth();
            target.Animate(CreatureAnimation);
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Splash has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;

        if (target.IsFriendlyTo(source) || target.IsGodModeEnabled() || target.Effects.Contains("Invulnerability"))
            return false;

        if (target.StatSheet.DefenseElement == Element.Water)
            return false;

        var splashEffects = new[]
        {
            "Beag Sal Splash",
            "Sal Splash",
            "Mor Sal Splash",
            "Ard Sal Splash"
        };

        return !splashEffects.Any(target.Effects.Contains);
    }
}