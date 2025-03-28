﻿using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class FasDeireasEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Torment",
            "Mor Fas Deireas",
            "Fas Deireas"
        ];

    private Animation? Animation { get; } = new()
    {
        TargetAnimation = 520,
        AnimationSpeed = 100
    };

    public override byte Icon => 106;
    public override string Name => "Fas Deireas";

    protected byte? Sound => 124;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Dmg = 6
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Dmg = 6
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Damage has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        
        return execution is not null;
    }
}