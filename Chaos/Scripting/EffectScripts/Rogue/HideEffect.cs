using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class HideEffect : EffectBase
{
    public override byte Icon => 10;
    public override string Name => "hide";

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.Hide))
            Subject.Status = Status.Hide;
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You become hidden to your surroundings.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.Hide))
            Subject.Status &= ~Status.Hide;

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are no longer shrouded in darkness.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("hide")) 
            return true;
        
        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are currently hidden from view already.");
        return false;
    }
}