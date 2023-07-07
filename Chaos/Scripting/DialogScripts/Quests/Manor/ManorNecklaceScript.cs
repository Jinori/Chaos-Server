using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class ManorNecklaceScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public ManorNecklaceScript(Dialog subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "zulera_keephernecklace":
            {
                if (stage == ManorNecklaceStage.ObtainedNecklace)
                    source.Trackers.Enums.Set(ManorNecklaceStage.KeptNecklace);

                source.Legend.AddUnique(
                    new LegendMark(
                        "Stolen Zulera's Heirloom",
                        "manorNecklace",
                        MarkIcon.Rogue,
                        MarkColor.Orange,
                        1,
                        GameTime.Now));

                source.Client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    "You receive a legend mark. The young one looks terribly sad.");

                break;
            }
            case "zulera_givenecklaceback":
            {
                if (!source.Inventory.HasCount("Zulera's Heirloom", 1))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Looks like my necklace isn't in your inventory..");
                    Subject.Close(source);

                    return;
                }

                source.Inventory.RemoveQuantity("Zulera's Heirloom", 1);

                if (stage == ManorNecklaceStage.ObtainedNecklace)
                    source.Trackers.Enums.Set(ManorNecklaceStage.ReturnedNecklace);

                ExperienceDistributionScript.GiveExp(source, 150000);
                source.TryGiveGamePoints(20);

                source.Legend.AddUnique(
                    new LegendMark(
                        "Returned Zulera's Heirloom",
                        "manorNecklace",
                        MarkIcon.Heart,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));

                source.Client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    "You receive twenty gamepoints, legend mark and 150,000 exp!");

                break;
            }
            case "zulera_initial":
            {
                switch (stage)
                {
                    case ManorNecklaceStage.ReturnedNecklace:
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "She smiles and nods while clutching the necklace.");
                        Subject.Close(source);

                        break;
                    case ManorNecklaceStage.KeptNecklace:
                        Subject.Reply(source, "I don't really want to talk to you anymore. You're mean!");

                        return;
                    case ManorNecklaceStage.ObtainedNecklace:
                    {
                        Subject.Reply(source, "You found it! Would you please hand over my precious necklace?");
                        Subject.Type = ChaosDialogType.Menu;
                        Subject.NextDialogKey?.Remove(0);

                        var option = new DialogOption
                        {
                            DialogKey = "zulera_giveNecklaceBack",
                            OptionText = "Yes, here you go."
                        };

                        var optionTwo = new DialogOption
                        {
                            DialogKey = "zulera_keepHerNecklace",
                            OptionText = "It's nice. I'm keeping it!"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        if (!Subject.HasOption(optionTwo.OptionText))
                            Subject.Options.Add(optionTwo);

                        break;
                    }
                    case ManorNecklaceStage.AcceptedQuest:
                        Subject.Reply(source, "Come back when you've found the necklace, please!");

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            }
            case "zulera_acceptedquest":
            {
                if (!hasStage || (stage == ManorNecklaceStage.None))
                    source.Trackers.Enums.Set(ManorNecklaceStage.AcceptedQuest);

                source.SendOrangeBarMessage("Go find the girl's lost necklace inside the manor!");

                break;
            }
        }
    }
}