using Chaos.Containers;
using Chaos.Data;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts;

public class GatherCherryScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public GatherCherryScript(ReactorTile subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {

        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("cherry", 23))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("suomi_cherry_farmer");
            var point = new Point(7, 5);

            aisling.SendOrangeBarMessage("The farmer waves, you head inside.");
            aisling.TraverseMap(mapInstance, point);
            aisling.Trackers.Counters.Remove("cherry", out _);
        }

        var cherry = ItemFactory.Create("Cherry");
        var cherryCount = Random.Shared.Next(1, 4);

        cherry.Count = cherryCount;

        if (!aisling.TryGiveItem(cherry))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("suomi_cherry_farmer");
            var point = new Point(7, 5);

            aisling.SendOrangeBarMessage("The farmer waves, you head inside.");
            aisling.TraverseMap(mapInstance, point);
            aisling.Trackers.Counters.Remove("cherry", out _);

            return;
        }

        var animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 20
        };

        aisling.Animate(animation);
        aisling.SendOrangeBarMessage("You gathered some cherries!");
        aisling.Trackers.Counters.AddOrIncrement("cherry");

        return;
    }
}