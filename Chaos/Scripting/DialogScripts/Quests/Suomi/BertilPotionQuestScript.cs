using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Mileth;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Suomi;

public class BertilPotionQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SpareAStickScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public BertilPotionQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out BertilPotion stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bertil_initial":
            {
                if (source.UserStatSheet.Level < 11)
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "bertilpotion_initial",
                    OptionText = "Red Potion"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "bertilpotion_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("bertilpotioncd", out var cdtime))
                {
                    Subject.Reply(source, $"That health potion is being put to excellent use. I am studying its characteristics to teach my students that mere potions will never match their magic. Bring me another in (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                if (hasStage && stage == BertilPotion.StartedQuest)
                {
                    Subject.Reply(source, "Skip", "bertilpotion_return");
                }
                break;
            }

            case "bertilpotion_start2":
            {
                source.Trackers.Enums.Set(BertilPotion.StartedQuest);
                source.SendOrangeBarMessage("Bring Bertil a Health Potion.");
                break;
            }

            case "bertilpotion_turnin":
            {
                var hasRequiredPotion = source.Inventory.HasCount("Health Potion", 1);


                if (hasStage && stage == BertilPotion.StartedQuest)
                {
                    if (hasRequiredPotion)
                    {
                        source.Inventory.RemoveQuantity("Health Potion", 1, out _);
                        source.Trackers.Enums.Set(BertilPotion.None);
                        source.Trackers.TimedEvents.AddEvent("bertilpotioncd", TimeSpan.FromHours(24), true);

                        Logger.WithTopics(
                                Topics.Entities.Aisling,
                                Topics.Entities.Gold,
                                Topics.Entities.Experience,
                                Topics.Entities.Dialog,
                                Topics.Entities.Quest)
                            .WithProperty(source)
                            .WithProperty(Subject)
                            .LogInformation(
                                "{@AislingName} has received {@ExpAmount} exp from a quest",
                                source.Name,
                                20000);

                        ExperienceDistributionScript.GiveExp(source, 15000);
                        source.TryGiveGamePoints(5);

                        if (IntegerRandomizer.RollChance(8))
                        {
                            source.Legend.AddOrAccumulate(
                                new LegendMark(
                                    "Loved by Suomi Mundanes",
                                    "suomiLoved",
                                    MarkIcon.Heart,
                                    MarkColor.Blue,
                                    1,
                                    GameTime.Now));

                            source.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                                "You received a unique legend mark!");
                        }
                    }

                    if (!hasRequiredPotion)
                    {
                        
                        Subject.Reply(source,
                            $"That's not what I need. I am looking for a red potion, some may call it the health potion. True healing comes from magic though.");
                    }
                }

                break;
            }
        }
    }
}