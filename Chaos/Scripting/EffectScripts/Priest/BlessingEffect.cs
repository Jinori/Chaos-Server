using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BlessingEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 512,
        AnimationSpeed = 100
    };

    public List<string> EffectNameHierarchy { get; init; } =
    [
        "blessing",
        "ard naomh aite",
        "mor naomh aite",
        "naomh aite",
        "beag naomh aite",
        "armachd",
        "mor beannaich",
        "beannaich",
        "mor fas deireas",
        "fas deireas",
        "motivate",
    ];
    
    public override byte Icon => 10;
    public override string Name => "blessing";

    protected byte? Sound => 122;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -12,
            Dmg = 14,
            Hit = 22,
            AtkSpeedPct = 45
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel blessed.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -12,
            Dmg = 14,
            Hit = 22,
            AtkSpeedPct = 45
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Blessing has faded.");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();
        
        return execution is not null;
    }
}