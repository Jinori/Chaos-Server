using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BeagAiteEffect : NonOverwritableEffectBase
{
    public override byte Icon => 11;
    public override string Name => "beag naohm aite";

    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(3);

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
    
    protected override byte? Sound => 140;

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.BeagAite))
            Subject.Status = Status.BeagAite;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Glioca has blessed your defenses.");
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