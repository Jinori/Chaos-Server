using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class MeteorChaosScript : MapScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript;
    private IIntervalTimer MeteorChaosTimer { get; }
    private int CurrentAnimationIndex { get; set; }
    private readonly Animation[] Animations = {
        new()
            { AnimationSpeed = 100, TargetAnimation = 147 },
        new()
            { AnimationSpeed = 100, TargetAnimation = 148 },
        new()
            { AnimationSpeed = 100, TargetAnimation = 149 }
    };

    /// <inheritdoc />
    public MeteorChaosScript(MapInstance subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        MeteorChaosTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        MeteorChaosTimer.Update(delta);

        if (!MeteorChaosTimer.IntervalElapsed)
            return;

        // Get all non-wall points on the map
        // You might want to switch this to the loop-based approach as previously suggested
        var nonWallPoints = Enumerable.Range(0, Subject.Template.Width)
                                      .SelectMany(
                                          x => Enumerable.Range(0, Subject.Template.Height)
                                                         .Where(y => !Subject.IsWall(new Point(x, y)))
                                                         .Select(y => new Point(x, y)))
                                      .ToList();

        if (nonWallPoints.Count > 0)
        {
            // Select a random non-wall point
            var targetPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

            // Use the current animation
            Subject.ShowAnimation(Animations[CurrentAnimationIndex].GetPointAnimation(targetPoint));

            // Check if a player is standing on the point and apply damage
            var targetPlayer = Subject.GetEntitiesAtPoint<Aisling>(targetPoint).FirstOrDefault();

            if (targetPlayer != null)
            {
                var damage = (int)(targetPlayer.StatSheet.EffectiveMaximumHp * 0.18); // 18% of max HP

                ApplyDamageScript.ApplyDamage(
                    targetPlayer,
                    targetPlayer,
                    this,
                    damage);
            }

            // Cycle to the next animation
            CurrentAnimationIndex = (CurrentAnimationIndex + 1) % Animations.Length;
        }
    }
}