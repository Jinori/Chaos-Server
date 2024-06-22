using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class MountEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(999);
    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 6,
        AnimationSpeed = 100
    };
    public List<string> ConflictingEffectNames { get; init; } =
    [
        "Mount",
        "Werewolf"
    ];
    public override byte Icon => 92;
    public override string Name => "mount";
    protected byte? Sound => 115;

    public override void OnTerminated()
    {
        if (AislingSubject != null)
        {
            AislingSubject.Sprite = 0;
            AislingSubject.Refresh(true);

            if (!AislingSubject.Trackers.TimedEvents.HasActiveEvent("mount", out _))
                AislingSubject.Trackers.TimedEvents.AddEvent("mount", TimeSpan.FromSeconds(5), true);
        }
    }
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}