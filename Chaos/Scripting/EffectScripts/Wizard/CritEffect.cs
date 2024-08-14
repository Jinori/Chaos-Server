using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public sealed class CritEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(6);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 168
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));
    /// <inheritdoc />
    public override byte Icon => 34;
    /// <inheritdoc />
    public override string Name => "Crit";

    public override void OnApplied() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel vunerable.");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        Subject.Animate(Animation);
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are no longer vulnerable.");
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Script.Is<ThisIsABossScript>())
            return false;
        
        if (target.Effects.Contains("Crit"))
            return false;

        return true;
    }
}