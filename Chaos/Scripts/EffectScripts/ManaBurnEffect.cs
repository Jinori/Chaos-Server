﻿using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;
using Chaos.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.EffectScripts;
public class ManaBurnEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon { get; } = 187;
    /// <inheritdoc />
    public override string Name { get; } = "Manaburn";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 135
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(18);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(500));

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        const int DAMAGE_PER_TICK = 1;

        if (Subject.StatSheet.CurrentMp <= DAMAGE_PER_TICK)
            return;

        Subject.StatSheet.SubtractManaPct(DAMAGE_PER_TICK);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}