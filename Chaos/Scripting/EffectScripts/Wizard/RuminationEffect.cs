using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class RuminationEffect : ContinuousAnimationEffectBase
{
    protected Point Point { get; set; }
    public override byte Icon => 91;
    public override string Name => "rumination";

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 300,
        TargetAnimation = 58
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.Rumination))
            Subject.Status = Status.Rumination;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            $"{MessageColor.Silver.ToPrefix()}You begin to sacrifice your health in return for mana gains..");

        Point = new Point(Subject.X, Subject.Y);
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed()
    {
        //Check if a player is casting a new spell

        if (AislingSubject?.LastSpellCastTemplateName?.EqualsI("rumination") is false)
        {
            AislingSubject?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}You cannot gain mana while casting other spells.");

            Subject.Effects.Terminate(Name);

            return;
        }

        //Check if they have moved from the original location
        var currentPoint = new Point(Subject.X, Subject.Y);

        if (!Point.Equals(currentPoint))
        {
            AislingSubject?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}You moved while trying to concentrate.");

            Subject.Effects.Terminate(Name);

            return;
        }

        //If their mana bar is full, stop the effect from draining further hp
        if (Subject.StatSheet.CurrentMp >= Subject.StatSheet.EffectiveMaximumMp)
        {
            Subject.Effects.Terminate(Name);

            AislingSubject?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}You cannot gain any more mana.");

            return;
        }

        //If they drop under 2% health. Stop the effect.
        if (Subject.StatSheet.HealthPercent < 10)
        {
            Subject.Effects.Terminate(Name);

            AislingSubject?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}You stop your mana recovery due to health concerns.");

            return;
        }

        var healthCost = Subject.StatSheet.EffectiveMaximumHp * .04;
        //Remove and Add HP
        Subject.StatSheet.SubtractHealthPct(4);
        Subject.StatSheet.AddMp((int)(healthCost * .66));

        //Show Vitality
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.Rumination))
            Subject.Status &= ~Status.Rumination;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("rumination"))
        {
            (source as Aisling)?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}Your body is already sacrificing itself.");

            return false;
        }

        return true;
    }
}