using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
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
        AnimationSpeed = 100,
        TargetAnimation = 214
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
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(9));
        PreAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(7));

        TimePassedSinceTileAnimation = 0;
        TimePassedSincePreAnimationStart = 0;
        TimePassedSinceMainAnimationStart = 0;

        GenerateSafeRectangle();
    }

    private bool ContainsWallorReactor(IRectangle rectangle)
    {
        foreach (var point in rectangle.GetPoints())
            if (Map.IsWall(point) || Map.IsBlockingReactor(point))

                return true;

        return false;
    }

    private void DealDamageToCreaturesAtPoint(IPoint point)
    {
        var creaturesToDamage = Map.GetEntitiesAtPoints<Creature>(point);

        foreach (var creature in creaturesToDamage.ToList())
            if (creature.Id != Subject.Id)
            {
                var damage = (int)(creature.StatSheet.EffectiveMaximumHp * 0.30);

                ApplyDamageScript.ApplyDamage(
                    Subject,
                    creature,
                    this,
                    damage);
            }
    }

    private void GenerateSafeRectangle()
    {
        const int MAX_ATTEMPTS = 100;
        var attempts = 0;
        const int WIDTH = 3; // Set the width of the rectangle
        const int HEIGHT = 3; // Set the height of the rectangle

        // Pass the map's boundary rectangle to GetRandomWalkablePoint
        var centerPoint = GetRandomWalkablePoint(Map.Template.Bounds);

        do
        {
            SafeRectangle = new Rectangle(centerPoint, WIDTH, HEIGHT);
            attempts++;

            if (attempts >= MAX_ATTEMPTS)
                break;
        } while (ContainsWallorReactor(SafeRectangle));

        // Add individual points within the SafeRectangle to SafePoints
        foreach (var point in SafeRectangle.GetPoints())
            SafePoints.Add(point);
    }

    public Point GetRandomWalkablePoint(IRectangle rect)
    {
        const int MAX_ATTEMPTS = 100; // Set a maximum number of attempts to avoid infinite loops
        var attemptCount = 0;

        while (attemptCount < MAX_ATTEMPTS)
        {
            var randomX = rect.Left + Random.Shared.Next(rect.Width);
            var randomY = rect.Top + Random.Shared.Next(rect.Height);
            var randomPoint = new Point(randomX, randomY);

            if (!Map.IsWall(randomPoint) && !Map.IsBlockingReactor(randomPoint))
                return randomPoint; // Found a walkable point

            attemptCount++;
        }

        // If no walkable point is found after the maximum attempts, return a default point.
        // You can handle this case based on your specific needs, such as throwing an exception or returning null.
        // For example:
        throw new Exception("Could not find a walkable point within the given bounds.");
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

        if (!Map.GetEntities<Aisling>()
                .Any())
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
            } else
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