using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.ItemScripts;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class Fury4Effect : EffectBase
{

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    public override byte Icon => 87;
    public override string Name => "Fury4";
    
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 291
    };

    public override void OnApplied()
    {
        base.OnApplied();
        AislingSubject?.Effects.Terminate("Fury3");
        AislingSubject?.StatSheet.SubtractHp(64000);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 4 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }

    public override void OnReApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "{=bFury 4 builds up inside you.");
        AislingSubject?.Animate(Animation);
    }
    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your fury returns to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("fury4") && target.StatSheet.CurrentHp <= 64000)
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You need 64000 health to enter Fury 4.");

            return false;
        }
        return true;
    }
}