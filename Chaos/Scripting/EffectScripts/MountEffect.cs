using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class MountEffect : NonOverwritableEffectBase
{
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 6,
        AnimationSpeed = 100
    };
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "MountEffect"
    };
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(999);
    public override byte Icon => 92;
    public override string Name => "mount";
    protected override byte? Sound => 115;

    public override void OnTerminated()
    {
        if (AislingSubject != null)
        {
            AislingSubject.Sprite = 0;
            AislingSubject.Refresh(true);

            if (!AislingSubject.Trackers.TimedEvents.HasActiveEvent("mount", out var mountEvent))
                AislingSubject.Trackers.TimedEvents.AddEvent("mount", TimeSpan.FromSeconds(5), true);
        }
    }
}