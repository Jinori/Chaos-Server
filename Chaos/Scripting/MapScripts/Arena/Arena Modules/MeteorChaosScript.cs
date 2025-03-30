using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

/// <summary>
///     Script to handle the chaos caused by meteors in the arena.
/// </summary>
public sealed class MeteorChaosScript : MapScriptBase
{
    private readonly Animation[] _animations = new[]
    {
        new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 147
        },
        new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 148
        },
        new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 149
        }
    };

    private readonly IApplyDamageScript _applyDamageScript;
    private readonly IIntervalTimer _meteorChaosTimer;
    private int _currentAnimationIndex;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MeteorChaosScript" /> class.
    /// </summary>
    /// <param name="subject">
    ///     The map instance subject to apply the script.
    /// </param>
    public MeteorChaosScript(MapInstance subject)
        : base(subject)
    {
        _applyDamageScript = ApplyAttackDamageScript.Create();
        _meteorChaosTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    }

    /// <summary>
    ///     Applies damage to entities at a specific point.
    /// </summary>
    /// <param name="targetPoint">
    ///     The target point to apply damage.
    /// </param>
    private void ApplyDamageToEntitiesAtPoint(Point targetPoint)
    {
        var targetPlayer = Subject.GetEntitiesAtPoints<Aisling>(targetPoint)
                                  .FirstOrDefault();

        if (targetPlayer == null)
            return;

        var damage = (int)(targetPlayer.StatSheet.EffectiveMaximumHp * 0.18);

        _applyDamageScript.ApplyDamage(
            targetPlayer,
            targetPlayer,
            this,
            damage);
    }

    /// <summary>
    ///     Gets all non-wall points on the map.
    /// </summary>
    /// <returns>
    ///     An enumerable of non-wall points.
    /// </returns>
    private IEnumerable<Point> GetNonWallPoints()
        => Enumerable.Range(0, Subject.Template.Width)
                     .SelectMany(
                         x => Enumerable.Range(0, Subject.Template.Height)
                                        .Where(y => !Subject.IsWall(new Point(x, y)))
                                        .Select(y => new Point(x, y)));

    /// <summary>
    ///     Applies the meteor chaos effect by showing animation and applying damage.
    /// </summary>
    private void TryApplyMeteorChaos()
    {
        var nonWallPoints = GetNonWallPoints()
            .ToList();

        if (nonWallPoints.Count == 0)
            return;

        var targetPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

        Subject.ShowAnimation(
            _animations[_currentAnimationIndex]
                .GetPointAnimation(targetPoint));

        ApplyDamageToEntitiesAtPoint(targetPoint);
        UpdateAnimationIndex();
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        _meteorChaosTimer.Update(delta);

        if (!_meteorChaosTimer.IntervalElapsed)
            return;

        TryApplyMeteorChaos();
    }

    /// <summary>
    ///     Updates the animation index to cycle through animations.
    /// </summary>
    private void UpdateAnimationIndex() => _currentAnimationIndex = (_currentAnimationIndex + 1) % _animations.Length;
}