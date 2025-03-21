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
    
    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = GetSnapshotAttributes;
        
        AislingSubject?.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your accelerate faster as your damage rises.");
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        var buff = 15 + source.StatSheet.EffectiveStr / 10;
        var flat = 25 + source.StatSheet.EffectiveStr;
        
        SnapshotVars.Set("dmg", buff);
        SnapshotVars.Set("atkSpeedPct", buff);
        SnapshotVars.Set("flatSkillDmg", flat);
    }
    
    private Attributes GetSnapshotAttributes => new()
    {
        AtkSpeedPct = SnapshotVars.Get<int>("atkSpeedPct"),
        Dmg = SnapshotVars.Get<int>("dmg"),
        FlatSkillDamage = SnapshotVars.Get<int>("flatSkillDmg")
    };

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = GetSnapshotAttributes;
        
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