using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Scripts.EffectScripts.Abstractions;

namespace Chaos.Scripts.EffectScripts.Monk;

public class ChiBlockerEffect : EffectBase
{
    public override byte Icon => 171;
    public override string Name => "chiBlocker";
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(25);

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