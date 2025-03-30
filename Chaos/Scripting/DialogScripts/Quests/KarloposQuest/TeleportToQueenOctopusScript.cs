using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.KarloposQuest;

public class TeleportToQueenOctopusScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToQueenOctopusScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    private List<Aisling>? GetNearbyGroupMembers(Aisling source)
        => source.Group
                 ?.Where(x => x.WithinRange(new Point(source.X, source.Y)))
                 .ToList();

    private bool IsGroupValid(Aisling source)
        => (source.Group != null) && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group) => group.All(member => member.WithinLevelRange(source));

    public override void OnDisplaying(Aisling source)
    {
        if (!IsGroupValid(source))
        {
            SendGroupInvalidMessage(source);
            WarpSourceBack(source);

            return;
        }

        var group = GetNearbyGroupMembers(source);

        if ((group == null) || (group.Count == 0))
        {
            SendNoGroupMembersMessage(source);

            return;
        }

        if (!IsGroupWithinLevelRange(source, group))
        {
            SendLevelRangeInvalidMessage(source);

            return;
        }

        TeleportGroupToQueenRoom(source, group);
    }

    private void SendGroupInvalidMessage(Aisling source)
        => source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");

    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
        Subject.Reply(source, "Some of your companions are not within your level range.");
    }

    private void SendNoGroupMembersMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Walking this beach requires a group.");
        Subject.Reply(source, "You have no group members nearby.");
    }

    private void TeleportGroupToQueenRoom(Aisling source, List<Aisling> group)
    {
        const string MAP_INSTANCE_KEY = "karloposqueenroom";

        var rectangle = new Rectangle(
            9,
            16,
            2,
            2);
        var mapInstance = SimpleCache.Get<MapInstance>(MAP_INSTANCE_KEY);

        foreach (var member in group)
        {
            Subject.Close(source);

            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));

            member.TraverseMap(mapInstance, point);
            member.Trackers.Enums.Set(QueenOctopusQuest.QueenSpawning);
            member.Inventory.Remove("Red Pearl");
            member.Inventory.Remove("Coral Pendant");
        }
    }

    private void WarpSourceBack(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }
}