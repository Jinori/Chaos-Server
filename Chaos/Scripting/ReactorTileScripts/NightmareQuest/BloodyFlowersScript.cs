using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.NightmareQuest;

public class BloodyFlowersScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    public BloodyFlowersScript(ReactorTile subject, IDialogFactory dialogFactory, IItemFactory itemFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ItemFactory = itemFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);

        if ((hasStage && (stage == NightmareQuestStage.Started))
            || (stage == NightmareQuestStage.MetRequirementsToEnter1)
            || (stage == NightmareQuestStage.EnteredDream)
            || (stage == NightmareQuestStage.SpawnedNightmare))
            if (aisling.UserStatSheet.BaseClass is BaseClass.Priest or BaseClass.Wizard && ((aisling.Gender & Gender.Female) != 0))
            {
                aisling.Trackers.Enums.Set(NightmareQuestStage.MetRequirementsToEnter1);
                aisling.SendOrangeBarMessage("You notice the red red flowers");
                var item = ItemFactory.Create("morrigudisciplepellison");
                var classDialog = DialogFactory.Create("bloodyflowers1", item);
                classDialog.Display(aisling);
            }
    }
}