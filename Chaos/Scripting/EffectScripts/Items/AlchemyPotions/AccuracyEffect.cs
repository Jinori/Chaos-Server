using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class AccuracyEffect : NonOverwritableEffectBase
{
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 127,
        AnimationSpeed = 100
    };

    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Small Haste",
        "Haste",
        "Strong Haste",
        "Small Power",
        "Power",
        "Strong Power",
        "Small Accuracy",
        "Accuracy",
        "Strong Accuracy",
        "Juggernaut",
        "Strong Juggernaut",
        "Strong Astral",
        "Astral"
    };

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    public override byte Icon => 12;
    public override string Name => "Accuracy";
    protected override byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Hit = 5
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your hit chance increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Hit = 5
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your hit chance has returned to normal.");
    }
}