using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class TeleportToCthonicDomainScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToCthonicDomainScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    private List<Aisling>? GetNearbyGroupMembers(Aisling source)
        => source.Group
                 ?.Where(x => x.WithinRange(new Point(source.X, source.Y)))
                 .ToList();

    private bool IsGroupEligible(Aisling source)
        => (source.Group != null)
           && source.Group.All(
               x => x.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory)
                    || x.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2)
                    || x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)
                    || x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)
                    || x.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight)
                    || x.Trackers.Enums.HasValue(MainStoryEnums.KilledSummoner));

    private bool IsGroupRequiredVitality(Aisling source)
        => (source.Group != null) && source.Group.All(x => (x.StatSheet.MaximumHp + x.StatSheet.MaximumMp * 2) >= 25000);

    private bool IsGroupValid(Aisling source)
        => (source.Group != null) && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group)
        => group.All(
            member => member.WithinLevelRange(source)
                      && (member.UserStatSheet.Level > 98)
                      && ((member.UserStatSheet.MaximumHp + member.UserStatSheet.MaximumMp * 2) > 25000));

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

        if (!IsGroupRequiredVitality(source))
        {
            SendGroupVitalityMessage(source);
            WarpSourceBack(source);
        }

        TeleportGroupTo3RdFloor(source, group);
    }

    private void SendGroupEligibleMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members are on this quest.");

        Subject.Reply(
            source,
            "Not all of your members are ready to face the darkness. Members must be on same part of quest or have completed the Pre-Master Mainstory Quest Line.");
    }

    private void SendGroupInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Facing the darkness in this room requires a group.");
        Subject.Reply(source, "You have no group members nearby.");
    }

    private void SendGroupVitalityMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members are above 30,000 Vitality.");
        Subject.Reply(source, "Not all of your members are higher than 25,000 Vitality to face the Summoner.");
    }

    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "One of your group members do not meet the requirements.");

        Subject.Reply(
            source,
            "The requirements to face the darkness is 25,000 Vitality and level 99. One of your group members does not meet the requirements.");
    }

    private void SendNoGroupMembersMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Facing the darkness in this room requires a group.");
        Subject.Reply(source, "You have no group members nearby.");
    }

    private void TeleportGroupTo3RdFloor(Aisling source, List<Aisling> group)
    {
        var rectangle = new Rectangle(
            24,
            14,
            3,
            4);
        var mapInstance = SimpleCache.Get<MapInstance>("cthonic_domain2");

        foreach (var member in group)
        {
            Subject.Close(source);

            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));

            if (member.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)
                || member.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)
                || member.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight))
            {
                member.Trackers.Enums.Set(MainStoryEnums.SearchForSummoner2);
                member.Trackers.Enums.Remove<SummonerBossFight>();
            }

            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            member.TraverseMap(mapInstance, point);
        }
    }

    private void WarpSourceBack(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }
}