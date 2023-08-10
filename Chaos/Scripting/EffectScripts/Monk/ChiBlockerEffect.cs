using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class ChiBlockerEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(25);
    public override byte Icon => 171;
    public override string Name => "chiBlocker";

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.ChiBlocker))
            Subject.Status = Status.ChiBlocker;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.ChiBlocker))
            Subject.Status &= ~Status.ChiBlocker;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }
}