using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Secret;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Namotion.Reflection;

namespace Chaos.Scripting.MapScripts.Secret;

public class CowMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1;

    private readonly IReactorTileFactory ReactorTileFactory;


    public CowMapScript(MapInstance subject, IReactorTileFactory reactorTileFactory, ISimpleCache simpleCache)
        : base(subject)
        => ReactorTileFactory = reactorTileFactory;

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        aisling.SendOrangeBarMessage("Welcome to the Cow Level, beware of the king.");
        OpenEscapePortal();
    }

    private void OpenEscapePortal()
    {

        // Check if there's already an escape portal open
        if (Subject.GetEntities<ReactorTile>()
                   .Any(x => x.Script.Is<milethCowPortal>()))
            return;

        var rectangle = new Rectangle(
            41,
            65,
            5,
            5);
        var point = rectangle.GetRandomPoint();

        var reactortile = ReactorTileFactory.Create("milethCowPortal", Subject, Point.From(point));
        
        Subject.SimpleAdd(reactortile);

    }
}