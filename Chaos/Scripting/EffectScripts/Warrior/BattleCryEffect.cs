using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class BattleCryEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);
    public override byte Icon => 88;
    public override string Name => "Battle Cry";
    
    private int _damageSaved;

    public override void OnApplied()
    {
        base.OnApplied();

        var buff = 15 + (Subject.StatSheet.EffectiveStr / 10);
        
        var attributes = new Attributes
        {
            AtkSpeedPct = buff,
            Dmg = buff
        };

        _damageSaved = buff;
        
        AislingSubject?.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your accelerate faster as your damage rises.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            AtkSpeedPct = _damageSaved,
            Dmg = _damageSaved
        };

        AislingSubject?.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Battle Cry"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your muscles are at their maximum.");

            return false;
        }

        return true;
    }
}