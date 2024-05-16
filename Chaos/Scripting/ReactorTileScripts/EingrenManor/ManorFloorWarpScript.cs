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
    private readonly Random Random = new();
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
        // List of map template keys
        string[] mapKeys = {
            "manor_library",
            "manor_study",
            "manor_study_2",
            "manor_kitchen",
            "manor_kitchen_2",
            "manor_commons",
            "manor_storage",
            "manor_depot",
            "manor_bedroom",
            "manor_bedroom_2",
            "manor_bedroom_3",
            "manor_bunks",
            "manor_master_suite"
        };

        // Randomly select a map key
        var selectedMapKey = mapKeys[Random.Next(mapKeys.Length)];
        var targetMap = SimpleCache.Get<MapInstance>(selectedMapKey);
        var aisling = source as Aisling;

        if (source.Trackers.TimedEvents.HasActiveEvent("Louegie2ndFloor", out _))
        {
            aisling?.SendOrangeBarMessage("You have recently killed all the banshees.");
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
        var allMembersHaveQuestFlag = aisling.Group.All(
            member =>
                member.Trackers.Enums.TryGetValue(out ManorLouegieStage value)
                && (value == ManorLouegieStage.AcceptedQuestBanshee)
                && member.WithinLevelRange(source)
                && (member.StatSheet.Level >= 41));

        if (allMembersHaveQuestFlag)
        {
            var point = targetMap.GetRandomWalkablePoint();
            foreach (var member in aisling.Group)
            {
                member.TraverseMap(targetMap, point);
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