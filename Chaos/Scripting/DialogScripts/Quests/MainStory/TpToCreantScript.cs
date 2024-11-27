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
        => (source.Group != null)
           && source.Group.All(
               x => x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants)
                    || x.Trackers.Flags.HasFlag(MainstoryFlags.FinishedCreants));

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
                var point = new Point(member.X, member.Y);

                var dialog = member.ActiveDialog.Get();
                dialog?.Close(member);
                Subject.Close(source);

                member.TraverseMap(mapInstance, point);
            }
        }
    }
}