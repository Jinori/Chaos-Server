using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class KnowledgeEffect : NonOverwritableEffectBase
{
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 127,
        AnimationSpeed = 100
    };
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Knowledge"
    };

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    public override byte Icon => 10;
    public override string Name => "Knowledge";
    protected override byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You gain increased experience.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "Your experience gain has returned to normal.");
}