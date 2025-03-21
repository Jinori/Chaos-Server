using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class Fury5Effect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(8);

    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override byte Icon => 87;
    public override string Name => "Fury5";

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Effects.Terminate("Fury4");
        var attributes = new Attributes
        {
            Dmg = 75,
            SkillDamagePct = 75
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.StatSheet.SubtractHp(128000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 5 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnReApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 5 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Dmg = 75,
            SkillDamagePct = 75
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your fury returns to normal.");
    }
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Fury5") && (target.StatSheet.CurrentHp <= 128000))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 128000 health to enter Fury 5.");

            return false;
        }

        return true;
    }
}