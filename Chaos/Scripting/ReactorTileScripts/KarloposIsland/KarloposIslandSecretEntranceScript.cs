using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class KarloposIslandSecretEntranceScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; }
    #endregion

    /// <inheritdoc />
    public KarloposIslandSecretEntranceScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        var aisling = source as Aisling;

        if (source.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
        {
            aisling?.SendOrangeBarMessage($"You must be at least level {targetMap.MinimumLevel} to enter this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        if (source.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
        {
            aisling?.SendOrangeBarMessage($"You must be at most level {targetMap.MaximumLevel} to enter this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        if (!aisling.Inventory.HasCount("red pearl", 1))
        {
            aisling?.SendOrangeBarMessage("You are missing the Red Pearl.");
            return;
        }

        if (aisling is not null
            && aisling.Trackers.Enums.TryGetValue<QueenOctopusQuest>(out var stage)
            && (stage == QueenOctopusQuest.Pendant))
            
            source.TraverseMap(targetMap, Destination);
        else
            aisling?.SendOrangeBarMessage("You see something frightening ahead, best not to disturb it.");
        
    }
}