using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.EventBuffs;

public class HotChocolateEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } = ["Inner Fire"];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 46,
        AnimationSpeed = 100
    };

    public override byte Icon => 65;
    public override string Name => "Hot Chocolate";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Yum. (Health and Mana regeneration increased.)");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your regeneration has returned to normal.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}