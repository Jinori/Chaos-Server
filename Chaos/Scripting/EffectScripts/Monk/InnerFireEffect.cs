﻿using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class InnerFireEffect : ContinuousAnimationEffectBase
{
    public override byte Icon => 126;
    public override string Name => "innerFire";
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 300,
        TargetAnimation = 5
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMinutes(8));

    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(8);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(400));

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.InnerFire))
            Subject.Status = Status.InnerFire;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body is at inner peace.");
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed() { }

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.InnerFire))
            Subject.Status &= ~Status.InnerFire;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body returns to a normal state.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("innerFire"))
        {
            (source as Aisling)?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "Your body is already feeling the affects of inner peace.");

            return false;
        }

        return true;
    }
}