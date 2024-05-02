using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness;

public class SpawnSilkWormReactorScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public SpawnSilkWormReactorScript(ReactorTile subject, IItemFactory itemFactory, ISimpleCache simpleCache,
        IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var monsterpoint = source.DirectionalOffset(source.Direction);
        var worm = MonsterFactory.Create("dirty_silk_worm", Subject.MapInstance, monsterpoint);

        if (!IntegerRandomizer.RollChance(40))
            return;

        Subject.MapInstance.AddEntity(worm, monsterpoint);
        aisling.SendOrangeBarMessage("You pull a Dirty Silk Worm from the ground.");
        Subject.MapInstance.RemoveEntity(Subject);
    }
}