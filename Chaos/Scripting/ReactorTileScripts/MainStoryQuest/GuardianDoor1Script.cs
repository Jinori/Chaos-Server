using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class GuardianDoor1Script : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public GuardianDoor1Script(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
    }

    #region ScriptVars

    protected Location Destination { get; init; } = null!;

    #endregion

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);

        switch (source.MapInstance.Name)
        {
            case "West Woodlands 6":
            {
                if (!source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1))
                {
                    var point = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
                    return;
                }
            }
                break;
            case "Astrid North":
            {
                if (!source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3))
                {
                    var point = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
                    return;
                }
            }
                break;

            case "Crypt 10":
            {
                if (!source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2))
                {
                    var point = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
                    return;
                }
            }
                break;
            case "Karlopos Island East":
            {
                if (!source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4))
                {
                    var point = source.DirectionalOffset(source.Direction.Reverse());
                    source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
                    return;
                }
            }
                break;
        }

        source.TraverseMap(targetMap, Destination);
    }
}