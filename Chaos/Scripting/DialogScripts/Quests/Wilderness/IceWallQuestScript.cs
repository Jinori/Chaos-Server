using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests;

public class IceWallQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly IItemFactory ItemFactory;

    public IceWallQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }


    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out IceWallQuest stage);


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "lilia_initial1":

                if (source.UserStatSheet.Level < 30)
                {
                    Subject.Reply(source, "skip", "lilia_initial");
                    return;
                }

                if (stage == IceWallQuest.Start)
                {
                    Subject.Reply(source, "skip", "lilia_initial2");
                    return;
                }
                
                if (stage == IceWallQuest.SampleComplete)
                {
                    Subject.Reply(source, "skip", "lilia_initial3");
                    return;
                }

                if (stage == IceWallQuest.KillWolves)
                {
                    Subject.Reply(source, "skip", "lilia_initial4");
                    return;
                }
                
                if (stage == IceWallQuest.WolfComplete)
                {
                    Subject.Reply(source, "skip", "lilia_initial5");
                    return;
                }

                if (stage == IceWallQuest.Charm)
                {
                    Subject.Reply(source, "skip", "lilia_initial6");
                    return;
                }
                
                if (stage == IceWallQuest.KillBoss)
                {
                    Subject.Reply(source, "skip", "lilia_initial7");
                    return;
                }

                if (stage == IceWallQuest.Complete)
                {
                    Subject.Reply(source, "skip", "lilia_initial8");
                    
                }

                break;

            case "lilia_quest2":
            {
                source.Trackers.Enums.Set(IceWallQuest.Start);
                source.SendOrangeBarMessage("Collect 3 ice samples.");
            }
                break;

            case "lilia_quest3":
            {
                if (stage == IceWallQuest.Start)
                {
                    if (!source.Inventory.HasCount("Ice Sample 1", 1) && !source.Inventory.HasCount("Ice Sample 2", 1) && !source.Inventory.HasCount("Ice Sample 3", 1))
                    {
                        source.SendOrangeBarMessage("You need to collect 3 samples.");
                        Subject.Close(source);

                        return;
                    }
                    source.Inventory.RemoveQuantity("Ice Sample 1", 1);
                    source.Inventory.RemoveQuantity("Ice Sample 2", 1);
                    source.Inventory.RemoveQuantity("Ice Sample 3", 1);
                    ExperienceDistributionScript.GiveExp(source, 75000);
                    source.Trackers.Enums.Set(IceWallQuest.SampleComplete);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and 75000 exp!");
                }
                

                break;
            }
            case "lilia_quest4":
            {
                source.Trackers.Enums.Set(IceWallQuest.KillWolves);
                source.SendOrangeBarMessage("Kill 10 Snow Wolves.");
            }
                break;
            
            case "lilia_quest5":
                if (!source.Trackers.Counters.TryGetValue("wolf", out var value) || (value < 10))
                {
                    Subject.Reply(source, "You need to kill more wolves.");
                    source.SendOrangeBarMessage("You need to kill 10 wolves.");

                    return;
                }

                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 150000);
                source.SendOrangeBarMessage("You receive five gamepoints and 150000 exp!");
                source.Trackers.Counters.Remove("wolf", out _);
                source.Trackers.Enums.Set(IceWallQuest.WolfComplete);
        
        break;

            case "lilia_quest8":
            {
                source.Trackers.Enums.Set(IceWallQuest.Charm);
                source.SendOrangeBarMessage("Bring Lilia a Ruby and Bronze Bar.");
            }
                break;

            case "lilia_quest9":
            {
                if (!source.Inventory.HasCount("Ruby", 1) && !source.Inventory.HasCount("Bronze Bar", 1))
                {
                    source.SendOrangeBarMessage("You are missing an item.");
                    Subject.Close(source);

                    return;
                }

                source.Inventory.RemoveQuantity("Ruby", 1);
                source.Inventory.RemoveQuantity("Bronze Bar", 1);
                source.TryGiveItems(ItemFactory.Create("charm"));
                ExperienceDistributionScript.GiveExp(source, 50000);
                source.Trackers.Enums.Set(IceWallQuest.KillBoss);
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive a Charm and 50000 exp!");

            }
                break;

            case "lilia_quest10":

                if (!source.Trackers.Counters.TryGetValue("abomination", out var val) || (val < 1))
                {
                    Subject.Reply(source, "You haven't defeated the Abomination yet.");
                    source.SendOrangeBarMessage("You haven't defeated the Abomination yet.");

                    return;
                }

                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 500000);
                source.SendOrangeBarMessage("You receive five gamepoints and 500000 exp!");
                source.Trackers.Counters.Remove("abomination", out _);
                source.Trackers.Enums.Set(IceWallQuest.Complete);

                break;
            }
        }
    
    }