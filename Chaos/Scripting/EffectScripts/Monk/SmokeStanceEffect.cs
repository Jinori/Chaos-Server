using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class SmokeStanceEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    public override byte Icon => 93;
    public override string Name => "Smoke Stance";
    private Attributes BonusAttributes = null!;

    public override void OnApplied()
    {
        base.OnApplied();

        BonusAttributes = new Attributes
        {
            MaximumHp = -GetVar<int>("reservedHp"),
            SkillDamagePct = GetVar<int>("skillDmgPct"),
            FlatSkillDamage = GetVar<int>("flatSkillDmg")
        };
        
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body becomes malleable and emits smoke");
    }

    public override void OnDispelled() => OnTerminated();

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        var reservedHp = Subject.StatSheet.EffectiveMaximumHp / 10;
        var pct = 5 + reservedHp / 500;
        var flat =  50 + reservedHp / 50;

        SetVar("reservedHp", reservedHp);
        SetVar("skillDmgPct", pct);
        SetVar("flatSkillDmg", flat);
    }

    public override void OnTerminated()
    {
        AislingSubject?.StatSheet.SubtractBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The smoke dissipates around your body.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Smoke Stance") && !target.Effects.Contains("Flame Stance"))
            return true;

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");

        return false;
    }
}