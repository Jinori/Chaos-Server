using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class PFPendantScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public PFPendantScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

        if (aisling.Inventory.Contains("Turuc Pendant"))
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

        if (hasStage && stage is PFQuestStage.TurnedInRoots or PFQuestStage.WolfManes or PFQuestStage.WolfManesTurnedIn)
        {
            var pendant = ItemFactory.Create("turucpendant");
            var dialog = DialogFactory.Create("pf_foundpendant", pendant);
            dialog.Display(aisling);
            aisling.GiveItemOrSendToBank(pendant);
            aisling.SendOrangeBarMessage("You found the Turuc Pendant, this is what Bertil dropped.");
        }
    }
}