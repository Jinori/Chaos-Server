﻿using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class TeleportToCthonicDemiseScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToCthonicDemiseScript(Dialog subject, ISimpleCache simpleCache)
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

        if (group == null || group.Count <= 3)
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

        if (HasGroupDoneRecently(source))
        {
            SendGroupRecentMessage(source);
            WarpSourceBack(source);
            return;
        }

        TeleportGroupToCthonicDemise(source, group);
    }

    private bool IsGroupEligible(Aisling source) =>
        source.Group != null && source.Group.All(x =>
            x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedDungeon)
            || x.Trackers.Flags.HasFlag(MainstoryFlags.FinishedDungeon));

    private bool HasGroupDoneRecently(Aisling source) =>
        source.Group != null && source.Group.All(x => x.Trackers.TimedEvents.HasActiveEvent("cthonicdemise", out _));
    private bool IsGroupValid(Aisling source) =>
        source.Group != null && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private List<Aisling>? GetNearbyGroupMembers(Aisling source) =>
        source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group) =>
        group.All(member => member.WithinLevelRange(source) && member.UserStatSheet.Level > 96);

    private void SendGroupInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Facing the army alone is suicide.");
        Subject.Reply(source, "You have no group members nearby.");
    }
    
    private void SendGroupRecentMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Someone in your party needs time to rest.");
        Subject.Reply(source, "You or your group members have faced the army too recently.");
    }

    private void SendNoGroupMembersMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Facing the army requires at least four members of the group.");
        Subject.Reply(source, "You do not have enough group members nearby. To face the army, you must have atleast four group members.");
    }

    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "All of your group members must be within level range.");
        Subject.Reply(source, "Some of your companions are not within your level range.");
    }

    private void SendGroupEligibleMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members are on this quest or completed it.");
        Subject.Reply(source, "Not all of your members are ready to face the army. Members must be on same part of quest or have faced the army already.");
    }

    private void WarpSourceBack(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }

    private void TeleportGroupToCthonicDemise(Aisling source, List<Aisling> group)
    {
        var rectangle = new Rectangle(35, 59, 3, 3);
        var mapInstance = SimpleCache.Get<MapInstance>("cthonic_demise");

        foreach (var member in group)
        {
            if (member.Inventory.Contains("Cthonic Bell"))
                member.Inventory.RemoveQuantity("Cthonic Bell", 100);

            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Jane);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.John);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Mary);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Mike);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Pam);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Phil);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.William);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Wanda);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Roy);
            member.Trackers.Flags.RemoveFlag(CdDungeonBoss.Ray);
            
            Point point;
            do
            {
                point = rectangle.GetRandomPoint();
            }
            while (!mapInstance.IsWalkable(point, member.Type));
            
            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            Subject.Close(source);
            member.Trackers.TimedEvents.AddEvent("cthonic_demise", TimeSpan.FromHours(4), true);
            member.TraverseMap(mapInstance, point);
        }
    }
}