using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.HobbyQuest;

public class FishingQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public FishingQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    private int CalculateLevel99Exp(int marks)
        => marks switch
        {
            250   => 1000000,
            500   => 5000000,
            800   => 10000000,
            1500  => 20000000,
            3000  => 25000000,
            5000  => 30000000,
            7500  => 45000000,
            10000 => 50000000,
            12500 => 50000000,
            15000 => 75000000,
            20000 => 75000000,
            25000 => 100000000,
            30000 => 125000000,
            40000 => 150000000,
            50000 => 200000000,
            _     => 0
        };

    public override void OnDisplaying(Aisling source)
    {
        var sourceFishingMarks = source.Legend.GetCount("fish");
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);

        var markMilestones = new[]
        {
            250,
            500,
            800,
            1500,
            3000,
            5000,
            7500,
            10000,
            12500,
            15000,
            20000,
            25000,
            30000,
            40000,
            50000
        };

        var goldRewards = new[]
        {
            10000,
            25000,
            50000,
            100000,
            150000,
            200000,
            300000,
            500000,
            750000,
            1000000,
            2000000,
            2500000,
            5000000,
            7500000,
            10000000
        };

        var expMultipliers = new[]
        {
            0.10,
            0.10,
            0.15,
            0.20,
            0.25,
            0.25,
            0.30,
            0.30,
            0.40,
            0.50,
            0.50,
            0.50,
            0.50,
            0.50,
            0.50
        };

        var pointRewards = new[]
        {
            5,
            5,
            5,
            5,
            10,
            10,
            10,
            10,
            15,
            15,
            20,
            25,
            50,
            75,
            100
        };

        var flags = new[]
        {
            FishingQuest.Reached250,
            FishingQuest.Reached500,
            FishingQuest.Reached800,
            FishingQuest.Reached1500,
            FishingQuest.Reached3000,
            FishingQuest.Reached5000,
            FishingQuest.Reached7500,
            FishingQuest.Reached10000,
            FishingQuest.Reached12500,
            FishingQuest.Reached15000,
            FishingQuest.Reached20000,
            FishingQuest.Reached25000,
            FishingQuest.Reached30000,
            FishingQuest.Reached40000,
            FishingQuest.CompletedFishing
        };

        var itemRewards = new[]
        {
            null!,
            null!,
            null!,
            null!,
            "goodfishingpole",
            null!,
            null!,
            null!,
            null!,
            "greatfishingpole",
            null!,
            null!,
            null!,
            null!,
            "grandfishingpole"
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "kamel_initial":
                if (source.Trackers.Flags.HasFlag(Hobbies.Fishing) && (sourceFishingMarks >= 250))
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "fishingquest_initial",
                            OptionText = "I am an Experienced Fisher now."
                        });

                break;

            case "fishingquest_initial":
                var hasReward = false;

                for (var i = 0; i < markMilestones.Length; i++)
                    if ((sourceFishingMarks >= markMilestones[i]) && !source.Trackers.Flags.HasFlag(flags[i]))
                    {
                        source.Trackers.Flags.AddFlag(flags[i]);

                        var exp = source.StatSheet.Level != 99
                            ? Convert.ToInt32(expMultipliers[i] * tnl)
                            : CalculateLevel99Exp(markMilestones[i]);
                        ExperienceDistributionScript.GiveExp(source, exp);
                        source.GiveGoldOrSendToBank(goldRewards[i]);
                        source.TryGiveGamePoints(pointRewards[i]);

                        if (!string.IsNullOrEmpty(itemRewards[i]))
                        {
                            var item = ItemFactory.Create(itemRewards[i]);
                            source.GiveItemOrSendToBank(item);
                            source.SendOrangeBarMessage($"Kamel hands you a new {itemRewards[i]}!");
                        }

                        hasReward = true;
                    }

                if (hasReward)
                {
                    if (source.Trackers.Flags.HasFlag(FishingQuest.CompletedFishing))
                        Subject.Reply(source, "This is the end of the road, I have nothing more to give you. You are a true fisherman.");
                    else
                        Subject.Reply(
                            source,
                            "You've done excellent getting those fish! I can't say I'm not proud. Here's something for your troubles. Come back anytime!",
                            "kamel_initial");
                } else
                    Subject.Reply(source, "You haven't gotten enough experience fishing yet, Aisling. Keep on fishing!", "kamel_initial");

                break;
        }
    }
}