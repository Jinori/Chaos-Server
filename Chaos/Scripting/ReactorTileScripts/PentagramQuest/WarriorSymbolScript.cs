using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.PentagramQuest;

public class WarriorSymbolScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;
    private static readonly Point WarriorSymbolSpot = new Point(7, 62);

    public WarriorSymbolScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
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
            if (aisling.UserStatSheet.BaseClass == BaseClass.Warrior)
            {
                var npcpoint = new Point(aisling.X, aisling.Y);
                var cluemerchant = MerchantFactory.Create("pentabeetle_merchant", aisling.MapInstance, npcpoint);
                var classDialog = DialogFactory.Create("pentasymbol", cluemerchant);
                classDialog.Display(aisling);
            }
    }
}

