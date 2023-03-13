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
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Quests;

public class SickChildScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public SickChildScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
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
                        {
                            return;
                        }

                        var option = new DialogOption
                        {
                            DialogKey = "paulin_quest1",
                            OptionText = "What can I do?"
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
                            DialogKey = "whiterose1-1",
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
                            DialogKey = "whiterose1_no",
                            OptionText = "No, Not yet."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);

                    }


                    if (stage == SickChildStage.WhiteRose2)
                    {
                        Subject.Text = "Did you find another white rose?";

                        var option = new DialogOption
                        {
                            DialogKey = "whiterose2-1",
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
                            DialogKey = "whiterose1_no",
                            OptionText = "No, Not yet."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);

                    }

                    if (stage == SickChildStage.GoldRose)
                    {
                        Subject.Text = "Do you have the Gold Rose?";

                        var option = new DialogOption
                        {
                            DialogKey = "goldrose1",
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
                            DialogKey = "whiterose1_no",
                            OptionText = "No, Not yet."
                        };

                        if (!Subject.HasOption(option2))
                            Subject.Options.Add(option2);

                    }

                    if ((!hasStage) || (stage == SickChildStage.SickChildComplete))
                    {

                        Subject.Text = "Thank you again for helping cure the princess";

                        return;

                    }

                    if ((!hasStage) || (stage == SickChildStage.SickChildKilled))
                    {

                        Subject.Text = "Long live the princess...";

                        return;

                    }
                }
                break;

            case "paulin_quest2":
                if (!hasStage || (stage == SickChildStage.None))
                {
                    source.Trackers.Enums.Set(SickChildStage.WhiteRose);
                }
                break;

            case "whiterose1-1":
                {
                    if (stage == SickChildStage.WhiteRose)
                    {

                        if (!source.Inventory.HasCount("white rose", 1))
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
                        source.Inventory.RemoveQuantity("white rose", 1);
                        ExperienceDistributionScript.GiveExp(source, 50000);
                        source.Trackers.Enums.Set(SickChildStage.WhiteRose2);
                    }
                }
                break;

            case "whiterose2-1":
                {
                    if (stage == SickChildStage.WhiteRose2)
                    {

                        if (!source.Inventory.HasCount("white rose", 1))
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
                        source.Inventory.RemoveQuantity("white rose", 1);
                        ExperienceDistributionScript.GiveExp(source, 50000);
                        source.Trackers.Enums.Set(SickChildStage.GoldRose);
                    }
                }
                break;

            case "goldrose1":
                {
                    if (stage == SickChildStage.GoldRose)
                    {

                        if (!source.Inventory.HasCount("gold rose", 1))
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
                        source.Inventory.RemoveQuantity("gold rose", 1);
                    }
                }
                break;

            case "goldrose2":

                if (!hasStage || (stage == SickChildStage.GoldRose))
                {
                    source.Trackers.Enums.Set(SickChildStage.SickChildComplete);
                    ExperienceDistributionScript.GiveExp(source, 100000);
                    source.TryGiveGold(25000);
                }
                break;

            case "blackrose2":
                {
                    if (stage == SickChildStage.WhiteRose || stage == SickChildStage.WhiteRose2 || stage == SickChildStage.GoldRose)
                    {
                        source.Trackers.Enums.Set(SickChildStage.BlackRose);
                    }
                    
                        if (!source.Inventory.HasCount("black rose", 1))
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
                    
                    break;

                }
            case "blackrose6":

                if (!hasStage || (stage == SickChildStage.BlackRose))
                {
                    source.Trackers.Enums.Set(SickChildStage.SickChildKilled);
                    ExperienceDistributionScript.GiveExp(source, 250000);
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
        }
    }
}