using Chaos.Common.Definitions;
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
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public PorteForestBossRoomScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
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
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You're nervous to enter without your full group...");
            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        // Check if all members of the group have the quest flag and are within level range
        var allMembersHaveQuestEnum = aisling.Group.All(
            member => member.Trackers.Enums.TryGetValue(out PFQuestStage stage)
                      && stage is PFQuestStage.WolfManesTurnedIn or PFQuestStage.WolfManes or PFQuestStage.TurnedInRoots
                      && member.Inventory.Contains("Turuc Pendant"));

        if (allMembersHaveQuestEnum)
        {
            // Create a merchant at the Aisling's current point
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("pf_bossroomentrance_merchant", aisling.MapInstance, npcpoint);
            // Create a dialog for the merchant
            var dialog = DialogFactory.Create("pf_bossroomentrance", merchant);
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