using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events;

public class MoveInDirectionReindeerScript : MonsterScriptBase
{
    private readonly IIntervalTimer RandomWalkInterval;
    private readonly List<Point> ExitLinePoints;

    public MoveInDirectionReindeerScript(Monster subject)
        : base(subject)
    {
        RandomWalkInterval = new RandomizedIntervalTimer(
            TimeSpan.FromMilliseconds(300),
            70,
            RandomizationType.Negative
        );

        // Generate the exit line points (25x, 8y to 25x, 20y)
        ExitLinePoints = GenerateExitLinePoints();
    }

    public override void Update(TimeSpan delta)
    {
        // Update walk interval
        RandomWalkInterval.Update(delta);

        if (!RandomWalkInterval.IntervalElapsed)
            return;

        // Check if the reindeer is at any exit point
        if (ExitLinePoints.Contains(new Point(Subject.X, Subject.Y)))
        {
            // Remove the reindeer from the map
            Map.RemoveEntity(Subject);
            return;
        }

        var targetDirection = Subject.Direction; // Reindeer's current direction
        var nextPosition = Subject.DirectionalOffset(targetDirection);

        // Check if there's a wall, blocking reactor, or any other obstacle
        if (Subject.MapInstance.IsWall(nextPosition) || Subject.MapInstance.IsBlockingReactor(nextPosition))
            return;

        // Check if an Aisling is directly in front
        var aislingInFront = Subject.MapInstance.GetEntitiesAtPoints<Aisling>(nextPosition).TopOrDefault();

        if (aislingInFront != null)
        {
            HandleTrample(aislingInFront);
            return;
        }

        // Move the reindeer forward if no obstacles or Aislings
        Subject.Walk(Direction.Right);
    }

    private void HandleTrample(Aisling aisling)
    {
        // Warp the Aisling to the designated point
        aisling.WarpTo(new Point(4, 11));

        // Send feedback message to the Aisling
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The crazed reindeer trampled you!");

        // Optional: Add additional logic (e.g., stagger the reindeer, cooldown mechanics)
    }

    private List<Point> GenerateExitLinePoints()
    {
        // Generate all points in the line (25x, 8y to 25x, 20y)
        var points = new List<Point>();
        for (var y = 8; y <= 20; y++)
        {
            points.Add(new Point(25, y));
        }
        return points;
    }
}
