using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class AmnesiaEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 42
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    protected IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Amnesia"
    };
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(700));
    /// <inheritdoc />
    public override byte Icon => 15;
    /// <inheritdoc />
    public override string Name => "Amnesia";

    protected void OnApply()
    {
        if (Subject is not Monster monster)
            return;

        monster.ResetAggro();
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject is not Monster monster)
            return;

        monster.ResetAggro();
    }
}