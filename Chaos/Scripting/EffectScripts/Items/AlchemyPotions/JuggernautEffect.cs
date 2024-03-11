using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class JuggernautEffect : NonOverwritableEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
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
    public override byte Icon => 13;
    public override string Name => "Juggernaut";
    protected override byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            MaximumHp = 200
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Maximum Health has increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            MaximumHp = 200
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Maximum Health has returned to normal.");
    }
}