﻿using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Mileth;

public class TerrorOfCryptEntranceScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IMerchantFactory _merchantFactory;

    public TerrorOfCryptEntranceScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        _merchantFactory = merchantFactory;
        _dialogFactory = dialogFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        // Check if the source is an Aisling
        if (source is not Aisling aisling)
            return;

        // Check if the group is null or has only one member
        if (aisling.Group is null || aisling.Group.Any(x => !x.OnSameMapAs(aisling) || !x.WithinRange(aisling)))
        {
            // Send a message to the Aisling
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");
            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        // Check if all members of the group have the quest flag and are within level range
        var allMembersHaveQuestFlag = aisling.Group.All(
            member => member.Trackers.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt) && member.WithinLevelRange(source));

        if (allMembersHaveQuestFlag)
        {
            // Create a merchant at the Aisling's current point
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = _merchantFactory.Create("teague", aisling.MapInstance, npcpoint);
            // Create a dialog for the merchant
            var dialog = _dialogFactory.Create("teague_enterTerror", merchant);
            dialog.Display(aisling);
        }
        else
        {
            // Send a message to the Aisling
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure everyone is within level range and has quest.");
            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }
}