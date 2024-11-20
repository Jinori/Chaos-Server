using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.NightmareQuest;

public class CthonicDiscipleCoffinScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public CthonicDiscipleCoffinScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
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

        if ((hasStage && (stage == NightmareQuestStage.Started)) || (stage == NightmareQuestStage.MetRequirementsToEnter1) || (stage == NightmareQuestStage.EnteredDream) || (stage == NightmareQuestStage.SpawnedNightmare))
            if (aisling.UserStatSheet.BaseClass is BaseClass.Priest or BaseClass.Wizard && ((aisling.Gender & Gender.Male) != 0))
                if (aisling.Inventory.Contains("Essence of Theselene")
                    && (aisling.Inventory.Contains("Essence of Serendael")
                        && (aisling.Inventory.Contains("Essence of Miraelis")))
                    && aisling.Inventory.Contains("Essence of Skandara"))
                {
                    aisling.Trackers.Enums.Set(NightmareQuestStage.MetRequirementsToEnter1);   
                    aisling.SendOrangeBarMessage("The coffin murmurs...");  
                    var npcpoint = new Point(aisling.X, aisling.Y);
                    var cluemerchant = MerchantFactory.Create("cthonicdisciple_merchant", aisling.MapInstance, npcpoint);
                    var classDialog = DialogFactory.Create("cthonicdisciplecoffin1", cluemerchant);
                    classDialog.Display(aisling);
                } else
                {
                    aisling.SendOrangeBarMessage("You hear something but can't make out what it is.");
                }
        
    }
}
