using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class Cunning6Effect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(16);

    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 76
    };

    public override byte Icon => 74;
    public override string Name => "Cunning6";

    public override void OnApplied()
    {
        base.OnApplied();
        var attributes = new Attributes
        {
            Dmg = 80,
            SkillDamagePct = 80
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.StatSheet.SubtractMp(256000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bCunning 6 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnReApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bCunning 6 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Dmg = 80,
            SkillDamagePct = 80
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your cunning returns to normal.");
    }
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Cunning6") && (target.StatSheet.CurrentMp <= 256000))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 256000 mana to enter Cunning 6.");

            return false;
        }

        return true;
    }
}