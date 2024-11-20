using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class ManorNecklaceScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<ManorNecklaceScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public ManorNecklaceScript(Dialog subject, IItemFactory itemFactory, ILogger<ManorNecklaceScript> logger)
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        ItemFactory = itemFactory;
        Logger = logger;
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "zulera_keephernecklace":
            {
                if (stage == ManorNecklaceStage.ReturningNecklace)
                    source.Trackers.Enums.Set(ManorNecklaceStage.KeptNecklace);

                
                source.Inventory.RemoveQuantity("Zulera's Cursed Necklace", 1);
                
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

                var necklace = ItemFactory.Create("zulerasHeirloom");

                Logger.WithTopics(
                          [Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Entities.Dialog,
                          Topics.Entities.Quest])
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} has received {@ItemName} from a quest", source.Name, necklace.DisplayName);

                source.GiveItemOrSendToBank(necklace);

                break;
            }
            case "zulera_givenecklaceback":
            {
                if (!source.Inventory.HasCount("Zulera's Cursed Necklace", 1))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Looks like my necklace isn't in your inventory..");
                    Subject.Close(source);

                    return;
                }

                source.Inventory.RemoveQuantity("Zulera's Cursed Necklace", 1);

                if (stage == ManorNecklaceStage.ReturningNecklace)
                    source.Trackers.Enums.Set(ManorNecklaceStage.ReturnedNecklace);

                Logger.WithTopics(
                          [Topics.Entities.Aisling,
                          Topics.Entities.Experience,
                          Topics.Entities.Dialog,
                          Topics.Entities.Quest])
                      .WithProperty(source)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, 150000);

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
                    case ManorNecklaceStage.ReturningNecklace:
                    {
                        Subject.Reply(source, "Skip", "zulera_foundnecklace");

                        return;
                    }
                    case ManorNecklaceStage.AcceptedQuest:
                        Subject.Reply(source, "Come back when you've found the necklace, please!");

                        break;
                    case ManorNecklaceStage.SawNecklace:
                    {
                     
                        Subject.Reply(
                            source,
                            "You saw it!? Then ghost appeared? They must of taken it! Go back to that room and find it for me!");
                        source.Trackers.Enums.Set(ManorNecklaceStage.AcceptedQuest);
                        
                    }

                        break;
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