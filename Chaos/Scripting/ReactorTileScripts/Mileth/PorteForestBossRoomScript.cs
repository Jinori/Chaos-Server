﻿using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Mileth;

public class PorteForestBossRoomScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IMerchantFactory _merchantFactory;

    public PorteForestBossRoomScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        _merchantFactory = merchantFactory;
        _dialogFactory = dialogFactory;
    }

public override void OnWalkedOn(Creature source)
{
    // Check if the source is an Aisling
    if (source is  not Aisling aisling)
        return;
     // Get the current point of the Aisling
    var currentPoint = new Point(aisling.X, aisling.Y);
     // Get the group of Aislings near the current point
    var group = aisling.Group?.Where(x => x.WithinRange(currentPoint)).ToList();
     // Check if the group is null or has only one member
    if (group is null || (group.Count <= 1))
    {
        // Send a message to the Aisling
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You're nervous to enter without a group...");
         // Warp the source back
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
        return;
    }
     // Check if all members of the group have the quest flag and are within level range
    var allMembersHaveQuestEnum = group.All(member => member.Trackers.Enums.TryGetValue(out PFQuestStage stage) && (stage == PFQuestStage.FoundPendant));
    if (allMembersHaveQuestEnum)
    {
        // Create a merchant at the Aisling's current point
        var npcpoint = new Point(aisling.X, aisling.Y);
        var merchant = _merchantFactory.Create("pf_bossroomentrance_merchant", aisling.MapInstance, npcpoint);
         // Create a dialog for the merchant
        var dialog = _dialogFactory.Create("pf_bossroomentrance", merchant);
        dialog.Display(aisling);
    }
    else
    {
        // Send a message to the Aisling
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members have found the pendant.");
         // Warp the source back
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }
}
}