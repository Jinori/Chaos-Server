using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class Cunning4Effect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    private Attributes BonusAttributes = null!;
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 76
    };

    public override byte Icon => 74;
    public override string Name => "Cunning4";

    public override void OnApplied()
    {
        base.OnApplied();
        AislingSubject?.Effects.Terminate("Cunning3");

        BonusAttributes = new Attributes
        {
            SkillDamagePct = GetVar<int>("skillDamagePct"),
            FlatSkillDamage = GetVar<int>("flatSkillDamage")
        };
        
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.StatSheet.SubtractMp(32000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bCunning 4 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnReApplied()
    {
        BonusAttributes = new Attributes
        {
            SkillDamagePct = GetVar<int>("skillDamagePct"),
            FlatSkillDamage = GetVar<int>("flatSkillDamage")
        };
        
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bCunning 4 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your cunning returns to normal.");
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        SetVar("skillDamagePct", 53);
        SetVar("flatSkillDamage", 350);
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Cunning4") && (target.StatSheet.CurrentMp <= 32000))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 32000 mana to enter Cunning 4.");

            return false;
        }

        return true;
    }
}