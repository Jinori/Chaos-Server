using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BeagAiteEffect : NonOverwritableEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(3);
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 125,
        AnimationSpeed = 100
    };
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "beag naomh aite",
        "naomh aite",
        "mor naomh aite",
        "ard naomh aite"
    };
    public override byte Icon => 9;
    public override string Name => "beag naomh aite";

    protected override byte? Sound => 31;

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.BeagAite))
            Subject.Status = Status.BeagAite;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have been blessed.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.BeagAite))
            Subject.Status &= ~Status.BeagAite;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have returned to normal.");
    }
}