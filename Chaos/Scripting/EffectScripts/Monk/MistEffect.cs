using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class MistEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);
    public override byte Icon => 18;
    public override string Name => "mist";

    public override void OnApplied()
    {
        base.OnApplied();

        var attributesToSubtract = new Attributes
        {
            Ac = 20
        };

        var attributesToAdd = new Attributes
        {
            MagicResistance = -20
        };

        Subject.StatSheet.SubtractBonus(attributesToSubtract);
        Subject.StatSheet.AddBonus(attributesToAdd);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor increased while magic resist decreased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributesToSubtract = new Attributes
        {
            MagicResistance = 20
        };

        var attributesToAdd = new Attributes
        {
            Ac = 20
        };

        Subject.StatSheet.SubtractMp(100);
        Subject.StatSheet.SubtractBonus(attributesToSubtract);
        Subject.StatSheet.AddBonus(attributesToAdd);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor and MR has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("mist"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Mist has already been applied.");

            return false;
        }

        return true;
    }
}