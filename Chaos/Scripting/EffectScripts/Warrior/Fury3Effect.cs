using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class Fury3Effect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(12);

    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override byte Icon => 87;
    public override string Name => "Fury3";

    private Attributes BonusAttributes = null!;
    
    public override void OnApplied()
    {
        base.OnApplied();
        AislingSubject?.Effects.Terminate("Fury2");

        BonusAttributes = new Attributes
        {
            SkillDamagePct = GetVar<int>("skillDamagePct"),
            Dmg = GetVar<int>("dmg"),
            FlatSkillDamage = GetVar<int>("flatSkillDamage")
        };
        
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.StatSheet.SubtractHp(32000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 3 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        SetVar("skillDamagePct", 30);
        SetVar("dmg", 30);
        SetVar("flatSkillDamage", 300);
    }
    
    public override void OnDispelled() => OnTerminated();

    public override void OnReApplied()
    {
        BonusAttributes = new Attributes
        {
            SkillDamagePct = GetVar<int>("skillDamagePct"),
            Dmg = GetVar<int>("dmg"),
            FlatSkillDamage = GetVar<int>("flatSkillDamage")
        };
        
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 3 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your fury returns to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Fury3") && (target.StatSheet.CurrentHp <= 32000))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 32000 health to enter Fury 3.");

            return false;
        }

        return true;
    }
}