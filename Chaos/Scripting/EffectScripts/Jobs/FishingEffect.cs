using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Jobs;

public class FishingEffect(IItemFactory itemFactory, ILogger<FishingEffect> logger) : ContinuousAnimationEffectBase
{
    private const byte FISHING_ICON = 95;

    private static readonly List<KeyValuePair<string, decimal>> FishData =
    [
        new("uselessboot", 20),
        new("trout", 25),
        new("bass", 20),
        new("perch", 15),
        new("pike", 10),
        new("rockfish", 8),
        new("lionfish", 6),
        new("purplewhopper", 2)
    ];

    private static readonly Dictionary<string, double> FishExperienceMultipliers = new()
    {
        {
            "Trout", 0.006
        },
        {
            "Bass", 0.007
        },
        {
            "Perch", 0.008
        },
        {
            "Pike", 0.009
        },
        {
            "Rock Fish", 0.01
        },
        {
            "Lion Fish", 0.02
        },
        {
            "Purple Whopper", 0.03
        }
    };

    private readonly List<string> BaitLossMessages =
    [
        "Bait lost to a sly swimmer!",
        "Empty hook, clever fish.",
        "The bait's gone, fish wins!",
        "Swift bite, no catch though.",
        "Fish tricked you this time.",
        "Bait stolen by the depths.",
        "Your bait's a fish's feast.",
        "Snatched! The bait's gone.",
        "A tug but no prize.",
        "Fish outsmarted you today.",
        "Bait gone, better luck next time.",
        "Sneaky fish, bait's gone!",
        "Bait lost, nothing caught.",
        "Unseen fish steals the bait.",
        "The water claims your bait."
    ];

    private int FishCatchChance = 2;

    private List<Point> FishingSpots = new();
    private int FishStealBait = 5;
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 169
    };

    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(10), false);

    public override byte Icon => FISHING_ICON;
    public override string Name => "Fishing";

    private int CalculateExperienceGain(Aisling source, int tnl, string fishName)
    {
        if (!FishExperienceMultipliers.TryGetValue(fishName, out var multiplier))
        {
            source.SendActiveMessage("Something went wrong when trying to catch the fish!");

            return 0;
        }

        return Convert.ToInt32(multiplier * tnl);
    }

    private bool IsUsingGoodPole(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;

        return weaponTemplateKey?.StartsWith("good", StringComparison.OrdinalIgnoreCase) == true;
    }

    private bool IsUsingGrandPole(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;

        return weaponTemplateKey?.StartsWith("Grand", StringComparison.OrdinalIgnoreCase) == true;
    }

    private bool IsUsingGreatPole(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;

        return weaponTemplateKey?.StartsWith("Great", StringComparison.OrdinalIgnoreCase) == true;
    }

    public override void OnApplied()
        => FishingSpots = Subject.MapInstance
                                 .GetEntities<ReactorTile>()
                                 .Where(x => x.ScriptKeys.Contains("FishingSpot"))
                                 .Select(x => new Point(x.X, x.Y))
                                 .ToList();

    protected override void OnIntervalElapsed()
    {
        var aisling = AislingSubject!;
        var playerLocation = new Point(Subject.X, Subject.Y);

        if ((FishingSpots.Count == 0) || !FishingSpots.Contains(playerLocation) || !aisling.Inventory.HasCount("Fishing Bait", 1))
        {
            Subject.Effects.Terminate("Fishing");

            return;
        }

        if (IsUsingGoodPole(aisling))
        {
            FishCatchChance = 4;
            FishStealBait = 4;
        } else if (IsUsingGreatPole(aisling))
        {
            FishCatchChance = 5;
            FishStealBait = 3;
        } else if (IsUsingGrandPole(aisling))
        {
            FishCatchChance = 6;
            FishStealBait = 2;
        }

        if (IntegerRandomizer.RollChance(FishStealBait))
        {
            var randomMessage = BaitLossMessages[Random.Shared.Next(BaitLossMessages.Count)];

            aisling.SendOrangeBarMessage(randomMessage);
            aisling.Inventory.RemoveQuantity("Fishing Bait", 1);
        }

        if (!IntegerRandomizer.RollChance(FishCatchChance))
            return;

        var templateKey = FishData.PickRandomWeighted();
        var fish = itemFactory.Create(templateKey);

        if (aisling.TryGiveItem(ref fish))
        {
            // Calculate experience based on fish caught and award it to the player
            var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
            var expGain = CalculateExperienceGain(aisling, tnl, fish.DisplayName);

            if (expGain >= 25000)
                expGain = 25000;

            ExperienceDistributionScript.GiveExp(aisling, expGain);
            aisling.SendOrangeBarMessage($"You caught a {fish.DisplayName} and gained {expGain} experience!");

            // Remove fishing bait
            aisling.Inventory.RemoveQuantity("Fishing Bait", 1);
            UpdatePlayerLegend(aisling);

            logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Item,
                      Topics.Entities.Dialog,
                      Topics.Entities.Quest,
                      Topics.Entities.Experience)
                  .WithProperty(aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "{@AislingName} has received {@fish} and {@Experience} from fishing.",
                      aisling.Name,
                      fish.DisplayName,
                      expGain);
        } else
        {
            aisling.SendOrangeBarMessage($"You caught a {fish.DisplayName}!");
            aisling.Inventory.RemoveQuantity("Fishing Bait", 1);
        }
    }

    public override void OnTerminated()
    {
        var playerLocation = new Point(Subject.X, Subject.Y);

        if ((FishingSpots.Count == 0) || !FishingSpots.Contains(playerLocation))
        {
            Subject.Effects.Terminate("Fishing");

            return;
        }

        var fishingSpot = FishingSpots.FirstOrDefault(x => x.Equals(playerLocation));

        var reactorTile = Subject.MapInstance
                                 .GetEntities<ReactorTile>()
                                 .FirstOrDefault(x => (x.X == fishingSpot.X) && (x.Y == fishingSpot.Y));

        reactorTile?.OnWalkedOn(Subject);
    }

    private void UpdatePlayerLegend(Aisling source)
        => source.Legend.AddOrAccumulate(
            new LegendMark(
                "Caught a fish",
                "fish",
                MarkIcon.Yay,
                MarkColor.White,
                1,
                GameTime.Now));
}