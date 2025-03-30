using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.PentagramQuest;

public class WizardSymbolScript : ReactorTileScriptBase
{
    private static readonly Point WizardSymbolSpot = new(7, 62);
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public WizardSymbolScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage) && (stage == PentagramQuestStage.ReceivedClue))
            if (aisling.UserStatSheet.BaseClass == BaseClass.Wizard)
            {
                var npcpoint = new Point(aisling.X, aisling.Y);
                var cluemerchant = MerchantFactory.Create("pentarat_merchant", aisling.MapInstance, npcpoint);
                var classDialog = DialogFactory.Create("pentasymbol", cluemerchant);
                classDialog.Display(aisling);
            }
    }
}