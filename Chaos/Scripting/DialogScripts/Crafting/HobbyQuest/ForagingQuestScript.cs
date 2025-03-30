using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.HobbyQuest;

public class ForagingQuestScript : DialogScriptBase
{
    private static readonly (int Marks, int Gold, int ExpPercent, int Exp99, int GamePoints, string? ItemKey, string? Message, ForagingQuest
        Flag)[] RewardStages =
        {
            (250, 10000, 10, 1000000, 5, null, null, ForagingQuest.Reached250),
            (500, 25000, 10, 5000000, 5, null, null, ForagingQuest.Reached500),
            (800, 50000, 15, 10000000, 5, null, null, ForagingQuest.Reached800),
            (1500, 100000, 20, 20000000, 5, null, null, ForagingQuest.Reached1500),
            (3000, 150000, 25, 25000000, 10, "ironglove", "Goran hands you a new cloth glove!", ForagingQuest.Reached3000),
            (5000, 200000, 25, 30000000, 10, null, null, ForagingQuest.Reached5000),
            (7500, 300000, 30, 45000000, 10, null, null, ForagingQuest.Reached7500),
            (10000, 500000, 35, 50000000, 10, null, null, ForagingQuest.Reached10000),
            (12500, 750000, 40, 50000000, 15, null, null, ForagingQuest.Reached12500),
            (15000, 1000000, 45, 75000000, 15, "mythrilglove", "Goran hands you a new cloth glove!", ForagingQuest.Reached15000),
            (20000, 2000000, 50, 75000000, 20, null, null, ForagingQuest.Reached20000),
            (25000, 2500000, 50, 100000000, 25, null, null, ForagingQuest.Reached25000),
            (30000, 5000000, 50, 125000000, 50, null, null, ForagingQuest.Reached30000),
            (40000, 7500000, 50, 150000000, 75, null, null, ForagingQuest.Reached40000),
            (50000, 10000000, 50, 200000000, 100, "hybrasylglove", "Goran hands you a new cloth glove!", ForagingQuest.Reached50000)
        };

    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public ForagingQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        var sourceForagingMarks = source.Legend.GetCount("forage");
        var hasRewarded = false;

        // Display initial dialog based on conditions
        if (Subject.Template.TemplateKey.Equals("goran_initial", StringComparison.OrdinalIgnoreCase)
            && source.Trackers.Flags.HasFlag(Hobbies.Foraging)
            && (sourceForagingMarks >= RewardStages[0].Marks))
        {
            Subject.Options.Insert(
                0,
                new DialogOption
                {
                    DialogKey = "foragingquest_initial",
                    OptionText = "I am an Experienced Forager now."
                });

            return;
        }

        // Handle the initial dialog for foraging quest
        if (Subject.Template.TemplateKey.Equals("foragingquest_initial", StringComparison.OrdinalIgnoreCase))
        {
            if (source.Trackers.Flags.HasFlag(ForagingQuest.CompletedForaging))
            {
                Subject.Reply(source, "The greenery fears your presence now. It tells me so... *goran chuckles*", "goran_initial");

                return;
            }

            foreach (var stage in RewardStages)

                // Check if the player has reached a reward stage and hasn't been rewarded for it yet
                if ((sourceForagingMarks >= stage.Marks) && !source.Trackers.Flags.HasFlag(stage.Flag))
                {
                    // Apply the flag to mark that the reward for this stage has been given
                    source.Trackers.Flags.AddFlag(stage.Flag);

                    // Calculate experience based on player level
                    var exp = source.StatSheet.Level != 99
                        ? stage.ExpPercent * LevelUpFormulae.Default.CalculateTnl(source) / 100
                        : stage.Exp99;

                    // Apply rewards
                    ExperienceDistributionScript.GiveExp(source, exp);
                    source.GiveGoldOrSendToBank(stage.Gold);
                    source.TryGiveGamePoints(stage.GamePoints);

                    if (!string.IsNullOrEmpty(stage.ItemKey))
                    {
                        var item = ItemFactory.Create(stage.ItemKey);
                        source.GiveItemOrSendToBank(item);
                    }

                    if (!string.IsNullOrEmpty(stage.Message))
                        source.SendOrangeBarMessage(stage.Message);

                    hasRewarded = true;
                }

            if (hasRewarded)
                Subject.Reply(
                    source,
                    "Not many keep foraging like you, Aisling. You're getting better with each day. I must reward you with a little something.",
                    "goran_initial");
            else
                Subject.Reply(
                    source,
                    "Your skill hasn't improved since the last time I saw you, Aisling. Keep on foraging!",
                    "goran_initial");
        }
    }
}