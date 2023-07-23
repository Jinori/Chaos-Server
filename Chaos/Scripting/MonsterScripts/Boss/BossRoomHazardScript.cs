using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossRoomHazardScript : MonsterScriptBase
{
    private const int MINIMUM_RECT_SIZE = 3;
    private const int MAXIMUM_RECT_SIZE = 5;
    private readonly List<IPoint> AnimatedPoints = new();
    private readonly List<IPoint> SafePoints = new();
    private bool IsAnimating;
    private bool IsPreAnimating;
    private Rectangle SafeRectangle = null!; // Store the safeRectangle here
    private double TimePassedSinceMainAnimationStart;
    private double TimePassedSincePreAnimationStart;
    private double TimePassedSinceTileAnimation;

    private Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 13
    };
    private IIntervalTimer AnimationTimer { get; }
    private IApplyDamageScript ApplyDamageScript { get; }

    private Animation PreAnimation { get; } = new()
    {
        AnimationSpeed = 100, // Adjust the speed as needed
        TargetAnimation = 214 // Use a different animation ID for the pre-animation
    };
    private IIntervalTimer PreAnimationTimer { get; }

    private IIntervalTimer PunishTimer { get; }

    public BossRoomHazardScript(Monster subject)
        : base(subject)
    {
        PunishTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(20),
            30,
            RandomizationType.Positive,
            false);

        ApplyDamageScript = ApplyAttackDamageScript.Create();
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(9));
        PreAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(7)); // Set pre-animation duration to 7 seconds

        TimePassedSinceTileAnimation = 0;
        TimePassedSincePreAnimationStart = 0;
        TimePassedSinceMainAnimationStart = 0;

        GenerateSafeRectangle();
    }

    private void DealDamageToCreaturesAtPoint(IPoint point)
    {
        var creaturesToDamage = Map.GetEntitiesAtPoint<Creature>(point);

        foreach (var creature in creaturesToDamage)
            if (creature.Id != Subject.Id)
            {
                var damage = (int)(creature.StatSheet.EffectiveMaximumHp * 0.15);

                ApplyDamageScript.ApplyDamage(
                    Subject,
                    creature,
                    this,
                    damage,
                    Element.Fire);
            }
    }

    public static Rectangle GenerateAislingSafeRectangle(IPoint centerPoint)
    {
        var width = Random.Shared.Next(MINIMUM_RECT_SIZE, MAXIMUM_RECT_SIZE + 1);
        var height = Random.Shared.Next(MINIMUM_RECT_SIZE, MAXIMUM_RECT_SIZE + 1);
        var rect = new Rectangle(centerPoint, width, height);

        return rect;
    }

    private void GenerateSafeRectangle()
    {
        var centerPoint = Map.GetRandomWalkablePoint();
        SafeRectangle = GenerateAislingSafeRectangle(centerPoint);

        // Add individual points within the SafeRectangle to SafePoints
        foreach (var point in SafeRectangle.GetPoints())
            SafePoints.Add(point);
    }

    private void ResetTimersAndPoints()
    {
        PunishTimer.Reset();
        AnimationTimer.Reset();
        PreAnimationTimer.Reset();
        TimePassedSinceTileAnimation = 0;
        TimePassedSincePreAnimationStart = 0;
        TimePassedSinceMainAnimationStart = 0;
        SafePoints.Clear();
        GenerateSafeRectangle();
    }

    public override void Update(TimeSpan delta)
    {
        PunishTimer.Update(delta);
        PreAnimationTimer.Update(delta);
        AnimationTimer.Update(delta);

        if (!Map.GetEntities<Aisling>().Any())
            return;

        if (PunishTimer.IntervalElapsed)
        {
            IsAnimating = true;
            IsPreAnimating = true; // Start the pre-animation
            AnimatedPoints.Clear();
            TimePassedSinceTileAnimation = 0;
            TimePassedSincePreAnimationStart = 0;
            TimePassedSinceMainAnimationStart = 0;
        }

        if (IsAnimating)
        {
            if (IsPreAnimating)
            {
                // Store the value before incrementing
                var previousPreAnimationSecond = (int)Math.Floor(TimePassedSincePreAnimationStart);

                TimePassedSincePreAnimationStart += delta.TotalSeconds; // Increment the pre-animation timer

                // Compare with the incremented value
                var currentPreAnimationSecond = (int)Math.Floor(TimePassedSincePreAnimationStart);

                // Show pre-animation for SafePoints only if it's a new second
                if (previousPreAnimationSecond != currentPreAnimationSecond)
                    foreach (var point in SafePoints)
                        Map.ShowAnimation(PreAnimation.GetPointAnimation(point));

                if (PreAnimationTimer.IntervalElapsed)
                {
                    // PreAnimationTimer elapsed, stop pre-animation
                    IsPreAnimating = false;
                    TimePassedSinceTileAnimation = 0; // Reset the tile animation timer after the pre-animation ends
                }
            }
            else
            {
                // Start the main animation
                var currentSecond = (int)Math.Floor(TimePassedSinceMainAnimationStart);

                for (var x = 0; x < Map.Template.Width; x++)
                {
                    for (var y = 0; y < Map.Template.Height; y++)
                    {
                        var point = new Point(x, y);

                        if (Map.IsWithinMap(point) && !Map.IsWall(point) && !SafePoints.Contains(point) && !AnimatedPoints.Contains(point))
                            if ((int)Math.Floor(TimePassedSinceMainAnimationStart) == currentSecond)
                            {
                                Map.ShowAnimation(Animation.GetPointAnimation(point));
                                DealDamageToCreaturesAtPoint(point);
                                AnimatedPoints.Add(point);
                            }
                    }
                }

                TimePassedSinceMainAnimationStart += delta.TotalSeconds; // Increment the main animation timer

                TimePassedSinceTileAnimation += delta.TotalSeconds;

                if (TimePassedSinceTileAnimation >= 1)
                {
                    TimePassedSinceTileAnimation = 0;
                    AnimatedPoints.Clear();
                }

                if (AnimationTimer.IntervalElapsed)
                {
                    // AnimationTimer elapsed, stop the animation
                    IsAnimating = false;
                    ResetTimersAndPoints();
                }
            }
        }
    }
}