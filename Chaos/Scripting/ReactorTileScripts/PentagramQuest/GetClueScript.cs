using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.PentagramQuest;

public class GetClueScript : ReactorTileScriptBase
{
    private static readonly Point WizardClueSpot = new(10, 13);
    private static readonly Point WarriorClueSpot = new(7, 7);
    private static readonly Point RogueClueSpot = new(11, 6);
    private static readonly Point PriestClueSpot = new(13, 10);
    private static readonly Point MonkClueSpot = new(6, 11);
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public GetClueScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage) && (stage == PentagramQuestStage.StartedRitual))
            switch (aisling.UserStatSheet.BaseClass)
            {
                case BaseClass.Warrior:
                    if (PriestClueSpot.Equals(aisling))
                    {
                        var npcpoint = new Point(aisling.X, aisling.Y);

                        var cluemerchant = MerchantFactory.Create("pentakingrat_merchant", aisling.MapInstance, npcpoint);

                        var classDialog = DialogFactory.Create("pentaclue", cluemerchant);
                        classDialog.Display(aisling);

                        source.Trackers.Enums.Set(PentagramQuestStage.ReceivedClue);
                        aisling.SendOrangeBarMessage("You have received a clue.");
                    }

                    break;

                case BaseClass.Rogue:
                    if (WizardClueSpot.Equals(aisling))
                    {
                        var npcpoint = new Point(aisling.X, aisling.Y);
                        var cluemerchant = MerchantFactory.Create("pentarat_merchant", aisling.MapInstance, npcpoint);
                        var classDialog = DialogFactory.Create("pentaclue", cluemerchant);
                        classDialog.Display(aisling);

                        source.Trackers.Enums.Set(PentagramQuestStage.ReceivedClue);
                        aisling.SendOrangeBarMessage("You have received a clue.");
                    }

                    break;

                case BaseClass.Wizard:
                    if (WarriorClueSpot.Equals(aisling))
                    {
                        var npcpoint = new Point(aisling.X, aisling.Y);

                        var cluemerchant = MerchantFactory.Create("pentabeetle_merchant", aisling.MapInstance, npcpoint);

                        var classDialog = DialogFactory.Create("pentaclue", cluemerchant);
                        classDialog.Display(aisling);

                        source.Trackers.Enums.Set(PentagramQuestStage.ReceivedClue);
                        aisling.SendOrangeBarMessage("You have received a clue.");
                    }

                    break;

                case BaseClass.Priest:
                    if (MonkClueSpot.Equals(aisling))
                    {
                        var npcpoint = new Point(aisling.X, aisling.Y);

                        var cluemerchant = MerchantFactory.Create("pentaspider_merchant", aisling.MapInstance, npcpoint);

                        var classDialog = DialogFactory.Create("pentaclue", cluemerchant);
                        classDialog.Display(aisling);

                        source.Trackers.Enums.Set(PentagramQuestStage.ReceivedClue);
                        aisling.SendOrangeBarMessage("You have received a clue.");
                    }

                    break;

                case BaseClass.Monk:
                    if (RogueClueSpot.Equals(aisling))
                    {
                        var npcpoint = new Point(aisling.X, aisling.Y);
                        var cluemerchant = MerchantFactory.Create("pentaworm_merchant", aisling.MapInstance, npcpoint);
                        var classDialog = DialogFactory.Create("pentaclue", cluemerchant);
                        classDialog.Display(aisling);

                        source.Trackers.Enums.Set(PentagramQuestStage.ReceivedClue);
                        aisling.SendOrangeBarMessage("You have received a clue.");
                    }

                    break;
            }
    }
}