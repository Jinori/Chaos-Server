using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest.Shadow;

public class BindEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

    private Animation? Animation { get; } = new()
    {
        TargetAnimation = 820,
        AnimationSpeed = 100
    };

    /// <inheritdoc />
    public List<string> EffectNameHierarchy { get; init; } = ["Bind"];
    public override byte Icon => 77;
    public override string Name => "Bind";
    
    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            AtkSpeedPct = 30
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel your attack speed being drained from you.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            AtkSpeedPct = 30
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Attack speed has returned to normal.");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();
        
        return execution is not null;
    }
}