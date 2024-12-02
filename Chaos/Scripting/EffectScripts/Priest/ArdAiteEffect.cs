using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class ArdAiteEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(20);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Blessing",
            "Torment",
            "Ard Naomh Aite",
            "Mor naomh Aite",
            "Naomh Aite",
            "Beag Naomh Aite"
        ];

    private Creature SourceOfEffect { get; set; } = null!;

    private Animation? Animation { get; } = new()
    {
        TargetAnimation = 414,
        AnimationSpeed = 100
    };

    public override byte Icon => 9;
    public override string Name => "Ard Naomh Aite";

    protected byte? Sound => 31;

    public override void OnApplied()
    {
        base.OnApplied();

        Subject.Animate(Animation!);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();
        
        return execution is not null;
    }
}