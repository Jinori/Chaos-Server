using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BeannaichEffect : EffectBase
{
    public override byte Icon => 16;
    public override string Name => "beannaich";

    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(5);

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Dmg = 5
        };

        AislingSubject?.StatSheet.SubtractMp(120);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Damage increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Dmg = 5
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Damage has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("beannaich"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Damage has already been applied.");

            return false;
        }

        return true;
    }
}