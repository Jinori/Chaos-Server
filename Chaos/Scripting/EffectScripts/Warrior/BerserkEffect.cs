using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.ItemScripts;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class BerserkEffect : EffectBase
{

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);
    public override byte Icon => 87;
    public override string Name => "Berserk";
    
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override void OnApplied()
    {
        base.OnApplied();
        
        var attributes = new Attributes
        {
            Ac = 10,
            AtkSpeedPct = 10,
            Dmg = 15,
            SkillDamagePct = 15
        };

        AislingSubject?.StatSheet.AddBonus(attributes);
        AislingSubject?.StatSheet.SubtractHp(1000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bYou suddenly go berserk, increasing damage and AC.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = 10,
            AtkSpeedPct = 10,
            Dmg = 15,
            SkillDamagePct = 15
        };

        AislingSubject?.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your emotions return to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.StatSheet.CurrentHp <= 1000)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough health to go Berserk.");

            return false;
        }
        
        return true;
    }
}