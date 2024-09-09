using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class WisdomPotionEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 509,
        AnimationSpeed = 100
    };

    public List<string> ConflictingEffectNames { get; init; } = 
    [
        "Strength Potion",
        "Intelligence Potion",
        "Wisdom Potion",
        "Constitution Potion",
        "Dexterity Potion"
    ];
    public override byte Icon => 71;
    public override string Name => "Wisdom Potion";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Wis = 6
        };

        Subject.Animate(Animation);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your wisdom increases by 6.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Wis = 6
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your wisdom potion wore off!");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}