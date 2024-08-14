using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class BeagSalSplashEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(6);
    private Creature SourceOfEffect { get; set; } = null!;
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 234
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(20000));
    protected IApplyDamageScript ApplyDamageScript { get; }

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 58
    };
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    public override byte Icon => 38;
    /// <inheritdoc />
    public override string Name => "BeagSalSplash";

    public BeagSalSplashEffect() => ApplyDamageScript = ApplyAttackDamageScript.Create();

    public override void OnApplied() =>
        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "Powerful water surrounds you.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        // Animate the subject of the effect
        Subject.Animate(Animation);

        // Get the points around the Subject where the effect is applied
        var points = AoeShape.AllAround.ResolvePoints(Subject);

        // Retrieve the targets at those points, filtering for hostiles
        var targets =
            Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                .WithFilter(SourceOfEffect, TargetFilter.HostileOnly)
                .ToList();

        // Apply damage to each valid target around the subject
        foreach (var target in targets)
        {
            // Skip the subject itself if it appears in the target list
            if (target == Subject)
                continue;

            // Apply damage from the SourceOfEffect to the target
            ApplyDamageScript.ApplyDamage(
                SourceOfEffect,
                target,
                this,
                (SourceOfEffect.StatSheet.Level + SourceOfEffect.StatSheet.EffectiveInt) * 5 + 100,
                Element.Water);

            // Show health status of the target after damage
            target.ShowHealth();

            // Animate the target to show it was affected
            target.Animate(CreatureAnimation);
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "Splash has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;
        
        if (target.IsFriendlyTo(source))
            return false;

        if (target.Effects.Contains("BeagSalsplash") || target.Effects.Contains("SalSplash") || target.Effects.Contains("MorSalSplash") || target.Effects.Contains("ArdSalSplash"))
            return false;

        return true;
    }
}