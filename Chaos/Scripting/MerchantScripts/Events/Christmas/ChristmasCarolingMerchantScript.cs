using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events.Christmas;

public class ChristmasCarolingMerchantScript : MerchantScriptBase
{
    // Map of carols to their lyrics
    private readonly Dictionary<string, List<string>> CarolLyrics;
    private readonly Dictionary<string, string> CarolRewards; // Rewards for completed carols
    private readonly IEffectFactory EffectFactory;
    private readonly IItemFactory ItemFactory;

    private readonly Dictionary<uint, List<string>> PlayerProgress;
    private readonly Dictionary<string, int> RareRewards; // Rare holiday rewards

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public ChristmasCarolingMerchantScript(Merchant subject, IItemFactory itemFactory, IEffectFactory effectFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        EffectFactory = effectFactory;

        // Define carols and their respective lyrics
        CarolLyrics = new Dictionary<string, List<string>>
        {
            {
                "Jingle Bells", new List<string>
                {
                    "Dashing through the snow",
                    "In a one-horse open sleigh",
                    "O'er the fields we go",
                    "Laughing all the way"
                }
            },
            {
                "Deck the Halls", new List<string>
                {
                    "Deck the halls with boughs of holly",
                    "Fa-la-la-la-la, la-la-la-la",
                    "Tis the season to be jolly",
                    "Fa-la-la-la-la, la-la-la-la"
                }
            },
            {
                "O Christmas Tree", new List<string>
                {
                    "O Christmas Tree, O Christmas Tree",
                    "How lovely are your branches",
                    "O Christmas Tree, O Christmas Tree",
                    "How lovely are your branches"
                }
            },
            {
                "Silent Night", new List<string>
                {
                    "Silent night, holy night",
                    "All is calm, all is bright",
                    "Round yon Virgin, Mother and Child",
                    "Holy Infant so tender and mild"
                }
            },
            {
                "We wish you a Merry Christmas", new List<string>
                {
                    "We wish you a Merry Christmas",
                    "We wish you a Merry Christmas",
                    "We wish you a Merry Christmas",
                    "And a Happy New Year"
                }
            },
            {
                "Frosty the Snowman", new List<string>
                {
                    "Frosty the snowman was a jolly happy soul",
                    "With a corncob pipe and a button nose",
                    "And two eyes made out of coal"
                }
            }
        };

        // Rewards for completed carols
        CarolRewards = new Dictionary<string, string>
        {
            {
                "Jingle Bells", "givegamepoints"
            },
            {
                "Deck the Halls", "givegold"
            },
            {
                "O Christmas Tree", "giveexperience"
            },
            {
                "Silent Night", "giveexpbuff"
            },
            {
                "We wish you a Merry Christmas", "givehotchocolatebuff"
            },
            {
                "Frosty the Snowman", "giveexperience"
            }
        };

        // Define rare rewards with weights
        RareRewards = new Dictionary<string, int>
        {
            {
                "santagift", 2
            },
            {
                "erbiegiftbox", 5
            },
            {
                "present", 15
            },
            {
                "mountmerrybox", 8
            },
            {
                "stockingstuffer", 5
            }
        };

        PlayerProgress = new Dictionary<uint, List<string>>();
    }

    public override void OnPublicMessage(Creature source, string message)
    {
        if (source is not Aisling aisling)
            return;

        if (!PlayerProgress.ContainsKey(aisling.Id))
            PlayerProgress[aisling.Id] = new List<string>(); // Initialize progress for the player

        foreach ((var carol, var lyrics) in CarolLyrics)
        {
            var progress = PlayerProgress[aisling.Id];

            // Check if the message matches the next lyric in the carol
            if (message.Equals(lyrics[progress.Count], StringComparison.OrdinalIgnoreCase))
            {
                if (aisling.Trackers.TimedEvents.HasActiveEvent($"{carol}", out var cdtime))
                {
                    aisling.SendOrangeBarMessage($"You have sung that carol too recently. {cdtime.Remaining.ToReadableString()}");

                    return;
                }

                if (aisling.Trackers.TimedEvents.HasActiveEvent($"{Subject.Name}", out var cdtime2))
                {
                    aisling.SendOrangeBarMessage($"{Subject.Name} listened to you carol already. {cdtime2.Remaining.ToReadableString()}");

                    return;
                }

                progress.Add(message);

                // If the player completes the carol
                if (progress.Count == lyrics.Count)
                {
                    Subject.Say($"Wonderful performance of '{carol}'!");
                    RewardPlayer(aisling, carol);
                    progress.Clear(); // Reset progress after rewarding

                    return;
                }

                Subject.Say("Beautiful! Keep going!");

                return;
            }
        }
    }

    private void RewardPlayer(Aisling aisling, string carol)
    {
        // Give the basic reward for completing the carol
        var rewardKey = CarolRewards[carol];

        switch (rewardKey)
        {
            case "giveexperience":
                if (aisling.UserStatSheet.Level > 98)
                    ExperienceDistributionScript.GiveExp(aisling, 2500000);
                else
                {
                    var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
                    var fivePercent = Convert.ToInt32(.05 * tnl);
                    ExperienceDistributionScript.GiveExp(aisling, fivePercent);
                }

                break;

            case "givehotchocolatebuff":
                var hotchocolate = EffectFactory.Create("hotchocolate");
                aisling.Effects.Apply(Subject, hotchocolate);

                break;

            case "givegold":
                var goldAmount = aisling.UserStatSheet.Level > 98 ? 15000 : 5000;
                aisling.TryGiveGold(goldAmount);

                break;

            case "givegamepoints":
                aisling.TryGiveGamePoints(2);

                break;

            case "giveexpbuff":
                var gmknowledge = EffectFactory.Create("gmknowledge");
                aisling.Effects.Apply(Subject, gmknowledge);

                break;
        }

        aisling.Trackers.TimedEvents.AddEvent($"{carol}", TimeSpan.FromHours(6), true);
        aisling.Trackers.TimedEvents.AddEvent($"{Subject.Name}", TimeSpan.FromHours(6), true);

        // Check for a rare bonus reward
        if (IntegerRandomizer.RollChance(10)) // 10% chance for a rare reward
        {
            var rareRewardKey = RareRewards.PickRandomWeighted();
            var rareRewardItem = ItemFactory.Create(rareRewardKey);
            aisling.GiveItemOrSendToBank(rareRewardItem);
            aisling.SendOrangeBarMessage($"You also received a rare reward: {rareRewardItem.DisplayName}!");
        }
    }
}