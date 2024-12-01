using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class RuminationEffect : ContinuousAnimationEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(999);
    protected Point Point { get; set; }

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 20,
        TargetAnimation = 295,
        Priority = 10
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    public override byte Icon => 19;
    public override string Name => "Rumination";

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            $"{MessageColor.Silver.ToPrefix()}You begin to sacrifice your health in return for mana gains..");

        Point = new Point(Subject.X, Subject.Y);
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed()
    {
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

        //Remove and Add MP
        Subject.StatSheet.SubtractHealthPct(8);
        Subject.StatSheet.AddManaPct(6);

        //Show Vitality
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.ShowHealth();
    }

    public override void OnTerminated() => AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Rumination"))
        {
            (source as Aisling)?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{MessageColor.Silver.ToPrefix()}Your body is already sacrificing itself.");

            return false;
        }

        return true;
    }
}