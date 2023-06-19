using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests;

public class SickChildScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public SickChildScript(Dialog subject, IItemFactory itemFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        MerchantFactory = merchantFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {

        var hasStage = source.Trackers.Enums.TryGetValue(out SickChildStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "paulin_initial":
                {
                    if ((!hasStage) || (stage == SickChildStage.None))
                    {
                        if (source.UserStatSheet.Level is <= 10 or >= 52)
                            return;
                        
                        Subject.Reply(source, "skip", "whiterose1-1start");
                        return;

                    }

                    if (stage == SickChildStage.WhiteRose)
                    {
                        if (source.Inventory.HasCount("black rose", 1))
                        {
                            Subject.Reply(source, "skip", "whiterose1-3black");
                            return;
                        }
                        Subject.Reply(source, "skip", "whiterose1-3white");
                        return;
                    }

                    if (stage == SickChildStage.WhiteRose1Turn)
                    {
                        Subject.Reply(source, "skip", "whiterosewaitstart");
                        return;
                    }


                    if (stage == SickChildStage.WhiteRose2)
                    {
                        if (source.Inventory.HasCount("black rose", 1))
                        {
                            Subject.Reply(source, "skip", "whiterose2-3black");
                            return;
                        }
                        Subject.Reply(source, "skip", "whiterose2-3white");
                        return;
                    }

                    if (stage == SickChildStage.WhiteRose2Turn)
                    {
                        Subject.Reply(source, "skip", "whiterose2waitstart");
                        return;
                    }

                    if (stage == SickChildStage.BlackRoseTurn)
                    {
                        Subject.Reply(source, "skip", "blackrose2-1start");
                        return;
                    }

                    if (stage == SickChildStage.GoldRose)
                    {
                        if (source.Inventory.HasCount("black rose", 1))
                        {
                            Subject.Reply(source, "skip", "goldrose1-3black");
                            return;
                        }
                        Subject.Reply(source, "skip", "goldrose1-3gold");
                        return;

                    }

                    if (stage == SickChildStage.SickChildComplete)
                    {

                        Subject.Reply(source, "Thank you again for helping cure the princess.");

                        return;

                    }

                    if (stage == SickChildStage.SickChildKilled)
                    {

                        Subject.Reply(source, "Long live the princess...");

                        return;

                    }
                }
                break;
            
            case "whiterose1-2":
                {
                    Subject.Reply(source, "Thank you! Please be careful on your journey. You can get to the Wilderness through Mileth, Abel, or Rucesion.");
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose);
                    source.SendOrangeBarMessage("Go find a White Rose in the Wilderness.");
                }
                break;

            case "whiterose1-3":
                {

                    if (!source.Inventory.RemoveQuantity("white rose", 1))
                    {
                        Subject.Reply(source, "Where is it?");
                        source.SendOrangeBarMessage("You do not have a rose.");

                        return;
                    }
                    ExperienceDistributionScript.GiveExp(source, 25000);
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose1Turn);
                    source.SendOrangeBarMessage("25000 Exp Rewarded!");
                    Subject.Reply(source, "Thank you! I need to get this to the healers right away. Please excuse me.","whiterosewait1");
                }

                break;

            case "whiterose2-2":
                {
                    Subject.Reply(source, "Thank you again. Please return as soon as you can.");
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose2);
                    source.SendOrangeBarMessage("Go find another White Rose in the Wilderness.");
                }

                break;
            case "whiterose2-3":

                if (!source.Inventory.RemoveQuantity("white rose", 1))
                {
                    Subject.Reply(source, "Where is it?");

                    var option = new DialogOption
                    {
                        DialogKey = "Close",
                        OptionText = "Be right back."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    source.SendOrangeBarMessage("You do not have a rose.");

                    return;
                }
                ExperienceDistributionScript.GiveExp(source, 50000);
                source.Trackers.Enums.Set(SickChildStage.WhiteRose2Turn);
                source.SendOrangeBarMessage("50000 Exp Rewarded!");
                Subject.Reply(source, "Thank you again! Please excuse me while I get this to the healers.", "whiterose2wait1");

                break;

            case "goldrose1-2":
                {
                    Subject.Reply(source, "Thank you again. Please return as soon as you can.");
                    source.Trackers.Enums.Set(SickChildStage.GoldRose);
                    source.SendOrangeBarMessage("Go find a Gold Rose in the Wilderness.");
                }
                break;

            case "goldrose1-3":
                {
                    if (!source.Inventory.RemoveQuantity("gold rose", 1))
                    {
                        Subject.Reply(source, "Where is it?");

                        var option = new DialogOption
                        {
                            DialogKey = "Close",
                            OptionText = "Be right back."
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);

                        source.SendOrangeBarMessage("You do not have a Gold Rose.");

                        return;
                    }
                    ExperienceDistributionScript.GiveExp(source, 100000);
                    source.TryGiveGold(20000);
                    source.TryGiveGamePoints(5);
                    source.SendOrangeBarMessage("5 Gamepoints, 20000 gold, and 100000 Exp Rewarded!");
                    source.Trackers.Enums.Set(SickChildStage.SickChildComplete);
                    source.Legend.AddOrAccumulate(
                          new LegendMark(
                              "Cured the Sick Child of Loures.",
                              "SickChild",
                              MarkIcon.Heart,
                              MarkColor.Blue,
                              1,
                              GameTime.Now));
                    Subject.Reply(source, "Thank you! With this we will be able to create the cure to save the princess! Please accept this reward in the name of the King.");

                    break;
                }
            
            case "blackrose2":

                {
                    if (!source.Inventory.Remove("black rose"))
                    {
                        Subject.Reply(source, "Where is it?");
                        source.SendOrangeBarMessage("You do not have a black rose.");
                        return;
                    }
                    
                    Subject.Reply(source, "skip", "blackrose2-1start");
                    source.Trackers.Enums.Set(SickChildStage.BlackRoseTurn);
                    break;
                }

            case "blackrose3":
                {
                    Subject.Reply(source, "Please leave.");
                    source.Trackers.Enums.Set(SickChildStage.SickChildKilled);
                    ExperienceDistributionScript.GiveExp(source, 125000);
                    source.TryGiveGamePoints(5);
                    source.SendOrangeBarMessage("5 Gamepoints and 125000 Exp Rewarded!");
                    source.SendOrangeBarMessage("The princess is dead...");
                    source.Legend.AddOrAccumulate(
                           new LegendMark(
                               "Killed the Sick Child of Loures.",
                               "SickChild",
                               MarkIcon.Warrior,
                               MarkColor.Orange,
                               1,
                               GameTime.Now));
                }
                break;

            case "whiterosewait1":
            {
                var blank = MerchantFactory.Create("blank_merchant", source.MapInstance, new Point(2, 2));

                Subject.DialogSource = blank;

            }

                break;
            
            case "whiterosewait5":
            {
                var paulin = MerchantFactory.Create("paulin", source.MapInstance, new Point(2, 2));

                Subject.DialogSource = paulin;

            }

                break;
            
            case "whiterose2wait1":
            {

                var blank = MerchantFactory.Create("blank_merchant", source.MapInstance, new Point(2, 2));

                Subject.DialogSource = blank;

            }

                break;
            
            case "whiterose2wait3":
            {
                var paulin = MerchantFactory.Create("paulin", source.MapInstance, new Point(2, 2));

                Subject.DialogSource = paulin;

            }

                break;
        }
    }
}