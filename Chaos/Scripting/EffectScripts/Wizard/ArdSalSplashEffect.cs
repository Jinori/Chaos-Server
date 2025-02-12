using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class ArdSalSplashEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(9);

    private Creature? SourceOfEffect { get; set; }

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
    public override string Name => "Ard Sal Splash";

    public override void OnApplied()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Powerful water surrounds you.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (SourceOfEffect == null)
        {
            AislingSubject?.Effects.Terminate(Name);

            return;
        }

        if ((AislingSubject == null) && !Subject.Script.Is<PetScript>())
        {
            Subject.Effects.Terminate("Ard Sal Splash");

            return;
        }

        if (Subject.MapInstance != SourceOfEffect.MapInstance)
        {
            Subject.Effects.Terminate("Ard Sal Splash");

            return;
        }

        // Animate the subject of the effect
        Subject.Animate(Animation);

        // Get the points around the subject where the effect is applied
        var options = new AoeShapeOptions
        {
            Source = new Point(Subject.X, Subject.Y),
            Range = 1,
            Direction = Direction.All
        };

        var points = AoeShape.AllAround.ResolvePoints(options);

        // Retrieve and filter targets at those points
        var targets = Subject.MapInstance
                             .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                             .WithFilter(Subject, TargetFilter.HostileOnly)
                             .WithFilter(Subject, TargetFilter.AliveOnly)
                             .Where(x => !x.Equals(Subject) && !x.MapInstance.IsWall(x))
                             .ToList();

        // Apply damage to each valid target
        foreach (var target in targets)
        {
            if (target.StatSheet.DefenseElement == Element.Water)
                continue;

            var pctMana = SourceOfEffect.StatSheet.EffectiveMaximumMp * 0.02;

            ApplyDamageScript.ApplyDamage(
                SourceOfEffect,
                target,
                this,
                (SourceOfEffect.StatSheet.Level + SourceOfEffect.StatSheet.EffectiveInt) * 10 + 1000 + (int)pctMana,
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