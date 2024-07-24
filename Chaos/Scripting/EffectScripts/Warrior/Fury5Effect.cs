using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.ItemScripts;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class Fury5Effect : EffectBase
{

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(8);
    public override byte Icon => 87;
    public override string Name => "Fury5";
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override void OnApplied()
    {
        base.OnApplied();
        AislingSubject?.StatSheet.SubtractHp(128000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 5 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your fury returns to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("fury5") && target.StatSheet.CurrentHp <= 128000)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 128000 health to enter Fury 5.");

            return false;
        }
        
        source.Effects.Terminate("Fury4");
        return true;
    }
}