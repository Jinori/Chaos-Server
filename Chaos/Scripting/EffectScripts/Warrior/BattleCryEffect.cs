using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class BattleCryEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    private Attributes BonusAttributes = null!;

    public override byte Icon => 89;
    public override string Name => "Battle Cry";

    public override void OnApplied()
    {
        base.OnApplied();

        BonusAttributes = new Attributes
        {
            AtkSpeedPct = GetVar<int>("atkSpeedPct"),
            Dmg = GetVar<int>("dmg"),
            FlatSkillDamage = GetVar<int>("flatSkillDmg")
        };

        AislingSubject?.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your accelerate faster as your damage rises.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.StatSheet.SubtractBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body has returned to normal.");
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        var buff = 15 + source.StatSheet.EffectiveStr / 10;
        var flat = 25 + source.StatSheet.EffectiveStr;

        SetVar("dmg", buff);
        SetVar("atkSpeedPct", buff);
        SetVar("flatSkillDmg", flat);
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