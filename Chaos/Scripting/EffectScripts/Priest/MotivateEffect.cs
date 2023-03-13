using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class MotivateEffect : EffectBase
{
    public override byte Icon => 13;
    public override string Name => "Motivate";

    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(2);

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            AtkSpeedPct = 40
        };

        AislingSubject?.StatSheet.SubtractMp(750);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Attack Speed increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            AtkSpeedPct = 40
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Attack Speed has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("motivate"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Damage has already been applied.");

            return false;
        }

        return true;
    }
}