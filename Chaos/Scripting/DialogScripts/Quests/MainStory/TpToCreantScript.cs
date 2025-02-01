using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class TpToCreantScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TpToCreantScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    private List<Aisling>? GetNearbyGroupMembers(Aisling source)
        => source.Group
                 ?.Where(x => x.WithinRange(new Point(source.X, source.Y)))
                 .ToList();

    private bool IsGroupEligible(Aisling source)
    {
        if (source.MapInstance.Template.TemplateKey == "6599")
        {
            // ❌ If ANYONE in the group has KilledMedusa or CompletedMedusa, return false
            if ((source.Group != null)
                && source.Group.Any(
                    x => x.Trackers.Flags.HasFlag(CreantEnums.KilledMedusa) || x.Trackers.Flags.HasFlag(CreantEnums.CompletedMedusa)))
                return false;

            // ❌ If ANYONE in the group does NOT have StartedCreants OR CreantRewards, return false
            if ((source.Group != null)
                && source.Group.Any(
                    x => !x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants)
                         && !x.Trackers.Flags.HasFlag(MainstoryFlags.CreantRewards)))
                return false;
        }

        if (source.MapInstance.Template.TemplateKey == "989")
        {
            if ((source.Group != null)
                && source.Group.Any(
                    x => x.Trackers.Flags.HasFlag(CreantEnums.KilledPhoenix) || x.Trackers.Flags.HasFlag(CreantEnums.CompletedPhoenix)))
                return false;

            // ❌ If ANYONE in the group does NOT have StartedCreants OR CreantRewards, return false
            if ((source.Group != null)
                && source.Group.Any(
                    x => !x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants)
                         && !x.Trackers.Flags.HasFlag(MainstoryFlags.CreantRewards)))
                return false;
        }

        if (source.MapInstance.Template.TemplateKey == "31010")
        {
            if ((source.Group != null)
                && source.Group.Any(
                    x => x.Trackers.Flags.HasFlag(CreantEnums.KilledSham) || x.Trackers.Flags.HasFlag(CreantEnums.CompletedSham)))
                return false;

            // ❌ If ANYONE in the group does NOT have StartedCreants OR CreantRewards, return false
            if ((source.Group != null)
                && source.Group.Any(
                    x => !x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants)
                         && !x.Trackers.Flags.HasFlag(MainstoryFlags.CreantRewards)))
                return false;
        }

        if (source.MapInstance.Template.TemplateKey == "19522")
        {
            if ((source.Group != null)
                && source.Group.Any(
                    x => x.Trackers.Flags.HasFlag(CreantEnums.KilledTauren) || x.Trackers.Flags.HasFlag(CreantEnums.CompletedTauren)))
                return false;

            // ❌ If ANYONE in the group does NOT have StartedCreants OR CreantRewards, return false
            if ((source.Group != null)
                && source.Group.Any(
                    x => !x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants)
                         && !x.Trackers.Flags.HasFlag(MainstoryFlags.CreantRewards)))
                return false;
        }

        return true;
    }

    private bool IsGroupValid(Aisling source)
        => (source.Group != null) && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group)
        => group.All(member => member.WithinLevelRange(source) && member.UserStatSheet is { Level: > 98, Master: true });

    public override void OnDisplaying(Aisling source)
    {
        if (source.IsGodModeEnabled())
        {
            if (source.MapInstance.Template.TemplateKey == "989")
            {
                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    source.Trackers.Flags.AddFlag(CreantEnums.StartedPhoenix);

                var mapInstance = SimpleCache.Get<MapInstance>("phoenixbossroom");

                var point = new Point(source.X, source.Y);

                var dialog = source.ActiveDialog.Get();
                dialog?.Close(source);
                Subject.Close(source);
                source.TraverseMap(mapInstance, point);

                return;
            }

            if (source.MapInstance.Template.TemplateKey == "31010")
            {
                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    source.Trackers.Flags.AddFlag(CreantEnums.StartedSham);

                var mapInstance = SimpleCache.Get<MapInstance>("shamensythbossroom");

                var point = new Point(source.X, source.Y);

                var dialog = source.ActiveDialog.Get();
                dialog?.Close(source);
                Subject.Close(source);
                source.TraverseMap(mapInstance, point);

                return;
            }

            if (source.MapInstance.Template.TemplateKey == "19522")
            {
                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    source.Trackers.Flags.AddFlag(CreantEnums.StartedTauren);

                var mapInstance = SimpleCache.Get<MapInstance>("taurenbossroom");

                var point = new Point(source.X, source.Y);

                var dialog = source.ActiveDialog.Get();
                dialog?.Close(source);
                Subject.Close(source);
                source.TraverseMap(mapInstance, point);

                return;
            }

            if (source.MapInstance.Template.TemplateKey == "6599")
            {
                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    source.Trackers.Flags.AddFlag(CreantEnums.StartedMedusa);
                var mapInstance = SimpleCache.Get<MapInstance>("medusabossroom");

                var point = new Point(source.X, source.Y);

                var dialog = source.ActiveDialog.Get();
                dialog?.Close(source);
                Subject.Close(source);
                source.TraverseMap(mapInstance, point);

                return;
            }
        }

        if (!IsGroupValid(source))
        {
            SendGroupInvalidMessage(source);

            return;
        }

        var group = GetNearbyGroupMembers(source);

        if (group == null)
        {
            SendNoGroupMembersMessage(source);

            return;
        }

        if (!IsGroupWithinLevelRange(source, group))
        {
            SendLevelRangeInvalidMessage(source);

            return;
        }

        if (!IsGroupEligible(source))
        {
            SendGroupEligibleMessage(source);

            return;
        }

        TeleportGroupToCreant(source, group);
    }

    private void SendGroupEligibleMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members are on this quest or completed it.");

        Subject.Reply(
            source,
            "Not all of your members are ready to face the Creant. Members must be on same part of quest or have defeated all creants already.");
    }

    private void SendGroupInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You cannot face the Creant alone.");
        Subject.Reply(source, "You have no group members nearby.");
    }

    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "All of your group members must be mastered.");
        Subject.Reply(source, "Some of your companions are not mastered.");
    }

    private void SendNoGroupMembersMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You must have at least one group member at all.");
        Subject.Reply(source, "You do not have any group members nearby.");
    }

    private void TeleportGroupToCreant(Aisling source, List<Aisling> group)
    {
        if (source.MapInstance.Template.TemplateKey == "989")
        {
            var mapInstance = SimpleCache.Get<MapInstance>("phoenixbossroom");

            foreach (var member in group)
            {
                if (member.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    member.Trackers.Flags.AddFlag(CreantEnums.StartedPhoenix);

                var point = new Point(member.X, member.Y);

                var dialog = member.ActiveDialog.Get();
                dialog?.Close(member);
                Subject.Close(source);

                member.TraverseMap(mapInstance, point);
            }
        }

        if (source.MapInstance.Template.TemplateKey == "31010")
        {
            var mapInstance = SimpleCache.Get<MapInstance>("shamensythbossroom");

            foreach (var member in group)
            {
                if (member.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    member.Trackers.Flags.AddFlag(CreantEnums.StartedSham);

                var point = new Point(member.X, member.Y);

                var dialog = member.ActiveDialog.Get();
                dialog?.Close(member);
                Subject.Close(source);

                member.TraverseMap(mapInstance, point);
            }
        }

        if (source.MapInstance.Template.TemplateKey == "19522")
        {
            var mapInstance = SimpleCache.Get<MapInstance>("taurenbossroom");

            foreach (var member in group)
            {
                if (member.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    member.Trackers.Flags.AddFlag(CreantEnums.StartedTauren);

                var point = new Point(member.X, member.Y);

                var dialog = member.ActiveDialog.Get();
                dialog?.Close(member);
                Subject.Close(source);

                member.TraverseMap(mapInstance, point);
            }
        }

        if (source.MapInstance.Template.TemplateKey == "6599")
        {
            var mapInstance = SimpleCache.Get<MapInstance>("medusabossroom");

            foreach (var member in group)
            {
                if (member.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                    member.Trackers.Flags.AddFlag(CreantEnums.StartedMedusa);

                var point = new Point(member.X, member.Y);

                var dialog = member.ActiveDialog.Get();
                dialog?.Close(member);
                Subject.Close(source);

                member.TraverseMap(mapInstance, point);
            }
        }
    }
}