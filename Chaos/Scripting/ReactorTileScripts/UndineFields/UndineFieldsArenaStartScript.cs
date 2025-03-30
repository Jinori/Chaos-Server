using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.UndineFields;

public class UndineFieldsArenaStartScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public UndineFieldsArenaStartScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var currentMap = SimpleCache.Get<MapInstance>(source.MapInstance.InstanceId);

        if (source is not Aisling aisling)
            return;

        if (aisling.Group is null || aisling.Group.Any(x => !x.OnSameMapAs(aisling) || !x.WithinRange(aisling)))
        {
            // Send a message to the Aisling
            aisling.SendOrangeBarMessage("You must have a group nearby to enter Carnun's Arena.");

            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        var allMembersHaveQuestEnum = aisling.Group.All(
            member => (member.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage) && (stage == UndineFieldDungeon.StartedDungeon))
                      || (member.Trackers.Flags.TryGetFlag(out UndineFieldDungeonFlag flag)
                          && (flag == UndineFieldDungeonFlag.CompletedUF)));

        if (allMembersHaveQuestEnum)
            aisling.Trackers.Enums.Set(UndineFieldDungeon.EnteredArena);
        else
        {
            // Send a message to the Aisling
            aisling.SendOrangeBarMessage("Not going to work.");

            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }
}