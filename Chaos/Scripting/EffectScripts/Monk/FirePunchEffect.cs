using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class FirePunchEffect : ContinuousAnimationEffectBase
{
    private bool FirstElapsed = false;
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(6);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 60,
        Priority = 15
    };
    /// <inheritdoc />
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
    /// <inheritdoc />
    public override byte Icon => 31;
    /// <inheritdoc />
    public override string Name => "Firepunch";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var damagePerTick = 5 * Subject.StatSheet.Level;

        if (Subject.StatSheet.CurrentHp <= damagePerTick)
            return;

        if (!Subject.StatSheet.TrySubtractHp(damagePerTick)) 
            return;
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        Subject.ShowHealth();
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Small Firestorm") && !target.Effects.Contains("Firestorm") &&
            !target.Effects.Contains("Firepunch")) 
            return true;
        
        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already burning.");

        return false;

    }
}