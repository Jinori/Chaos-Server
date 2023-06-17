using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts
{
    public class SuomiGatheringScript : MapScriptBase
    {
        private readonly IReactorTileFactory ReactorTileFactory;

        public SuomiGatheringScript(MapInstance subject, IReactorTileFactory reactorTileFactory)
            : base(subject)
        {
            ReactorTileFactory = reactorTileFactory;

            var cherryRectangle = new Rectangle(27, 69, 16, 21);
            var grapeRectangle = new Rectangle(76, 38, 13, 19);

            var cherryPoints = GenerateRandomPoints(cherryRectangle, cherryRectangle.Area / 3);
            var grapePoints = GenerateRandomPoints(grapeRectangle, grapeRectangle.Area / 3);

            CreateReactorTiles("cherry", cherryPoints);
            CreateReactorTiles("grape", grapePoints);
        }

        private HashSet<Point> GenerateRandomPoints(Rectangle rectangle, int count)
        {
            var points = new HashSet<Point>();
            for (var i = 0; i < count; i++)
            {
                var point = rectangle.GetRandomPoint();
                points.Add(point);
            }
            return points;
        }

        private void CreateReactorTiles(string reactorName, HashSet<Point> points)
        {
            foreach (var point in points)
            {
                var reactor = ReactorTileFactory.Create(reactorName, Subject, point);
                Subject.SimpleAdd(reactor);
            }
        }
    }
}