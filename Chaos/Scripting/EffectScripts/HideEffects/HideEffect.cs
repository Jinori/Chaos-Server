using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public sealed class HideEffect : EffectBase
{
    /// <inheritdoc />\
    
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);
    private readonly IIntervalTimer HideTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    private bool HasRecentlyHidden = true;

    private Animation HideAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 994
    };
    
    /// <inheritdoc />
    public override byte Icon => 8;
    /// <inheritdoc />
    public override string Name => "Hide";

    /// <inheritdoc />
    public override void OnApplied()
    {
        Subject.Trackers.Counters.Set("HideSec", 0);
        Subject.SetVisibility(VisibilityType.Hidden);
    }

    /// <inheritdoc />
    public override void OnTerminated() => Subject.SetVisibility(VisibilityType.Normal);

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Visibility is not VisibilityType.Normal)
        {
            AislingSubject?.SendOrangeBarMessage("You are already hidden.");

            return false;
        }

        return base.ShouldApply(source, target);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        if (HasRecentlyHidden)
        {
            HideTimer.Update(delta);

            if (HideTimer.IntervalElapsed)
            {
                Subject.Trackers.Counters.AddOrIncrement("HideSec");

                if (Subject.Trackers.Counters.CounterGreaterThanOrEqualTo("HideSec", 5))
                    HasRecentlyHidden = false;
            }   
        }
        
        base.Update(delta);
    }
}