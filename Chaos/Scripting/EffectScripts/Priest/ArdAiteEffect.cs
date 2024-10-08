﻿using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class ArdAiteEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(20);
    public List<string> EffectNameHierarchy { get; init; } =
    [
        "blessing",
        "ard naomh aite",
        "mor naomh aite",
        "naomh aite",
        "beag naomh aite"
    ];
    
    private Animation? Animation { get; } = new()
    {
        TargetAnimation = 414,
        AnimationSpeed = 100
    };
    
    protected byte? Sound => 31;
    public override byte Icon => 9;
    public override string Name => "ard naomh aite";

    public override void OnApplied()
    {
        base.OnApplied();

        Subject.Animate(Animation!);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have been blessed.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have returned to normal.");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();
        
        return execution is not null;
    }
}