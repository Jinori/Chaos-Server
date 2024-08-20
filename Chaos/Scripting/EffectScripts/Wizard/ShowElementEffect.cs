using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class ShowElementEffect : ContinuousAnimationEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(9);
    protected Point Point { get; set; }

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 295
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    public override byte Icon => 19;
    public override string Name => "Show Element";

    public override void OnApplied()
    {
        base.OnApplied();

        if (Subject is Aisling)
            return;

        if (Subject.StatSheet.DefenseElement == Element.Earth)
            Animation.TargetAnimation = 401;

        if (Subject.StatSheet.DefenseElement == Element.Water)
            Animation.TargetAnimation = 402;

        if (Subject.StatSheet.DefenseElement == Element.Fire)
            Animation.TargetAnimation = 404;

        if (Subject.StatSheet.DefenseElement == Element.Wind)
            Animation.TargetAnimation = 403;

        if (Subject.StatSheet.DefenseElement == Element.Darkness)
            Animation.TargetAnimation = 76;

        if (Subject.StatSheet.DefenseElement == Element.Holy)
            Animation.TargetAnimation = 277;

        if (Subject.StatSheet.DefenseElement == Element.Metal)
            Animation.TargetAnimation = 237;

        if (Subject.StatSheet.DefenseElement == Element.None)
            Animation.TargetAnimation = 363;

        if (Subject.StatSheet.DefenseElement == Element.Wood)
            Animation.TargetAnimation = 235;

        if (Subject.StatSheet.DefenseElement == Element.Undead)
            Animation.TargetAnimation = 233;

        Subject.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed() =>
        //Check if they have moved from the original location
        Subject.Animate(Animation);

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target is Aisling { IsDead: true })
            return false;

        if (target.Effects.Contains("showelement"))
        {
            (source as Aisling)?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}The target is already exposing its weakness.");

            return false;
        }

        return true;
    }
}