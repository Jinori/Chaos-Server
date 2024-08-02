using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Jobs;

public class ForagingEffect(IItemFactory itemFactory) : ContinuousAnimationEffectBase
{
    private const int FORAGE_GATHER_CHANCE = 1;
    private const byte FORAGE_ICON = 95;
    private const int DAMAGE_GLOVE = 5;
    
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private static readonly List<KeyValuePair<string, decimal>> ForagingData =
    [
        new KeyValuePair<string, decimal>("acorn", 25),
        new KeyValuePair<string, decimal>("apple", 20), 
        new KeyValuePair<string, decimal>("cherry", 20), 
        new KeyValuePair<string, decimal>("grape", 20), 
        new KeyValuePair<string, decimal>("greengrapes", 10),
        new KeyValuePair<string, decimal>("strawberry", 10),
        new KeyValuePair<string, decimal>("tangerines", 10),
        new KeyValuePair<string, decimal>("carrot", 15),
        new KeyValuePair<string, decimal>("rambutan", 4),
        new KeyValuePair<string, decimal>("tomato", 15),
        new KeyValuePair<string, decimal>("vegetable", 15),
        new KeyValuePair<string, decimal>("petunia", 0.1m),
        new KeyValuePair<string, decimal>("pinkrose", 0.1m),
        new KeyValuePair<string, decimal>("pinkrose", 0.1m),
        new KeyValuePair<string, decimal>("waterlily", 0.1m),
        new KeyValuePair<string, decimal>("blossomofbetrayal", 0.02m),
        new KeyValuePair<string, decimal>("bocanbough", 0.04m),
        new KeyValuePair<string, decimal>("cactusflower", 0.04m),
        new KeyValuePair<string, decimal>("dochasbloom", 0.04m),
        new KeyValuePair<string, decimal>("lilypad", 0.04m),
        new KeyValuePair<string, decimal>("koboldtail", 0.05m),
        new KeyValuePair<string, decimal>("kabineblossom", 0.05m),
        new KeyValuePair<string, decimal>("passionflower", 0.05m),
        new KeyValuePair<string, decimal>("raineach", 0.05m),
        new KeyValuePair<string, decimal>("sparkflower", 0.03m)
    ];

    private readonly List<string> GloveBreakMessages =
    [
        "Your glove gets a little bit worn.",
        "That bush tore a small hole in your glove.",
        "Glove takes a little damage",
        "Your glove is torn slightly from the bush.",
        "The bush damages your glove slightly.",
        "You feel your glove wearing out.",
        "You notice your glove is breaking slowly.",
        "A little tear happens on your glove.",
        "Glove fabric experiences a tear."
    ];

    private static readonly Dictionary<string, double> ForagingExperienceMultipliers = new()
    {
        { "Acorn", 0.004 },
        { "Apple", 0.005 },
        { "Cherry", 0.005 },
        { "Grapes", 0.005 },
        { "Green Grapes", 0.007 },
        { "Strawberry", 0.007 },
        { "Tangerines", 0.007 },
        { "Carrot", 0.005 },
        { "Rambutan", 0.01 },
        { "Tomato", 0.005 },
        { "Vegetable", 0.005 },
        { "Petunia", 0.015 },
        { "Pink Rose", 0.015 },
        { "Water Lily", 0.015 },
        { "Blossom of Betrayal", 0.03 },
        { "Bocan Bough", 0.01 },
        { "Cactus Flower", 0.01 },
        { "Dochas Bloom", 0.01 },
        { "Lily Pad", 0.01 },
        { "Kobold Tail", 0.009 },
        { "Kabine Blossom", 0.009 },
        { "Passion Flower", 0.009 },
        { "Raineach", 0.009 },
        { "sparkflower", 0.009 },
    };
    
    private List<Point> ForagingSpots = new();
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 169
    };
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(10));

    public override byte Icon => FORAGE_ICON;
    public override string Name => "Foraging";

    public override void OnApplied() =>
        ForagingSpots = Subject.MapInstance.GetEntities<ReactorTile>()
                               .Where(x => x.ScriptKeys.Contains("ForagingSpot"))
                               .Select(x => new Point(x.X, x.Y))
                               .ToList();

    protected override void OnIntervalElapsed()
    {
        var aisling = AislingSubject!;
        var playerLocation = new Point(Subject.X, Subject.Y);

        if ((ForagingSpots.Count == 0)
            || !ForagingSpots.Contains(playerLocation)
            || (aisling.Equipment[EquipmentSlot.Weapon]?.Template.Name != ("Cloth Glove")))
        {
            Subject.Effects.Terminate("Foraging");

            return;
        }

        if (IntegerRandomizer.RollChance(DAMAGE_GLOVE))
        {
            var randomMessage = GloveBreakMessages[Random.Shared.Next(GloveBreakMessages.Count)];
            var hasGloveOn = aisling.Equipment.TryGetObject("Cloth Glove", out var item);

            switch (hasGloveOn)
            {
                case true when item?.CurrentDurability > 5:
                    item.CurrentDurability -= 5;

                    break;
                case true when item?.CurrentDurability <= 5:
                    aisling.Equipment.RemoveByTemplateKey("clothglove");
                    aisling.SendOrangeBarMessage("Your glove breaks!");

                    break;
            }

            aisling.SendOrangeBarMessage(randomMessage);
        }

        if (!IntegerRandomizer.RollChance(FORAGE_GATHER_CHANCE))
            return;

        var templateKey = ForagingData.PickRandomWeighted();
        var herb = itemFactory.Create(templateKey);

        if (aisling.TryGiveItem(ref herb))
        {
            // Calculate experience based on herb foraged and award it to the player
            var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
            var expGain = CalculateExperienceGain(aisling, tnl, herb.DisplayName);

            ExperienceDistributionScript.GiveExp(aisling, expGain);
            aisling.SendOrangeBarMessage($"You gather a {herb.DisplayName} and gained {expGain} experience!");

            UpdatePlayerLegend(aisling);
        }
        else
        {
            aisling.SendOrangeBarMessage($"You gathered a {herb.DisplayName}!");
        }
    }

    private int CalculateExperienceGain(Aisling source, int tnl, string fishName)
    {

        if (!ForagingExperienceMultipliers.TryGetValue(fishName, out var multiplier))
        {
            source.SendActiveMessage("Something went wrong when trying to forage a herb!");

            return 0;
        }

        return Convert.ToInt32(multiplier * tnl);
    }

    private void UpdatePlayerLegend(Aisling source) =>
    source.Legend.AddOrAccumulate(
        new LegendMark(
            "Foraged a bush",
            "forage",
            MarkIcon.Yay,
            MarkColor.White,
            1,
            GameTime.Now));
    public override void OnTerminated()
    {
        var playerLocation = new Point(Subject.X, Subject.Y);

        if ((ForagingSpots.Count == 0) || !ForagingSpots.Contains(playerLocation))
        {
            Subject.Effects.Terminate("Foraging");

            return;
        }

        var foragingSpot = ForagingSpots.FirstOrDefault(x => x.Equals(playerLocation));

        var reactorTile = Subject.MapInstance.GetEntities<ReactorTile>()
                                 .FirstOrDefault(x => (x.X == foragingSpot.X) && (x.Y == foragingSpot.Y));

        reactorTile?.OnWalkedOn(Subject);
    }
}