using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class BerserkEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);

    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override byte Icon => 87;
    public override string Name => "Berserk";

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = 10,
            AtkSpeedPct = 25,
            FlatSkillDamage = 150
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
            AtkSpeedPct = 25,
            FlatSkillDamage = 150
        };

        AislingSubject?.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your emotions return to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.StatSheet.CurrentHp <= 1000)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough health to go Berserk.");

            return false;
        }

        if (target.Effects.Contains("Berserk"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You're currently berserking!");

            return false;
        }

        return true;
    }
}