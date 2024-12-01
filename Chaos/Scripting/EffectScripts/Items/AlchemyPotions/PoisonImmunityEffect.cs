using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class PoisonImmunityEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } = [];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30);

    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 491,
        AnimationSpeed = 100
    };

    public override byte Icon => 12;
    public override string Name => "Poison Immunity";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        Subject.Animate(Animation);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You become immune to poison.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Poison immunity wears off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Poison Immunity"))
            target.Effects.Dispel("Poison Immunity");

        if (target.Effects.Contains("Poison"))
            target.Effects.Dispel("Poison");

        return true;
    }
}