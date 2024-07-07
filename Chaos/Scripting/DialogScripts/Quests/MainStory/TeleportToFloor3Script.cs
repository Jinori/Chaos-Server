using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class TeleportToFloor3Script : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToFloor3Script(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        if (!IsGroupValid(source))
        {
            SendGroupInvalidMessage(source);
            WarpSourceBack(source);
            return;
        }

        var group = GetNearbyGroupMembers(source);

        if (group == null || group.Count == 0)
        {
            SendNoGroupMembersMessage(source);
            WarpSourceBack(source);
            return;
        }

        if (!IsGroupWithinLevelRange(source, group))
        {
            SendLevelRangeInvalidMessage(source);
            WarpSourceBack(source);
            return;
        }

        if (!IsGroupEligible(source))
        {
            SendGroupEligibleMessage(source);
            WarpSourceBack(source);
            return;
        }

        TeleportGroupTo3RdFloor(source, group);
    }

    private bool IsGroupEligible(Aisling source) =>
        source.Group != null && source.Group.All(x =>
            x.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3) ||
            x.Trackers.Enums.HasValue(MainStoryEnums.RetryServant) ||
            x.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner) &&
             x.Inventory.HasCount("True Elemental Artifact", 1));
    private bool IsGroupValid(Aisling source) =>
        source.Group != null && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private List<Aisling>? GetNearbyGroupMembers(Aisling source) =>
        source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group) =>
        group.All(member => member.WithinLevelRange(source) && member.UserStatSheet.Level > 96);

    private void SendGroupInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Exploring the third floor requires a group.");
        Subject.Reply(source, "You have no group members nearby.");
    }

    private void SendNoGroupMembersMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Exploring the third floor requires a group.");
        Subject.Reply(source, "You have no group members nearby.");
    }

    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
        Subject.Reply(source, "Some of your companions are not within your level range.");
    }

    private void SendGroupEligibleMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members are on this quest or completed it.");
        Subject.Reply(source, "Not all of your members are ready to explore the third floor of the manor. Members must be on same part of quest and have the True Elemental Artifact or have completed Eingren Manor Floor 3 already.");
    }

    private void WarpSourceBack(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }

    private void TeleportGroupTo3RdFloor(Aisling source, List<Aisling> group)
    {
        var rectangle = new Rectangle(35, 59, 3, 3);
        var mapInstance = SimpleCache.Get<MapInstance>("manor_floor_3");

        foreach (var member in group)
        {
            Point point;
            do
            {
                point = rectangle.GetRandomPoint();
            }
            while (!mapInstance.IsWalkable(point, member.Type));

            if (member.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner) ||
                member.Trackers.Enums.HasValue(MainStoryEnums.RetryServant))
            {
                member.Trackers.Enums.Set(MainStoryEnums.Entered3rdFloor);
            }

            member.Inventory.Remove("True Elemental Artifact");
            
            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            Subject.Close(source);
            member.TraverseMap(mapInstance, point);
        }
    }
}
