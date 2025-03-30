using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class ArmachdEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(8);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Blessing",
            "Armachd"
        ];

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 512,
        AnimationSpeed = 100
    };

    public override byte Icon => 0;
    public override string Name => "Armachd";

    protected byte? Sound => 122;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = 10
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -10
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }
}