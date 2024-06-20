using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.WerewolfQuest;

public class PietWerewolfWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public PietWerewolfWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var aisling = source as Aisling;
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        
        if (source.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant))
        {
            var targetMap2 = SimpleCache.Get<MapInstance>("piet_empty_room2");
            var point = new Point(11, 6);
            source.TraverseMap(targetMap2, point);
            return;
        }
        
        if (source.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
        {
            aisling?.SendOrangeBarMessage($"You must be at least level {targetMap.MinimumLevel} to enter this area.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);

            return;
        }

        if (source.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
        {
            aisling?.SendOrangeBarMessage($"You must be at most level {targetMap.MaximumLevel} to enter this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);

            return;
        }

        source.TraverseMap(targetMap, Destination);
    }
}