using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class Fury6Effect : EffectBase
{

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(6);
    public override byte Icon => 87;
    public override string Name => "Fury6";
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override void OnApplied()
    {
        base.OnApplied();
        AislingSubject?.StatSheet.SubtractHp(256000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 6 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your fury returns to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("fury6"))
        {
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are already in Fury 6.");
            return false;
        }
        if (!target.Effects.Contains("fury6") && target.StatSheet.CurrentHp <= 256000)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 256000 health to enter Fury 6.");

            return false;
        }
        
        source.Effects.Terminate("Fury5");
        return true;
    }
}