using Chaos.Models.World;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class TeleportScript : MapScriptBase
{
    private IIntervalTimer TeleportTimer { get; }
    private IIntervalTimer AnimationTimer { get; }
    private Point? AnimatedTile { get; set; }
    
    private Animation PortAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 96
    };

    /// <inheritdoc />
    public TeleportScript(MapInstance subject)
        : base(subject)
    {
        TeleportTimer = new IntervalTimer(TimeSpan.FromSeconds(7));
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        TeleportTimer.Update(delta);

        if (TeleportTimer.IntervalElapsed)
        {
            // Get all non-wall points on the map
            var nonWallPoints = Enumerable.Range(0, Subject.Template.Width)
                                          .SelectMany(x => Enumerable.Range(0, Subject.Template.Height)
                                                                     .Where(y => !Subject.IsWall(new Point(x, y)))
                                                                     .Select(y => new Point(x, y))).ToList();

            if (nonWallPoints.Count > 0)
            {
                // Select a random non-wall point for animation
                AnimatedTile = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

                Subject.ShowAnimation(PortAnimation.GetPointAnimation(AnimatedTile));
                AnimationTimer.Reset();
            }
        }

        AnimationTimer.Update(delta);

        if (AnimationTimer.IntervalElapsed && (AnimatedTile != null))
        {
            var nonWallPoints = Enumerable.Range(0, Subject.Template.Width)
                                          .SelectMany(x => Enumerable.Range(0, Subject.Template.Height)
                                                                     .Where(y => !Subject.IsWall(new Point(x, y)) && new Point(x, y) != AnimatedTile)
                                                                     .Select(y => new Point(x, y))).ToList();

            if (nonWallPoints.Count > 0)
            {
                // Select a random non-wall point to teleport the player
                var targetPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

                // Identify all players on the animated tile
                var playersOnTile = Subject.GetEntitiesAtPoint<Aisling>(AnimatedTile);

                // Teleport each player to the random point
                foreach (var player in playersOnTile)
                {
                    player.WarpTo(targetPoint);
                }
            }

            AnimatedTile = null; // Reset the animated tile
        }
    }
}
