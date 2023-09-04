using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.NightmareQuest;

public class CarnunGladeScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public CarnunGladeScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);

        if (hasStage && (stage == NightmareQuestStage.Started))
            if (aisling.UserStatSheet.BaseClass == BaseClass.Warrior)
            {
                aisling.Trackers.Enums.Set(NightmareQuestStage.MetRequirementsToEnter1);   
                aisling.SendOrangeBarMessage("You notice what looks like the glade of Carnun.");
            }

        if (stage is NightmareQuestStage.MetRequirementsToEnter1 or NightmareQuestStage.EnteredDream or NightmareQuestStage.SpawnedNightmare)
            if (aisling.UserStatSheet.BaseClass == BaseClass.Warrior)
            {
                var npcpoint = new Point(aisling.X, aisling.Y);
                var cluemerchant = MerchantFactory.Create("carnunchampion_merchant", aisling.MapInstance, npcpoint);
                var classDialog = DialogFactory.Create("carnunglade1", cluemerchant);
                classDialog.Display(aisling);
            }
    }
}
