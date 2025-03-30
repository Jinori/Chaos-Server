using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.GMEffects;

public class FollowEffect : ContinuousAnimationEffectBase
{
    /// <summary>
    ///     The duration is set to a very long time since this effect is meant to persist.
    /// </summary>
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromDays(999); // Extended duration for following

    private Creature SourceOfEffect { get; set; } = null!; // The creature receiving the effect
    public Aisling TargetPlayer { get; set; } = null!; // The player being followed

    /// <summary>
    ///     The animation that will be used for the effect.
    /// </summary>
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 1,
        TargetAnimation = 210
    };

    /// <summary>
    ///     Timer for the animation interval.
    /// </summary>
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromDays(99));

    /// <summary>
    ///     Timer for effect's periodic behavior.
    /// </summary>
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);

    /// <inheritdoc />
    public override byte Icon => 1;

    /// <inheritdoc />
    public override string Name => "Follow";

    /// <summary>
    ///     When the effect is applied, it initializes the target player. The effect is applied to the source (follower), not
    ///     the target.
    /// </summary>
    public override void OnApplied()
    {
        if ((SourceOfEffect != null) && (AislingSubject != null))
        {
            TargetPlayer = AislingSubject;
            AislingSubject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You are now following {TargetPlayer.Name}.");
        }
    }

    /// <summary>
    ///     Periodically checks and moves the source to follow the target player.
    /// </summary>
    protected override void OnIntervalElapsed()
    {
        if ((SourceOfEffect == null) || (TargetPlayer == null))
        {
            AislingSubject?.Effects.Terminate("follow");

            return;
        }

        // Check if the target player has swapped maps
        if (TargetPlayer.MapInstance != SourceOfEffect.MapInstance)

            // Teleport the source to the target if on a different map
            SourceOfEffect.TraverseMap(TargetPlayer.MapInstance, TargetPlayer, true);
        else
        {
            // Move the source to the target's current position on the same map
            if ((SourceOfEffect.X != TargetPlayer.X) || (SourceOfEffect.Y != TargetPlayer.Y))
            {
                if (TargetPlayer.ActiveObject.TryGet<WorldMap>() != null)
                    return;

                SourceOfEffect.WarpTo(TargetPlayer);
            }
        }
    }

    public override void OnReApplied()
    {
        if ((SourceOfEffect != null) && SourceOfEffect.Effects.Contains("follow"))
            SourceOfEffect.Effects.Terminate("follow");
    }

    /// <summary>
    ///     When the effect is terminated, notify the source player.
    /// </summary>
    public override void OnTerminated() { }

    /// <summary>
    ///     Determine whether the effect should be applied to the source.
    /// </summary>
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source; // Apply the effect to the source (the follower)

        if (SourceOfEffect.Effects.Contains("follow"))
        {
            SourceOfEffect.Effects.Terminate("follow");

            return false;
        }

        return base.ShouldApply(source, target);
    }
}