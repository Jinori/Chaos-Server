using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.CthonicRemains;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.CthonicRemains;

public class CrHorrorEntranceScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IMerchantFactory _merchantFactory;

    public CrHorrorEntranceScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
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
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");
         // Warp the source back
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
        return;
    }
     // Check if all members of the group have the quest flag and are within level range
     var allMembersHaveQuestFlag = group.All(member => member.Trackers.Flags.HasFlag(QuestFlag1.CrHorror) && member.WithinLevelRange(source));
     if (allMembersHaveQuestFlag)
    {
        // Create a merchant at the Aisling's current point
        var npcpoint = new Point(aisling.X, aisling.Y);
        var merchant = _merchantFactory.Create("brynhorn", aisling.MapInstance, npcpoint);
         // Create a dialog for the merchant
        var dialog = _dialogFactory.Create("crhorrorenter", merchant);
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
