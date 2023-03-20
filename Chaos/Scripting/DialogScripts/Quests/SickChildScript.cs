using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Quests;

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
                        Subject.Text = "Excuse me, aisling! I require your assistance. The princess is gravely ill, and we need to find a cure quickly. I've been informed that a white rose is said to have magical healing properties that could help her. Unfortunately, I am unable to leave my post. Will you aid us in finding this flower?";

                        var option = new DialogOption
                        {
                            DialogKey = "Whiterose1-1",
                            OptionText = "Where can I find this flower?"
                        };

                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        return;

                    }

                    if (stage == SickChildStage.WhiteRose)
                    {
                        Subject.Text = "Did you find a white rose?";

                        var option = new DialogOption
                        {
                            DialogKey = "whiterose1-3",
                            OptionText = "Yes. Here you go."
                        };
                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        if (source.Inventory.HasCount("black rose", 1))
                        {
                            var option1 = new DialogOption
                            {
                                DialogKey = "blackrose1",
                                OptionText = "I found a black rose. Will this work?"
                            };
                            if (!Subject.HasOption(option1))
                                Subject.Options.Insert(1, option1);
                        }
                        var option2 = new DialogOption
                        {
                            DialogKey = "close",
                            OptionText = "No, Not yet."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);

                    }

                    if (stage == SickChildStage.WhiteRose1Turn)
                    {
                        Subject.Text = "Paulin hasn't returned yet.";

                        var option1 = new DialogOption
                        {
                            DialogKey = "whiterosewait1",
                            OptionText = "Wait for Paulin to return."
                        };

                        if (!Subject.HasOption(option1))
                            Subject.Options.Add(option1);

                        var option2 = new DialogOption
                        {
                            DialogKey = "close",
                            OptionText = "Leave."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);
                    }


                    if (stage == SickChildStage.WhiteRose2)
                    {
                        Subject.Text = "Did you find another white rose?";

                        var option = new DialogOption
                        {
                            DialogKey = "whiterose2-3",
                            OptionText = "Yes. Here you go."
                        };
                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        if (source.Inventory.HasCount("black rose", 1))
                        {
                            var option1 = new DialogOption
                            {
                                DialogKey = "blackrose1",
                                OptionText = "I found a black rose. Will this work?"
                            };
                            if (!Subject.HasOption(option1))
                                Subject.Options.Insert(1, option1);
                        }
                        var option2 = new DialogOption
                        {
                            DialogKey = "close",
                            OptionText = "No, Not yet."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);

                    }

                    if (stage == SickChildStage.WhiteRose2Turn)
                    {
                        Subject.Text = "Paulin hasn't returned yet.";

                        var option1 = new DialogOption
                        {
                            DialogKey = "whiterose2wait1",
                            OptionText = "Wait for Paulin to return."
                        };

                        if (!Subject.HasOption(option1))
                            Subject.Options.Add(option1);

                        var option2 = new DialogOption
                        {
                            DialogKey = "close",
                            OptionText = "Leave."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);
                    }

                    if (stage == SickChildStage.BlackRoseTurn)
                    {
                        Subject.Text = "What have we done...";

                        var option1 = new DialogOption
                        {
                            DialogKey = "blackrose2-1",
                            OptionText = "What happened?"
                        };

                        if (!Subject.HasOption(option1))
                            Subject.Options.Add(option1);

                        var option2 = new DialogOption
                        {
                            DialogKey = "close",
                            OptionText = "Leave."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);
                    }

                    if (stage == SickChildStage.GoldRose)
                    {
                        Subject.Text = "Were you able to find a Gold Rose?";

                        var option = new DialogOption
                        {
                            DialogKey = "goldrose1-3",
                            OptionText = "Yes. Here it is."
                        };

                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        if (source.Inventory.HasCount("black rose", 1))
                        {
                            var option1 = new DialogOption
                            {
                                DialogKey = "blackrose1",
                                OptionText = "I found a black rose. Will this work?"
                            };
                            if (!Subject.HasOption(option1))
                                Subject.Options.Insert(1, option1);
                        }
                        var option2 = new DialogOption
                        {
                            DialogKey = "close",
                            OptionText = "No, Not yet."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);

                    }

                    if (stage == SickChildStage.SickChildComplete)
                    {

                        Subject.Text = "Thank you again for helping cure the princess.";

                        return;

                    }

                    if (stage == SickChildStage.SickChildKilled)
                    {

                        Subject.Text = "Long live the princess...";

                        return;

                    }
                }
                break;

            case "whiterose1-1":
                {
                    Subject.Text = "According to legend, the white rose only blooms in a gardens deep within the Wilderness. Are you still willing to help?";

                    var option = new DialogOption
                    {
                        DialogKey = "whiterose1-2",
                        OptionText = "Yes, I'll help."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }
                break;

            case "whiterose1-2":
                {
                    Subject.Text = "Thank you! Please be careful on your journey. You can get to the Wilderness through Mileth, Abel, or Rucesion.";
                    Subject.Type = MenuOrDialogType.Normal;
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose);
                    source.SendOrangeBarMessage("Go find a White Rose in the Wilderness.");
                }
                break;

            case "whiterose1-3":
                {

                    if (!source.Inventory.Remove("white rose"))
                    {
                        Subject.Text = "Where is it?";

                        var option = new DialogOption
                        {
                            DialogKey = "Close",
                            OptionText = "Be right back."
                        };

                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        source.SendOrangeBarMessage("You do not have a rose.");

                        return;
                    }
                    ExperienceDistributionScript.GiveExp(source, 25000);
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose1Turn);
                    source.SendOrangeBarMessage("25000 Exp Rewarded!");
                    Subject.Text = "Thank you! I need to get this to the healers right away. Please excuse me.";
                    Subject.NextDialogKey = "whiterosewait1";
                }

                break;

            case "whiterose2-1":
                {

                    Subject.Text = "It seems to have helped a little but I'm afraid it's not enough. Would you be willing to find another rose?";

                    var option = new DialogOption
                    {
                        DialogKey = "whiterose2-2",
                        OptionText = "Of course. Anything for the princess."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }

                break;

            case "whiterose2-2":
                {
                    Subject.Text = "Thank you again. Please return as soon as you can.";
                    Subject.Type = MenuOrDialogType.Normal;
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose2);
                    source.SendOrangeBarMessage("Go find another White Rose in the Wilderness.");
                }

                break;

            case "whiterose2-3":

                if (!source.Inventory.Remove("white rose"))
                {
                    Subject.Text = "Where is it?";

                    var option = new DialogOption
                    {
                        DialogKey = "Close",
                        OptionText = "Be right back."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    source.SendOrangeBarMessage("You do not have a rose.");

                    return;
                }
                ExperienceDistributionScript.GiveExp(source, 50000);
                source.Trackers.Enums.Set(SickChildStage.WhiteRose2Turn);
                source.SendOrangeBarMessage("50000 Exp Rewarded!");
                Subject.Text = "Thank you again! Please excuse me while I get this to the healers.";
                Subject.NextDialogKey = "whiterose2wait1";

                break;

            case "goldrose1-1":
                {

                    Subject.Text = "To finish the cure we need a Gold Rose. Gold roses are more rare then the white roses are but they can be found around the same area. Please search all the gardens in the Wilderness and find us a gold rose.";

                    var option = new DialogOption
                    {
                        DialogKey = "goldrose1-2",
                        OptionText = "Got it. Be right back."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }

                break;

            case "goldrose1-2":
                {
                    Subject.Text = "Thank you again. Please return as soon as you can.";
                    Subject.Type = MenuOrDialogType.Normal;
                    source.Trackers.Enums.Set(SickChildStage.GoldRose);
                    source.SendOrangeBarMessage("Go find a Gold Rose in the Wilderness.");
                }
                break;

            case "goldrose1-3":
                {
                    if (!source.Inventory.Remove("gold rose"))
                    {
                        Subject.Text = "Where is it?";

                        var option = new DialogOption
                        {
                            DialogKey = "Close",
                            OptionText = "Be right back."
                        };

                        if (!Subject.HasOption(option))
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
                    Subject.Text = "Thank you! With this we will be able to create the cure to save the princess! Please accept this reward in the name of the King.";
                    Subject.NextDialogKey = "close";

                    break;
                }

            case "blackrose1":
                {
                    Subject.Text = "Hmm this looks like the rose needed but tainted. I'm not sure if this would help or not, but at this point we don't have a choice. The princess cannot wait any longer.";

                    var option = new DialogOption
                    {
                        DialogKey = "blackrose2",
                        OptionText = "Let's give it a shot."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    var option2 = new DialogOption
                    {
                        DialogKey = "close",
                        OptionText = "Nevermind."
                    };

                    if (!Subject.HasOption(option2))
                        Subject.Options.Add(option2);
                }
                break;

            case "blackrose2":

                {
                    if (!source.Inventory.Remove("black rose"))
                    {
                        Subject.Text = "Where is it?";

                        var option = new DialogOption
                        {
                            DialogKey = "Close",
                            OptionText = "Be right back."
                        };

                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        source.SendOrangeBarMessage("You do not have a black rose.");

                        return;
                    }
                    
                    Subject.Text = "This was a mistake. Oh god what have we done!";
                    source.Trackers.Enums.Set(SickChildStage.BlackRoseTurn);

                    var option2 = new DialogOption
                    {
                        DialogKey = "blackrose2-1",
                        OptionText = "What happened?"
                    };

                    if (!Subject.HasOption(option2))
                        Subject.Options.Add(option2);
                }
                

                break;

            case "blackrose3":
                {
                    Subject.Text = "Please leave.";
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

                Subject.SourceEntity = blank;

            }

                break;
            
            case "whiterosewait5":
            {
                var paulin = MerchantFactory.Create("paulin", source.MapInstance, new Point(2, 2));

                Subject.SourceEntity = paulin;

            }

                break;
            
            case "whiterose2wait1":
            {
                var blank = MerchantFactory.Create("blank_merchant", source.MapInstance, new Point(2, 2));

                Subject.SourceEntity = blank;

            }

                break;
            
            case "whiterose2wait3":
            {
                var paulin = MerchantFactory.Create("paulin", source.MapInstance, new Point(2, 2));

                Subject.SourceEntity = paulin;

            }

                break;
        }
    }
}