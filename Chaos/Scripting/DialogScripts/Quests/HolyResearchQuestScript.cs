using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests;

public class HolyResearchQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HolyResearchQuestScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out HolyResearchStage stage);

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
                    if (source.Trackers.TimedEvents.HasActiveEvent("HolyResearchCd", out var timedEvent))
                    {
                        Subject.Reply(source, $"I don't need any more right now, please come back later. (({timedEvent.Remaining.ToReadableString()
                        }))");

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
                        DialogKey = "HolyResearch_use",
                        OptionText = "What do you use these for?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(2, option2);
                }

                if (stage == HolyResearchStage.StartedRawHoney)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedrawhoney",
                        OptionText = "I have your Raw Honey here."
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "HolyResearch_where",
                        OptionText = "Where can I find that?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(1, option2);

                    return;
                }

                if (stage == HolyResearchStage.StartedRawWax)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedrawwax",
                        OptionText = "I have your Raw Wax here."
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "HolyResearch_where",
                        OptionText = "Where can I find that?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(1, option2);

                    return;
                }

                if (stage == HolyResearchStage.StartedRoyalWax)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "HolyResearch_startedroyalwax",
                        OptionText = "I have your Royal Wax here."
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "HolyResearch_where",
                        OptionText = "Where can I find that?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(1, option2);
                }

                break;

            case "holyresearch_yes":
                if (!hasStage || (stage == HolyResearchStage.None))
                {
                    var randomHolyResearchStage = new[]
                    {
                        HolyResearchStage.StartedRawHoney, HolyResearchStage.StartedRawWax, HolyResearchStage.StartedRoyalWax
                    }.PickRandom();

                    source.Trackers.Enums.Set(randomHolyResearchStage);

                    switch (randomHolyResearchStage)
                    {
                        case HolyResearchStage.StartedRawHoney:
                        {
                            Subject.Reply(source, "Thank you so much Aisling, bring me one raw honey.");
                        }

                            break;

                        case HolyResearchStage.StartedRawWax:
                        {
                            Subject.Reply(source, "Thank you so much Aisling, bring me one raw wax.");
                        }

                            break;

                        case HolyResearchStage.StartedRoyalWax:
                        {
                            Subject.Reply(source, "Thank you so much Aisling, bring me one royal wax.");
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
                    source.Trackers.Enums.Set(HolyResearchStage.None);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and 2000 exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("HolyResearchCd", TimeSpan.FromHours(1), true);
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
                    source.Trackers.Enums.Set(HolyResearchStage.None);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and 2000 exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("HolyResearchCd", TimeSpan.FromHours(1), true);
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
                    source.Trackers.Enums.Set(HolyResearchStage.None);
                    source.TryGiveGamePoints(5);
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive five gamepoints and 2000 exp!");
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("HolyResearchCd", TimeSpan.FromHours(1), true);
                }

                break;
        }
    }
}