using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class LimboWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public LimboWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        var aisling = source as Aisling;

        if (!aisling!.UserStatSheet.Master)
            return;

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

        aisling.IsDead = false;
        aisling.StatSheet.SetHealthPct(25);
        aisling.StatSheet.SetManaPct(25);
        aisling.Client.SendAttributes(StatUpdateType.Vitality);
        aisling.SendActiveMessage("Welcome to Limbo. Are you dead or alive?");
        aisling.Refresh();
        aisling.Display();

        aisling.TraverseMap(targetMap, Destination);
    }
}