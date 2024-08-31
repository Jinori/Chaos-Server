using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
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

public class ForagingEffect(IItemFactory itemFactory, ILogger<ForagingEffect> logger) : ContinuousAnimationEffectBase
{
    private int ForageGatherChance = 2;
    private const byte FORAGE_ICON = 95;
    private int DamageGlove = 5;

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private static readonly List<KeyValuePair<string, decimal>> ForagingData = new()
    {
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
        new KeyValuePair<string, decimal>("petunia", 2),
        new KeyValuePair<string, decimal>("pinkrose", 2),
        new KeyValuePair<string, decimal>("waterlily", 2),
        new KeyValuePair<string, decimal>("blossomofbetrayal", 0.3m),
        new KeyValuePair<string, decimal>("bocanbough", 0.8m),
        new KeyValuePair<string, decimal>("cactusflower", 0.8m),
        new KeyValuePair<string, decimal>("dochasbloom", 0.8m),
        new KeyValuePair<string, decimal>("lilypad", 0.8m),
        new KeyValuePair<string, decimal>("koboldtail", 1),
        new KeyValuePair<string, decimal>("kabineblossom", 1),
        new KeyValuePair<string, decimal>("passionflower", 1),
        new KeyValuePair<string, decimal>("raineach", 1),
        new KeyValuePair<string, decimal>("sparkflower", .06m)
    };

    private readonly List<string> GloveBreakMessages = new()
    {
        "Your glove gets a little bit worn.",
        "That bush tore a small hole in your glove.",
        "Glove takes a little damage",
        "Your glove is torn slightly from the bush.",
        "The bush damages your glove slightly.",
        "You feel your glove wearing out.",
        "You notice your glove is breaking slowly.",
        "A little tear happens on your glove.",
        "Glove fabric experiences a tear."
    };

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
        { "Sparkflower", 0.009 },
    };

    private List<Point> ForagingSpots = new();
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 169
    };
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(10), false);

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
            || aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey.EndsWith("Glove", StringComparison.OrdinalIgnoreCase) != true)
        {
            Subject.Effects.Terminate("Foraging");
            return;
        }

        
        if (IsUsingIronGlove(aisling))
        {
            ForageGatherChance = 4;
            DamageGlove = 4;
        } else if (IsUsingMythrilGlove(aisling))
        {
            ForageGatherChance = 5;
            DamageGlove = 3;
        }else if (IsUsingHybrasylGlove(aisling))
        {
            ForageGatherChance = 6;
            DamageGlove = 2;
        }

        if (IntegerRandomizer.RollChance(DamageGlove))
        {
            var randomMessage = GloveBreakMessages[Random.Shared.Next(GloveBreakMessages.Count)];
            var equippedGlove = aisling.Equipment[EquipmentSlot.Weapon];

            if (equippedGlove != null && equippedGlove.Template.TemplateKey.EndsWith("Glove", StringComparison.OrdinalIgnoreCase))
            {
                if (equippedGlove.CurrentDurability > 5)
                {
                    equippedGlove.CurrentDurability -= 5;
                }
                else
                {
                    aisling.Equipment.RemoveByTemplateKey(equippedGlove.Template.TemplateKey);
                    aisling.SendOrangeBarMessage("Your glove breaks!");
                }

                aisling.SendOrangeBarMessage(randomMessage);
            }
        }


        if (!IntegerRandomizer.RollChance(ForageGatherChance))
            return;

        var templateKey = ForagingData.PickRandomWeighted();
        var herb = itemFactory.Create(templateKey);

        if (aisling.TryGiveItem(ref herb))
        {
            // Calculate experience based on herb foraged and award it to the player
            var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
            var expGain = CalculateExperienceGain(aisling, tnl, herb.DisplayName);

            if (expGain >= 25000)
            {
                expGain = 25000;
            }

            ExperienceDistributionScript.GiveExp(aisling, expGain);
            aisling.SendOrangeBarMessage($"You gather a {herb.DisplayName} and gained {expGain} experience!");
            
            logger.WithTopics(
                    Topics.Entities.Aisling,
                    Topics.Entities.Item,
                    Topics.Entities.Dialog,
                    Topics.Entities.Quest,
                    Topics.Entities.Experience)
                .WithProperty(aisling)
                .WithProperty(Subject)
                .LogInformation("{@AislingName} has received {@Herb} and {@Experience} from foraging.", aisling.Name, herb.DisplayName, expGain);


            UpdatePlayerLegend(aisling);
        }
        else
        {
            aisling.SendOrangeBarMessage($"You gathered a {herb.DisplayName}!");
        }
    }
    
    private bool IsUsingIronGlove(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;
        return weaponTemplateKey?.StartsWith("Iron", StringComparison.OrdinalIgnoreCase) == true;
    }

    private bool IsUsingMythrilGlove(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;
        return weaponTemplateKey?.StartsWith("Mythril", StringComparison.OrdinalIgnoreCase) == true;
    }

    private bool IsUsingHybrasylGlove(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;
        return weaponTemplateKey?.StartsWith("Hybrasyl", StringComparison.OrdinalIgnoreCase) == true;
    }

    private int CalculateExperienceGain(Aisling source, int tnl, string herbName)
    {

        if (!ForagingExperienceMultipliers.TryGetValue(herbName, out var multiplier))
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
