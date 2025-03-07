﻿using Chaos.Common.Definitions;
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

namespace Chaos.Scripting.EffectScripts.Warrior;

public class WhirlwindEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 185
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);
    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();
    /// <inheritdoc />
    public override byte Icon => 98;
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
    /// <inheritdoc />
    public override string Name => "Whirlwind";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
            if (Subject.StatSheet.CurrentMp < 20)
            {
                Subject.Effects.Terminate(Name);
                return;
            }

            Subject.StatSheet.SubtractMp(20);
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);

        var options = new AoeShapeOptions
            {
                Source = new Point(Subject.X, Subject.Y),
                Range = 1
            };

            var points = AoeShape.AllAround.ResolvePoints(options);

        var targets =
            Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>()).WithFilter(Subject, TargetFilter.HostileOnly).ToList();

        var damage = (Subject.StatSheet.EffectiveStr * 3);
        
        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(
                Subject,
                target,
                this,
                damage);

            target.ShowHealth();
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "Your whirlwind calms down.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.StatSheet.ManaPercent <= 5)
            return false;

        return true;
    }
}