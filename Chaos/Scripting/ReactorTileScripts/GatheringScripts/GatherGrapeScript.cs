using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts;

public class GatherGrapeScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public GatherGrapeScript(ReactorTile subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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

        if (aisling.Trackers.Counters.CounterGreaterThanOrEqualTo("grape", 23))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("suomi_grape_farmer");
            var point = new Point(8, 5);

            aisling.SendOrangeBarMessage("The farmer is staring, you head inside.");
            aisling.TraverseMap(mapInstance, point);
            aisling.Trackers.Counters.Remove("grape", out _);
        }

        var grape = ItemFactory.Create("Grape");
        var grapeCount = Random.Shared.Next(1, 4);

        grape.Count = grapeCount;

        if (!aisling.TryGiveItem(grape))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("suomi_grape_farmer");
            var point = new Point(8, 5);

            aisling.SendOrangeBarMessage("The farmer is staring, you head inside.");
            aisling.TraverseMap(mapInstance, point);
            aisling.Trackers.Counters.Remove("grape", out _);

            return;
        }

        var animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 20
        };

        aisling.Animate(animation);
        aisling.SendOrangeBarMessage("You gathered some grapes!");
        aisling.Trackers.Counters.AddOrIncrement("grape");

        return;
    }
}