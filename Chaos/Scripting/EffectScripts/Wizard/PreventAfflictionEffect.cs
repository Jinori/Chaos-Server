using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class PreventAfflictionEffect : EffectBase
{
    public override byte Icon => 112;
    public override string Name => "preventaffliction";

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(18);

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.PreventAffliction))
            Subject.Status = Status.PreventAffliction;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your magic has been simplified.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.PreventAffliction))
            Subject.Status &= ~Status.PreventAffliction;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your magic returns to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("preventaffliction"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "This magic has already been simplified.");

            return false;
        }

        return true;
    }
}