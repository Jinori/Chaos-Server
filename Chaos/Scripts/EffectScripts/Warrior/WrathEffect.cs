﻿using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts.Warrior;

public class WrathEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon { get; } = 176;
    /// <inheritdoc />
    public override string Name { get; } = "Wrath";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 95
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    protected IApplyDamageScript ApplyDamageScript { get; }
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(1);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(400));

    public WrathEffect() => ApplyDamageScript = DefaultApplyDamageScript.Create();

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.StatSheet.ManaPercent < 1)
        {
            Subject.Effects.Terminate(Name);

            return;
        }

        Subject.StatSheet.SubtractManaPct(1);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);

        var points = AoeShape.AllAround.ResolvePoints(Subject);

        var targets =
            Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>()).WithFilter(Subject, TargetFilter.HostileOnly);

        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(Subject, target, this, Subject.StatSheet.Level);
            target.ShowHealth();
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "You end your wrath and your skin turns to normal.");
}