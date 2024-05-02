using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine
{
    public class MysteriousArtifactSpawnScript : MapScriptBase
    {
        private readonly IReactorTileFactory ReactorTileFactory;

        public MysteriousArtifactSpawnScript(MapInstance subject, IReactorTileFactory reactorTileFactory)
            : base(subject)
        {
            ReactorTileFactory = reactorTileFactory;

            // Get the boundary rectangle of the map
            var mapBoundary = Subject.Template.Bounds;

            // Generate a random point within the map's boundary
            var randomPoint = GenerateRandomPointWithinRectangle(mapBoundary);

            // Create a reactor tile at the random point
            CreateReactorTile("mysteriousartifact", randomPoint);
        }

        private void CreateReactorTile(string reactorName, Point point)
        {
            var reactor = ReactorTileFactory.Create(reactorName, Subject, point);
            Subject.SimpleAdd(reactor);
        }

        private Point GenerateRandomPointWithinRectangle(IRectangle rectangle)
        {
            var random = new Random();

            Point randomPoint;
            bool isValidPoint;

            do
            {
                // Generate random X and Y coordinates within the rectangle
                var randomX = random.Next(rectangle.Left, rectangle.Right);
                var randomY = random.Next(rectangle.Top, rectangle.Bottom);

                randomPoint = new Point(randomX, randomY);

                // Check if the random point is not on a wall or blocking reactor
                isValidPoint = !Subject.IsWall(randomPoint) && !Subject.IsBlockingReactor(randomPoint);

            } while (!isValidPoint);

            return randomPoint;
        }

    }
}