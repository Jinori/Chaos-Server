using Chaos.Common.Utilities;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Thanksgiving;

public class ThanksgivingTreasureHuntScript(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private const int REQUIRED_HUNTS = 5;
    private readonly IItemFactory ItemFactory = itemFactory;

    // Level-based item lists for each level bracket
    private readonly Dictionary<string, List<string>> ItemLists = new()
    {
        {
            "1-11", new List<string>
            {
                "spidereye",
                "apple",
                "spidersilk",
                "centipedegland",
                "beesting",
                "vipergland",
                "carrot",
                "mantiseye",
                "rawwax",
                "rawhoney",
                "royalwax"
            }
        },
        {
            "11-41", new List<string>
            {
                "batwing",
                "scorpionsting",
                "scorpiontail",
                "giantbatwing",
                "dendronflower",
                "mythichoney",
                "wolfskin",
                "trentroot",
                "giantantwing",
                "silverwolfmanehair",
                "koboldskull",
                "wolfteeth",
                "wolflock"
            }
        },
        {
            "41-71", new List<string>
            {
                "liver",
                "kardifur",
                "marauderspine",
                "mimicteeth",
                "succubushair",
                "whitebatwing",
                "frogmeat",
                "wolfskin",
                "crabclaw",
                "crabshell",
                "turtleshell",
                "frogleg",
                "faeriewing",
                "hobgoblinskull",
                "krakententancle",
                "wispcore",
                "wispflame"
            }
        },
        {
            "71-98", new List<string>
            {
                "ancientbone",
                "anemoneantenna",
                "blackcattail",
                "blackwidowsilk",
                "brawlfishscale",
                "ghastskull",
                "gremlinear",
                "gruesomeflyantenna",
                "gruesomeflywing",
                "leechtail",
                "mummybandage",
                "goo",
                "polypsac",
                "sporesac",
                "zombieflesh",
                "blackkoboldtail",
                "brownkoboldtail",
                "silverkoboldtail",
                "strangepotion"
            }
        },
        {
            "99+", new List<string>
            {
                "blackshockerpiece",
                "blueshockerpiece",
                "redshockerpiece",
                "goldshockerpiece",
                "direwolflock",
                "iceelementalflame",
                "iceskeletonskull",
                "icesporesac",
                "losganntail",
                "ruidhteartoe",
                "goldbeetalichead",
                "satyrhoof",
                "gargoylefiendskull",
                "horsehair",
                "darkflame",
                "brokenjackalsword",
                "cauldron",
                "copperfile",
                "goldsand",
                "holyink",
                "holyscroll",
                "jackalhilt",
                "magicink",
                "magicscroll",
                "smithhammer"
            }
        }
    };

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private string GetActiveHuntItem(Aisling source)
    {
        foreach (var levelBracket in ItemLists.Keys)
        {
            foreach (var item in ItemLists[levelBracket])
                if (source.Trackers.Counters.TryGetValue(item, out _))
                    return item;
        }

        return null!;
    }

    private string GetLevelBracket(int level)
        => level switch
        {
            <= 11 => "1-11",
            <= 41 => "11-41",
            <= 71 => "41-71",
            <= 98 => "71-98",
            _     => "99+"
        };

    private string GetRandomItem(string levelBracket)
        => ItemLists[levelBracket]
            .PickRandom();

    private void GiveQuestReward(Aisling source)
    {
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var seventyFivePercent = MathEx.GetPercentOf<int>(tnl, 75);
        var rewardAmount = source.UserStatSheet.Level < 99 ? seventyFivePercent : 100000000;
        ExperienceDistributionScript.GiveExp(source, rewardAmount);
    }

    public override void OnDisplaying(Aisling source)
    {
        var currentHuntItem = GetActiveHuntItem(source);

        if (!source.Trackers.Counters.TryGetValue("ThanksgivingTreasureHuntProgress", out var huntsCompleted))
            huntsCompleted = 0;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "tgtreasurehunt_initial":
                // Check if the player has exceeded the required hunts
                if (huntsCompleted > REQUIRED_HUNTS)
                {
                    Subject.Reply(source, "You've already completed the Thanksgiving Treasure Hunt.");

                    return;
                }

                // Congratulate player if they’ve completed the required hunts exactly
                if (huntsCompleted == REQUIRED_HUNTS)
                {
                    Subject.Reply(
                        source,
                        "Congratulations! You have completed the Thanksgiving Treasure Hunt quest! Please take this as your reward!");
                    GiveQuestReward(source);
                    source.Trackers.Counters.AddOrIncrement("ThanksgivingTreasureHuntProgress");

                    return;
                }

                // Determine the appropriate dialog ID based on the player's progress and current hunt item
                var dialogId = huntsCompleted switch
                               {
                                   0 when string.IsNullOrEmpty(currentHuntItem)     => "tgtreasurehunt_initial1",
                                   >= 0 when !string.IsNullOrEmpty(currentHuntItem) => "tgtreasurehunt_initial2",
                                   > 0 when string.IsNullOrEmpty(currentHuntItem)   => "tgtreasurehunt_initial3",
                                   _                                                => null
                               }
                               ?? string.Empty;

                Subject.Reply(source, "Skip", dialogId);

                break;

            case "tgtreasurehunt_turnin":
                if (!string.IsNullOrEmpty(currentHuntItem))
                {
                    var item = ItemFactory.Create(currentHuntItem);

                    if (source.Inventory.ContainsByTemplateKey(currentHuntItem))
                    {
                        Subject.Reply(source, "That is exactly what I am looking for! Thank you Aisling.", "chloe_initial");
                        source.Inventory.RemoveQuantityByTemplateKey(currentHuntItem, 1);
                        source.Trackers.Counters.AddOrIncrement("ThanksgivingTreasureHuntProgress");
                        source.Trackers.Counters.Remove(currentHuntItem, out _);

                        return;
                    }

                    source.Trackers.Counters.TryGetValue("ThanksgivingTreasureHuntProgress", out var huntsCompleted2);

                    if (huntsCompleted2 == REQUIRED_HUNTS)
                    {
                        Subject.Reply(
                            source,
                            "Congratulations! You have completed the Thanksgiving Treasure Hunt quest! Please take this as your reward!");
                        GiveQuestReward(source);
                        source.Trackers.Counters.AddOrIncrement("ThanksgivingTreasureHuntProgress");

                        return;
                    }

                    Subject.Reply(source, $"You don't have the {item.DisplayName}! Bring it back to me.", "chloe_initial");
                }

                break;

            case "tgtreasurehunt_start":
                StartTreasureHunt(source, huntsCompleted);

                break;
        }
    }

    private void StartTreasureHunt(Aisling source, int huntsCompleted)
    {
        var levelBracket = GetLevelBracket(source.UserStatSheet.Level);
        var requiredItem = GetRandomItem(levelBracket);
        var item = ItemFactory.Create(requiredItem);

        Subject.Reply(
            source,
            $"Your next treasure hunt has begun! Please bring back {item.DisplayName}.\n\n{huntsCompleted} treasure hunts completed. You must complete five.");
        source.Trackers.Counters.AddOrIncrement(requiredItem);
    }
}