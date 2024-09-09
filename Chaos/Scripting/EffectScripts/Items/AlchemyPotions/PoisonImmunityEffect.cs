using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class PoisonImmunityEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30);
    protected Animation Animation { get; } = new()
    {
        TargetAnimation = 491,
        AnimationSpeed = 100
    };

    public List<string> ConflictingEffectNames { get; init; } = 
    [
    ];
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
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Poison immunity wears off.");
    }

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {

        if (target.Effects.Contains("poisonimmunity"))
        {
            target.Effects.Dispel("poisonimmunity");
        }
        
        if (target.Effects.Contains("poison"))
        {
            target.Effects.Dispel("poison");
        }

        return true;
    }
}