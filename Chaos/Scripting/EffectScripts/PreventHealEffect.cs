using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class PreventHealEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(9);
    public List<string> ConflictingEffectNames { get; init; } =
    [
        "Prevent Heal"
    ];
    public override byte Icon => 66;
    public override string Name => "Prevent Heal";
    public override void OnTerminated()
    {
        AislingSubject?.SendOrangeBarMessage("You don't resist heals anymore.");
    }
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}