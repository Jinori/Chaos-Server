using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class PowerEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } =
        [
            "Small Haste",
            "Haste",
            "Strong Haste",
            "Potent Haste",
            "Small Power",
            "Power",
            "Strong Power",
            "Potent Power",
            "Small Accuracy",
            "Accuracy",
            "Strong Accuracy",
            "Potent Accuracy",
            "Juggernaut",
            "Strong Juggernaut",
            "Potent Juggernaut",
            "Astral",
            "Strong Astral",
            "Potent Astral"
        ];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 127,
        AnimationSpeed = 100
    };

    public override byte Icon => 13;
    public override string Name => "Power";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Dmg = 6
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Damage has increased by 6.");
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
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}