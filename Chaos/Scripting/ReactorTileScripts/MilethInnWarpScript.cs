using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class MilethInnWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public MilethInnWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        var aisling = source as Aisling;

        var hasStage = source.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage stage);

        if (!hasStage || (hasStage == (stage == RionaTutorialQuestStage.StartedRatQuest)))
        {
            aisling?.SendOrangeBarMessage("Speak to Riona and complete the rat quest to leave.");

            return;
        }

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

        source.TraverseMap(targetMap, Destination);
    }
}