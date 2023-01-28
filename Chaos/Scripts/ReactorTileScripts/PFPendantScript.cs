using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts;

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
        var aisling = source as Aisling;
        var hasStage = aisling.Enums.TryGetValue(out PFQuestStage stage);

        if (stage is PFQuestStage.TurnedInRoots or PFQuestStage.WolfManes or PFQuestStage.WolfManesTurnedIn)
        {
            var pendant = ItemFactory.Create("turucpendant");
            var dialog = DialogFactory.Create("pf_foundpendant", pendant);
            dialog.Display(aisling);
            aisling.TryGiveItem(pendant);
            aisling.SendOrangeBarMessage("You found the Turuc Pendant, this is what Bertil dropped.");
            aisling.Enums.Set(PFQuestStage.FoundPendant);
        }
        return;
    }
}