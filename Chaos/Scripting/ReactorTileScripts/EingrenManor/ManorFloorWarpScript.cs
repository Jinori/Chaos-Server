using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EingrenManor;

public class ManorFloorWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public ManorFloorWarpScript(ReactorTile subject, ISimpleCache simpleCache)
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

        // Check if the group is null or has only one member
        if (aisling?.Group is null || aisling.Group.Any(x => !x.OnSameMapAs(aisling) || !x.WithinRange(aisling)))
        {
            // Send a message to the Aisling
            aisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "Make sure you are grouped or your group is near you.");

            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }
        
        // Check if all members of the group have the quest enum and are within level range
        var allMembersHaveQuestFlag = aisling.Group.All(member =>
            (member.Trackers.Enums.TryGetValue(out ManorLouegieStage value) && (value == ManorLouegieStage.AcceptedQuest)) || ((value == ManorLouegieStage.CompletedQuest) &&
                member.WithinLevelRange(source)));

        if (allMembersHaveQuestFlag)
        {
            foreach (var member in aisling.Group)
            {
                member.TraverseMap(targetMap, Destination);  
            }
        }
        else
        {
            // Send a message to the Aisling
            aisling.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "Make sure everyone is within level range and has accepted the quest.");

            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }
}