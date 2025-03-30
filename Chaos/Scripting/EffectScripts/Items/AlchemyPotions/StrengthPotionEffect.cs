using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class StrengthPotionEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } =
        [
            "Strength Potion",
            "Intellect Potion",
            "Wisdom Potion",
            "Constitution Potion",
            "Dexterity Potion"
        ];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(20);

    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 508,
        AnimationSpeed = 100
    };

    public override byte Icon => 71;
    public override string Name => "Strength Potion";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Str = 10
        };

        Subject.StatSheet.AddBonus(attributes);
        Subject.Animate(Animation);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your strength increases by 10.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Str = 10
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your strength potion wore off!");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}