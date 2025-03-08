using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class MiracleEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);
    public override byte Icon => 124;
    public override string Name => "Miracle";

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            HealBonusPct = 10
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Miraelis' miracle has worn off.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            HealBonusPct = 10
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Miraelis' miracle expands your crafting and healing.");
    }
}