using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class DropBoostEffect : EffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(60);

    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 975,
        AnimationSpeed = 100
    };

    public override byte Icon => 123;
    public override string Name => "DropBoost";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Animate(Animation);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You gain 25% increased gold and items for monster kills.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your gold and item gain has returned to normal.");
}