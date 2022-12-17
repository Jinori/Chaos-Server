using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;
using Chaos.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class SmashVialManaEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 144;
    /// <inheritdoc />
    public override string Name => "ManaRegen";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 127
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(7);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        //the interval is 100ms, so this will be applied 10 times a second
        const int MANA_PER_TICK = 5;
        Subject.StatSheet.AddMp(MANA_PER_TICK);
        //if the subject was a player, update their vit
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}
