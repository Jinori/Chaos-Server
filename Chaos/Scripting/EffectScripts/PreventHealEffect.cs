using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class PreventHealEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } = ["Prevent Heal"];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(9);
    public override byte Icon => 66;
    public override string Name => "Prevent Heal";

    public override void OnTerminated()
    {
        AislingSubject?.SendOrangeBarMessage("You don't resist heals anymore.");
        AislingSubject?.ShowHealth();
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}