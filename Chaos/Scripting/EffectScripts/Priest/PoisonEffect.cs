using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class PoisonEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 247
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(500));
    /// <inheritdoc />
    public override byte Icon => 27;
    /// <inheritdoc />
    public override string Name => "Poison";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        double maxHp = Subject.StatSheet.MaximumHp;
        const double DAMAGE_PERCENTAGE = 0.005;
        const int DAMAGE_CAP = 500;

        var damage = (int)Math.Min(maxHp * DAMAGE_PERCENTAGE, DAMAGE_CAP);

        if (Subject.StatSheet.TrySubtractHp(damage))
        {
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        }
    }

}