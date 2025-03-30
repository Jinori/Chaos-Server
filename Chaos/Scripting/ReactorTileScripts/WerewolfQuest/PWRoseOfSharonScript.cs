using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class PwRoseOfSharonScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public PwRoseOfSharonScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Inventory.Contains("Rose of Sharon"))
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out WerewolfOfPiet stage);

        if (hasStage && stage is WerewolfOfPiet.KilledWerewolf)
        {
            source.Trackers.Enums.Set(WerewolfOfPiet.CollectedBlueFlower);
            var roseofsharon = ItemFactory.Create("roseofsharon");
            var dialog = DialogFactory.Create("roseofsharon1", roseofsharon);
            dialog.Display(aisling);
            aisling.GiveItemOrSendToBank(roseofsharon);
            aisling.SendOrangeBarMessage("You found the Rose of Sharon, this is the flower Appie mentioned.");
        } else
            aisling.SendOrangeBarMessage("The Master Werewolf is tracking you, you're afraid to pick the flower.");
    }
}