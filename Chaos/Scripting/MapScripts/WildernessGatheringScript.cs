using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class WildernessGatheringScript : MapScriptBase
{
    private readonly IReactorTileFactory ReactorTileFactory;

    public WildernessGatheringScript(MapInstance subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;

        var cottonrectangle = new Rectangle(
            5,
            152,
            13,
            10);

        var wildernesscherryrectangle = new Rectangle(
            33,
            81,
            14,
            11);

        var wildernessicerectangle = new Rectangle(
            200,
            0,
            53,
            58);

        var cottonpoints = new HashSet<Point>();
        var wildernesscherrypoints = new HashSet<Point>();
        var icepoints = new HashSet<Point>();
        var count1 = cottonrectangle.Area / 6;
        var count2 = wildernesscherryrectangle.Area / 6;
        var count3 = wildernessicerectangle.Area / 90;

        for (var i = 0; i < count1; i++)
        {
            var cottonpoint = cottonrectangle.GetRandomPoint();
            cottonpoints.Add(cottonpoint);
        }

        for (var i = 0; i < count2; i++)
        {
            var wildernesscherrypoint = wildernesscherryrectangle.GetRandomPoint();
            wildernesscherrypoints.Add(wildernesscherrypoint);
        }

        for (var i = 0; i < count3; i++)
        {
            var wildernessicepoints = wildernessicerectangle.GetRandomPoint();
            icepoints.Add(wildernessicepoints);
        }

        foreach (var cottonpoint in cottonpoints)
        {
            var cotton = ReactorTileFactory.Create("cotton", Subject, cottonpoint);

            Subject.SimpleAdd(cotton);
        }

        foreach (var wildernesscherrypoint in wildernesscherrypoints)
        {
            var wildernesscherry = ReactorTileFactory.Create("wildernesscherry", Subject, wildernesscherrypoint);

            Subject.SimpleAdd(wildernesscherry);
        }

        foreach (var wildernessicepoints in icepoints)
        {
            var ice = ReactorTileFactory.Create("ice", Subject, wildernessicepoints);

            Subject.SimpleAdd(ice);
        }
    }
}