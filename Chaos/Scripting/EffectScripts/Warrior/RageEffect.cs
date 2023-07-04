using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class RageEffect : EffectBase
{
    private int _damageSaved;

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(25);
    public override byte Icon => 87;
    public override string Name => "rage";

    public override void OnApplied()
    {
        base.OnApplied();

        var damFormula = (Subject.StatSheet.EffectiveStr + Subject.StatSheet.EffectiveCon) / 5;

        var attributes = new Attributes
        {
            Dmg = 10 + damFormula
        };

        _damageSaved = 10 + damFormula;

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bA violent rage builds up inside you.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Dmg = _damageSaved
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your rage has subsided.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("rage"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You couldn't possibly get any more pissed.");

            return false;
        }

        return true;
    }
}