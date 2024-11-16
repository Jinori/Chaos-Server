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
    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public SpawnSilkWormReactorScript(
        ReactorTile subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        IMonsterFactory monsterFactory)
        : base(subject)
        => MonsterFactory = monsterFactory;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var direction = source.Direction;
        var monsterpoint = source.DirectionalOffset(direction);

        if (!Subject.MapInstance.IsWithinMap(monsterpoint))
            monsterpoint = new Point(source.X, source.Y);

        var worm = MonsterFactory.Create("dirty_silk_worm", Subject.MapInstance, monsterpoint);

        if (!IntegerRandomizer.RollChance(40))
            return;

        Subject.MapInstance.AddEntity(worm, monsterpoint);
        aisling.SendOrangeBarMessage("You pull a Dirty Silk Worm from the ground.");
        Subject.MapInstance.RemoveEntity(Subject);
    }
}