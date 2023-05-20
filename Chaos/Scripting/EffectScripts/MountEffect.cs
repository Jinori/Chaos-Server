using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class MountEffect : NonOverwritableEffectBase
{
    public override byte Icon => 92;
    public override string Name => "mount";

    protected override TimeSpan Duration { get; } = TimeSpan.FromHours(999);

    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 6,
        AnimationSpeed = 100
    };
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "MountEffect",
    };
    protected override byte? Sound => 115;


    public override void OnDispelled()
    {
        AislingSubject.Sprite = 0;
        AislingSubject.Refresh(true);
    }

    public override void OnTerminated()
    {
        AislingSubject?.Trackers.TimedEvents.AddEvent("mount", TimeSpan.FromSeconds(5), true);

        return;
    }
}