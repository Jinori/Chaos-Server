using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class CelebrationEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 300
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(2000));

    /// <inheritdoc />
    public override byte Icon => 82;

    /// <inheritdoc />
    public override string Name => "Celebration";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var animations = new[]
        {
            new Animation
            {
                AnimationSpeed = 200,
                TargetAnimation = 294
            },
            new Animation
            {
                AnimationSpeed = 200,
                TargetAnimation = 289
            },
            new Animation
            {
                AnimationSpeed = 200,
                TargetAnimation = 304
            },
            new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 359
            }
        };

        // Pick one at random
        var chosenAnimation = animations.PickRandom();

        var rectangle = new Rectangle(
            Subject.X - 2,
            Subject.Y - 2,
            5,
            5);

        var pointToAnimate = rectangle.GetRandomPoint();

        if (!Subject.MapInstance.IsWithinMap(pointToAnimate))
            Subject.Animate(chosenAnimation);
        else
            Subject.MapInstance.ShowAnimation(chosenAnimation.GetPointAnimation(pointToAnimate));
    }
}