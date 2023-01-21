using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts.Mileth;

public class HolyResearchQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public HolyResearchQuestScript(
        Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {

        var hasStage = source.Enums.TryGetValue(out HolyResearchStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "berteli_initial":
            {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_initial",
                        OptionText = "Holy Research"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
            }
                

                break;

            case "holyresearch_initial":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    
                    if (source.TimedEvents.TryGetNearestToCompletion(TimedEvent.TimedEventId.HolyResearchCd, out var timedEvent))
                    {
                        Subject.Text = $"I don't need any more right now, please come back later. (({timedEvent.Remaining.ToReadableString()}))";
                        return;
                    }
                    
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_yes",
                        OptionText = "Yes I will help you, what do you need?"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "HolyResearch_no",
                        OptionText = "I have better things to do."
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "HolyResearch_where",
                        OptionText = "Where can I find that?"
                    };

                    var option3 = new DialogOption
                    {
                        DialogKey = "HolyResearch_use",
                        OptionText = "What do you use these for?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);
                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(2, option2);
                    if (!Subject.HasOption(option3))
                        Subject.Options.Insert(3, option3);
                }


                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedrawhoney",
                        OptionText = "I have your Raw Honey here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == HolyResearchStage.StartedRawWax)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedrawwax",
                        OptionText = "I have your Raw Wax here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == HolyResearchStage.StartedRoyalWax)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedroyalwax",
                        OptionText = "I have your Royal Wax here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "holyresearch_yes":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    var randomHolyResearchStage = new[]
                    {
                        HolyResearchStage.StartedRawHoney, HolyResearchStage.StartedRawWax, HolyResearchStage.StartedRoyalWax
                    }.PickRandom();

                    source.Enums.Set(randomHolyResearchStage);

                    switch (randomHolyResearchStage)
                    {
                        case HolyResearchStage.StartedRawHoney:
                        {
                            Subject.Text = "Thank you so much Aisling, bring me one raw honey.";
                        }

                            break;

                        case HolyResearchStage.StartedRawWax:
                        {
                            Subject.Text = "Thank you so much Aisling, bring me one raw wax.";
                        }

                            break;

                        case HolyResearchStage.StartedRoyalWax:
                        {
                            Subject.Text = "Thank you so much Aisling, bring me one royal wax.";
                        }

                            break;
                    }

                }

                break;

            case "holyresearch_startedrawhoney":
                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    if (!source.Inventory.HasCount("raw honey", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("raw honey", 1);
                    ExperienceDistributionScript.GiveExp(source, 2000);
                    source.Enums.Set(HolyResearchStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.HolyResearchCd, TimeSpan.FromHours(1), true);
                    
                }

                break;

            case "holyresearch_startedrawwax":
                if (stage == HolyResearchStage.StartedRawWax)
                {
                    if (!source.Inventory.HasCount("raw wax", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("raw wax", 1);
                    ExperienceDistributionScript.GiveExp(source, 2000);
                    source.Enums.Set(HolyResearchStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.HolyResearchCd, TimeSpan.FromHours(1), true);
                }

                break;

            case "holyresearch_startedroyalwax":
                if (stage == HolyResearchStage.StartedRoyalWax)
                {
                    if (!source.Inventory.HasCount("royal wax", 1))
                    {
                        source.SendOrangeBarMessage("Berteli sighs and looks away. You don't have any.");
                        Subject.Close(source);

                        return;
                    }

                    source.Inventory.RemoveQuantity("royal wax", 1);
                    ExperienceDistributionScript.GiveExp(source, 2000);
                    source.Enums.Set(HolyResearchStage.None);
                    Subject.Close(source);
                    source.TimedEvents.AddEvent(TimedEvent.TimedEventId.HolyResearchCd, TimeSpan.FromHours(1), true);
                }

                break;
        }
    }
}