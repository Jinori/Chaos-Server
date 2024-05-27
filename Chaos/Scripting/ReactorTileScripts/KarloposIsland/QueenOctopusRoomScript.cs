using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.KarloposIsland;

public class QueenOctopusRoomScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public QueenOctopusRoomScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
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

        if (aisling.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage) && stage == QueenOctopusQuest.QueenKilled ||
            stage == QueenOctopusQuest.Complete)
        {
            aisling.SendOrangeBarMessage("You've already killed the queen for yourself.");
            return;
        }

        if (stage != QueenOctopusQuest.SpokeToMaria && stage != QueenOctopusQuest.QueenSpawned)
        {
            aisling.SendOrangeBarMessage("You are not on this part of the quest.");
            return;
        }
        
        // Check if all members of the group have the quest flag and are within level range
        var missingRequirements = string.Empty;

        var allMembersHaveQuestEnum = aisling.Group.All(member =>
        {
            
            if (member.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage) &&
                stage == QueenOctopusQuest.QueenSpawned || stage == QueenOctopusQuest.Complete)
                return true;
            
            
            if (stage != QueenOctopusQuest.SpokeToMaria || !member.Inventory.Contains("Red Pearl") 
                && !member.Inventory.Contains("Coral Pendant"))
            {
                missingRequirements += $"{member.Name}: ";

                if (stage != QueenOctopusQuest.SpokeToMaria && stage != QueenOctopusQuest.QueenSpawned)
                {
                    missingRequirements += "not on this part of quest, ";
                    if (!member.Inventory.Contains("Red Pearl"))
                        missingRequirements += "missing Red Pearl, ";
                    if (!member.Inventory.Contains("Coral Pendant"))
                        missingRequirements += "missing Coral Pendant, ";
                }
                return false;
            }

            return true;
        });
        
        if (allMembersHaveQuestEnum)
        {
            // Create a merchant at the Aisling's current point
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("QueenOctopusEntrance_merchant", aisling.MapInstance, npcpoint);
            // Create a dialog for the merchant
            var dialog = DialogFactory.Create("QueenOctopusEntrance", merchant);
            dialog.Display(aisling);
        }
        else
        {
            // Send a message to the Aisling with the missing requirements
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("QueenOctopusEntrance_merchant", aisling.MapInstance, npcpoint);
            // Create a dialog for the merchant
            var dialog = DialogFactory.Create("QueenOctopusEntrance", merchant);
            dialog.Reply(aisling, $"{missingRequirements}");
            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }
}