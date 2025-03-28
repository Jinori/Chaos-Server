﻿using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.KarloposQuest;

public class TeleportToTrialScript : DialogScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToTrialScript(Dialog subject, ISimpleCache simpleCache, IDialogFactory dialogFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        DialogFactory = dialogFactory;
    }

    private MapInstance? GetNextInstance(string mapInstanceId, out Rectangle currentTrial)
    {
        currentTrial = mapInstanceId.ToLower() switch
        {
            "karlopossecret" => new Rectangle(
                20,
                56,
                3,
                2),
            "karlopostrap" => new Rectangle(
                3,
                7,
                3,
                2),
            "karloposfirsttrial" => new Rectangle(
                9,
                11,
                3,
                2),
            "karlopossecondtrial" => new Rectangle(
                9,
                9,
                3,
                2),
            "karloposthirdtrial" => new Rectangle(
                7,
                11,
                3,
                2),
            "karloposfinaltrial" => new Rectangle(
                6,
                9,
                3,
                2),
            _ => new Rectangle(
                0,
                0,
                1,
                1)
        };

        return mapInstanceId.ToLower() switch
        {
            "karlopossecret"      => SimpleCache.Get<MapInstance>("karlopostrap"),
            "karlopostrap"        => SimpleCache.Get<MapInstance>("karloposfirsttrial"),
            "karloposfirsttrial"  => SimpleCache.Get<MapInstance>("karlopossecondtrial"),
            "karlopossecondtrial" => SimpleCache.Get<MapInstance>("karloposthirdtrial"),
            "karloposthirdtrial"  => SimpleCache.Get<MapInstance>("karloposfinaltrial"),
            "karloposfinaltrial"  => SimpleCache.Get<MapInstance>("karlopospendant"),
            _                     => null
        };
    }

    public override void OnDisplaying(Aisling source)
    {
        var group = source.Group;

        if ((group == null) || group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source)))
        {
            SendMessageAndReply(source, "You are missing your group.", "You must have a group to enter the next room.");
            WarpSourceBack(source);

            return;
        }

        if (!group.All(member => member.WithinLevelRange(source)))
        {
            SendMessageAndReply(
                source,
                "Make sure your companions are within level range.",
                "Some of your companions are not within your level range.");
            WarpSourceBack(source);

            return;
        }

        if (group.Any(
                x => !x.Trackers.Enums.HasValue(QueenOctopusQuest.Pendant2) && !x.Trackers.Enums.HasValue(QueenOctopusQuest.Complete)))
        {
            SendMessageAndReply(
                source,
                "Your group must be on the same step as you.",
                "You must have a group on the same progress or finished this quest to enter the next room.");
            WarpSourceBack(source);

            return;
        }

        var nextInstance = GetNextInstance(source.MapInstance.InstanceId, out var currentTrial);

        if (nextInstance == null)
        {
            SendMessageAndReply(source, "Invalid map instance.", "Unable to determine the next trial.");

            return;
        }

        foreach (var member in group)
        {
            Point point;

            do
                point = currentTrial.GetRandomPoint();
            while (!nextInstance.IsWalkable(point, member.Type));

            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            member.Trackers.Counters.Remove("karlopostrialkills", out _);
            member.TraverseMap(nextInstance, point);
        }
    }

    private void SendMessageAndReply(Aisling source, string serverMessage, string replyMessage)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, serverMessage);
        Subject.Reply(source, replyMessage);
    }

    private static void WarpSourceBack(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }
}