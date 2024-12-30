using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EventReactorTiles
{
    public class MtMerryScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : ReactorTileScriptBase(subject)
    {
        public override void OnWalkedOn(Creature source)
        {
            if (source is not Aisling aisling)
                return;

            var currentDate = DateTime.UtcNow;
            const string DESTINATION_MAP_INSTANCE_ID = "mtmerry_northpole";

            // Check if the current map is part of an active event
            if (!EventPeriod.IsEventActive(currentDate, DESTINATION_MAP_INSTANCE_ID))
            {
                return;
            }

            if (source.StatSheet.Level < 11)
            {
                aisling.SendOrangeBarMessage("You cannot dream of Mount Merry until Level 11.");
                return;
            }

            // Create a merchant and display the dialog
            var point = new Point(source.X, source.Y);
            var blankMerchant = merchantFactory.Create("mtmerry_merchant", Subject.MapInstance, point);
            var dialog = dialogFactory.Create("mtmerry_initial", blankMerchant);
            dialog.Display(aisling);
        }
    }
}