using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public sealed class PramhEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(12);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 28
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
    /// <inheritdoc />
    public override byte Icon => 95;
    /// <inheritdoc />
    public override string Name => "pramh";

    public override void OnApplied() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your eyelids become heavy.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        Subject.Animate(Animation);
        AislingSubject?.Client.SendCancelCasting();
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You awake from your slumber.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Script.Is<ThisIsABossScript>())
            return false;
        
        if (target.Effects.Contains("pramh")
            || target.Effects.Contains("beagpramh"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already asleep.");

            return false;
        }

        return true;
    }
}