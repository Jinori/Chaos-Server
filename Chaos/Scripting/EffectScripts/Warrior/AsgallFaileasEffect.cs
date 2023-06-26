using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class AsgallFaileasEffect : EffectBase
{
    public override byte Icon => 44;
    public override string Name => "asgallfaileas";

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(12);

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.AsgallFaileas))
            Subject.Status = Status.AsgallFaileas;
        
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your stance is more defensive.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.AsgallFaileas))
            Subject.Status &= ~Status.AsgallFaileas;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your stance has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.StatSheet.CurrentMp < 310)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana.");

            return false;
        }

        if (target.Effects.Contains("asgallfaileas"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");

            return false;
        }

        return true;
    }
}