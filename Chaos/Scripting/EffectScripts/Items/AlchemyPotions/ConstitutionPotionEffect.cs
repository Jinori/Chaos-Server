﻿using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class ConstitutionPotionEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 510,
        AnimationSpeed = 100
    };

    public List<string> ConflictingEffectNames { get; init; } = 
    [
        "Strength Potion",
        "Intellect Potion",
        "Wisdom Potion",
        "Constitution Potion",
        "Dexterity Potion"
    ];
    public override byte Icon => 71;
    public override string Name => "Constitution Potion";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Con = 6
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your constitution increases by 6.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Con = 6
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your constitution potion wore off!");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}