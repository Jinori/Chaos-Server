﻿using Chaos.Common.Definitions;
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
    private const int FORAGE_GATHER_CHANCE = 2;
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
        new KeyValuePair<string, decimal>("petunia", 5),
        new KeyValuePair<string, decimal>("pinkrose", 5),
        new KeyValuePair<string, decimal>("waterlily", 5),
        new KeyValuePair<string, decimal>("blossomofbetrayal", 2),
        new KeyValuePair<string, decimal>("bocanbough", 4),
        new KeyValuePair<string, decimal>("cactusflower", 4),
        new KeyValuePair<string, decimal>("dochasbloom", 4),
        new KeyValuePair<string, decimal>("lilypad", 4),
        new KeyValuePair<string, decimal>("koboldtail", 5),
        new KeyValuePair<string, decimal>("kabineblossom", 5),
        new KeyValuePair<string, decimal>("passionflower", 5),
        new KeyValuePair<string, decimal>("raineach", 5),
        new KeyValuePair<string, decimal>("sparkflower", 5),
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
        { "acorn", 0.004 },
        { "apple", 0.005 },
        { "cherry", 0.005 },
        { "grape", 0.005 },
        { "greengrapes", 0.007 },
        { "strawberry", 0.007 },
        { "tangerines", 0.007 },
        { "carrot", 0.005 },
        { "rambutan", 0.01 },
        { "tomato", 0.005 },
        { "vegetable", 0.005 },
        { "petunia", 0.015 },
        { "pinkrose", 0.015 },
        { "waterlily", 0.015 },
        { "blossomofbetrayal", 0.03 },
        { "bocanbough", 0.01 },
        { "cactusflower", 0.01 },
        { "dochasbloom", 0.01 },
        { "lilypad", 0.01 },
        { "koboldtail", 0.009 },
        { "kabineblossom", 0.009 },
        { "passionflower", 0.009 },
        { "raineach", 0.009 },
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

    if ((ForagingSpots.Count == 0) || !ForagingSpots.Contains(playerLocation) || aisling.Equipment[EquipmentSlot.Weapon]?.Template.Name != ("Cloth Glove"))
    {
        Subject.Effects.Terminate("Foraging");
        return;
    }

    if (IntegerRandomizer.RollChance(DAMAGE_GLOVE))
    {
        var randomMessage = GloveBreakMessages[Random.Shared.Next(GloveBreakMessages.Count)];
        var equipment = aisling.Equipment;

        foreach (var item in equipment)
        {
            if (aisling.IsGodModeEnabled())
                continue;
            
            if (!item.DisplayName.Contains("Glove"))
                return;
            
                // Reduce the item's durability by 20, ensuring it does not drop below 0
                for (var i = 0; i < 5; i++)
                {
                    if (item.CurrentDurability >= 1)
                        item.CurrentDurability--;
                    else
                        break; // Stop reducing if durability reaches 0
                }
        }
        aisling.SendOrangeBarMessage(randomMessage);
    }
    
    if (!IntegerRandomizer.RollChance(FORAGE_GATHER_CHANCE))
        return;

    var templateKey = ForagingData.PickRandomWeighted();
    var herb = itemFactory.Create(templateKey);

    if (aisling.TryGiveItem(ref herb))
    {
        // Calculate experience based on fish caught and award it to the player
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