using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class GMKnowledgeEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } = ["GMKnowledgeEffect"];
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(60);

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 157,
        AnimationSpeed = 100
    };

    public override byte Icon => 10;
    public override string Name => "GM Knowledge";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You gain %25 increased experience.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your experience gain has returned to normal.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You cast {Name} on {Subject.Name}.");

        return execution is not null;
    }
}