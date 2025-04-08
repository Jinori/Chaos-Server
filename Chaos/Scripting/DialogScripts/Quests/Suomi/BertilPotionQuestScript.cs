using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Suomi;

public class BertilPotionQuestScript(Dialog subject, ILogger<BertilPotionQuestScript> logger) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private const string COOLDOWN_EVENT_NAME = "bertilpotioncd";
    private const string POTION_NAME = "Health Potion";
    private const int REQUIRED_LEVEL = 11;
    private const int EXP_REWARD = 15000;
    private const int GAME_POINT_REWARD = 5;
    private const int LEGEND_MARK_CHANCE = 8;

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out BertilPotion stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bertil_initial":
                HandleInitialDisplay(source);
                break;

            case "bertilpotion_initial":
                HandleCooldownOrReturnFlow(source, stage);
                break;

            case "bertilpotion_start2":
                StartQuest(source);
                break;

            case "bertilpotion_turnin":
                TurnInPotion(source, stage);
                break;
        }
    }

    private void HandleInitialDisplay(Aisling source)
    {
        if (source.UserStatSheet.Level < REQUIRED_LEVEL)
            return;

        const string OPTION_TEXT = "Red Potion";

        if (!Subject.HasOption(OPTION_TEXT))
        {
            Subject.Options.Insert(0, new DialogOption
            {
                DialogKey = "bertilpotion_initial",
                OptionText = OPTION_TEXT
            });
        }
    }

    private void HandleCooldownOrReturnFlow(Aisling source, BertilPotion stage)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent(COOLDOWN_EVENT_NAME, out var cooldownTime))
        {
            Subject.Reply(source,
                $"That health potion is being put to excellent use. I am studying its characteristics to teach my students that mere potions will never match their magic. Bring me another in (({cooldownTime.Remaining.ToReadableString()}))");
            return;
        }

        if (stage == BertilPotion.StartedQuest)
        {
            Subject.Reply(source, "Skip", "bertilpotion_return");
        }
    }

    private void StartQuest(Aisling source)
    {
        source.Trackers.Enums.Set(BertilPotion.StartedQuest);
        source.SendOrangeBarMessage("Bring Bertil a Health Potion.");
    }

    private void TurnInPotion(Aisling source, BertilPotion stage)
    {
        if (stage != BertilPotion.StartedQuest)
            return;

        var hasPotion = source.Inventory.HasCount(POTION_NAME, 1);

        if (!hasPotion)
        {
            Subject.Reply(source,
                "That's not what I need. I am looking for a red potion, some may call it the health potion. True healing comes from magic though.");
            return;
        }

        source.Inventory.RemoveQuantity(POTION_NAME, 1, out _);
        source.Trackers.Enums.Set(BertilPotion.None);
        source.Trackers.TimedEvents.AddEvent(COOLDOWN_EVENT_NAME, TimeSpan.FromHours(22), true);

        LogReward(source);
        ExperienceDistributionScript.GiveExp(source, EXP_REWARD);
        source.TryGiveGamePoints(GAME_POINT_REWARD);

        TryGrantLegendMark(source);
    }

    private void LogReward(Aisling source) =>
        logger.WithTopics(
                  Topics.Entities.Aisling,
                  Topics.Entities.Gold,
                  Topics.Entities.Experience,
                  Topics.Entities.Dialog,
                  Topics.Entities.Quest)
              .WithProperty(source)
              .WithProperty(Subject)
              .LogInformation("{@AislingName} has received {@ExpAmount} exp from Bertil Potion Quest", source.Name, EXP_REWARD);

    private void TryGrantLegendMark(Aisling source)
    {
        if (!IntegerRandomizer.RollChance(LEGEND_MARK_CHANCE))
            return;

        source.Legend.AddOrAccumulate(new LegendMark(
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
