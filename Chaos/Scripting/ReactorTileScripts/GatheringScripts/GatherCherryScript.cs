using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts;

public class GatherCherryScript : ReactorTileScriptBase
{
    private readonly IItemFactory _itemFactory;
    private readonly ISimpleCache _simpleCache;

    /// <inheritdoc />
    public GatherCherryScript(ReactorTile subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        _itemFactory = itemFactory;
        _simpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("cherry", 23))
        {
            var mapInstance = _simpleCache.Get<MapInstance>("suomi_cherry_farmer");
            var point = new Point(7, 5);
            aisling.SendOrangeBarMessage("The farmer waves, you head inside.");
            aisling.TraverseMap(mapInstance, point);
            aisling.Trackers.Counters.Remove("cherry", out _);
        }

        var cherry = _itemFactory.Create("Cherry");
        var cherryCount = Random.Shared.Next(1, 4);
        cherry.Count = cherryCount;

        if (!aisling.TryGiveItem(ref cherry))
        {
            var mapInstance = _simpleCache.Get<MapInstance>("suomi_cherry_farmer");
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
    }
}