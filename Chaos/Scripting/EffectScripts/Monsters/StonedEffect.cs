using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monsters;

public sealed class StonedEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(3);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 24,
        Priority = 90
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    private IIntervalTimer DeathInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(20), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 118;

    /// <inheritdoc />
    public override string Name => "Stoned";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        Subject.Animate(Animation);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body is turning to stone.");
        AislingSubject?.Client.SendCancelCasting();

        if (DeathInterval.IntervalElapsed)
        {
            if (Subject.IsGodModeEnabled())
                return;
            
            Subject.StatSheet.SetHealthPct(0);
            Subject.Script.OnDeath();

            Subject.Animate(Animation);
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel fine again.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Script.Is<ThisIsABossScript>())
            return false;

        if (target.IsGodModeEnabled())
            return false;

        if (target.IsStoned())
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already stoned.");

            return false;
        }

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {target.Name}.");
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{source.Name} casted {Name} on you.");

        return true;
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Interval.Update(delta);
        DeathInterval.Update(delta);

        if (Interval.IntervalElapsed)
            OnIntervalElapsed();
    }
}