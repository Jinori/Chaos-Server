using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.PentagramQuest;

public class RogueSymbolScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;
    private static readonly Point RogueSymbolSpot = new Point(7, 62);

    public RogueSymbolScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage)
            && (stage == PentagramQuestStage.ReceivedClue))
            if (aisling.UserStatSheet.BaseClass == BaseClass.Rogue)
            {
                var npcpoint = new Point(aisling.X, aisling.Y);
                var cluemerchant = MerchantFactory.Create("pentaworm_merchant", aisling.MapInstance, npcpoint);
                var classDialog = DialogFactory.Create("pentasymbol", cluemerchant);
                classDialog.Display(aisling);
            }
    }
}
