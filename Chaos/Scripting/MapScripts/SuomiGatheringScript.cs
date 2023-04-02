using Chaos.Containers;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class SuomiGatheringScript : MapScriptBase
{
    private readonly IReactorTileFactory ReactorTileFactory;
    
    public SuomiGatheringScript(MapInstance subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;

        var cherryrectangle = new Rectangle(
            27,
            69,
            16,
            21);

        var graperectangle = new Rectangle(
            76,
            38,
            13,
            19);
        
        var cherrypoints = new HashSet<Point>();
        var count = cherryrectangle.Area / 3;
        var count2 = graperectangle.Area / 3;
        var grapepoints = new HashSet<Point>();

        for (var i = 0; i < count; i++)
        {
            var cherrypoint = cherryrectangle.GetRandomPoint();
            cherrypoints.Add(cherrypoint);
        }

        for (var i = 0; i < count2; i++)
        {
            var grapepoint = graperectangle.GetRandomPoint();
            grapepoints.Add(grapepoint);
        }

        foreach (var cherrypoint in cherrypoints)
        {
            var cherry = ReactorTileFactory.Create(
                "cherry",
                Subject,
                cherrypoint);

            Subject.SimpleAdd(cherry);
        }
        
        foreach (var grapepoint in grapepoints)
        {
            var grape = ReactorTileFactory.Create(
                "grape",
                Subject,
                grapepoint);

            Subject.SimpleAdd(grape);
        }

    }
    
    
}