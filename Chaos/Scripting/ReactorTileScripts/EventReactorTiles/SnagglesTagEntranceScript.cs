using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EventReactorTiles;

public class SnagglesTagEntranceScript(
    ReactorTile subject,
    IMerchantFactory merchantFactory,
    IDialogFactory dialogFactory)
    : ReactorTileScriptBase(subject)
{
    public override void OnWalkedOn(Creature source)
    {
        // Check if the source is an Aisling
        if (source is not Aisling aisling)
            return;
        
        // Check if the current map is part of an active event
        if (!EventPeriod.IsEventActive(DateTime.UtcNow, Subject.MapInstance.InstanceId))
        {
            return;
        }
        
        source.Trackers.Enums.TryGetValue(out FlourentineQuest queststate);

        if (queststate is not FlourentineQuest.SpeakWithSnaggles)
            return;
        
        // Create a merchant at the Aisling's current point
        var npcpoint = new Point(aisling.X, aisling.Y);
        var merchant = merchantFactory.Create("snagglesthesweetsnatcher", aisling.MapInstance, npcpoint);
        // Create a dialog for the merchant
        var dialog = dialogFactory.Create("snagglesthesweetsnatcher_portal", merchant);
        dialog.Display(aisling);
    }
}