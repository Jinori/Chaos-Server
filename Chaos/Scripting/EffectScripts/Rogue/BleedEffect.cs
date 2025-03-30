using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class BleedEffect : ContinuousAnimationEffectBase
{
    private readonly IApplyDamageScript DamageScript = ApplyNonAttackDamageScript.Create();

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(4);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 305,
        Priority = 25
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(2));

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(500), false);

    /// <inheritdoc />
    public override byte Icon => 39;

    /// <inheritdoc />
    public override string Name => "Bleed";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var pctDmg = DamageHelper.CalculatePercentDamage(Source, Subject, 1);
        var flatDmg = Source.StatSheet.EffectiveDex * (Source.StatSheet.Level / 5);
        var capDmg = MathEx.GetPercentOf<int>((int)Subject.StatSheet.EffectiveMaximumHp, 2);

        var finalDmg = pctDmg + flatDmg;
        finalDmg = Math.Min(finalDmg, capDmg);

        DamageScript.ApplyDamage(
            Source,
            Subject,
            this,
            finalDmg);
    }
}