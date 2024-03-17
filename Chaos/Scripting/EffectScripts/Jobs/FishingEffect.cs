using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Jobs;

public class FishingEffect(IItemFactory itemFactory) : ContinuousAnimationEffectBase
{
    private const int FISH_CATCH_CHANCE = 2;
    private const byte FISHING_ICON = 203;

    private static readonly List<KeyValuePair<string, decimal>> FishData =
    [
        new KeyValuePair<string, decimal>("uselessboot", 20),
        new KeyValuePair<string, decimal>("trout", 25),
        new KeyValuePair<string, decimal>("bass", 20),
        new KeyValuePair<string, decimal>("perch", 15),
        new KeyValuePair<string, decimal>("pike", 10),
        new KeyValuePair<string, decimal>("rockfish", 8),
        new KeyValuePair<string, decimal>("lionfish", 6),
        new KeyValuePair<string, decimal>("purplewhopper", 2)
    ];
    private List<Point> FishingSpots = new();
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 169
    };
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(10));

    public override byte Icon => FISHING_ICON;
    public override string Name => "Fishing";

    public override void OnApplied() =>
        FishingSpots = Subject.MapInstance.GetEntities<ReactorTile>()
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

        if (!IntegerRandomizer.RollChance(FISH_CATCH_CHANCE))
        {
            aisling.SendOrangeBarMessage($"You feel a tug and lose your bait.");
            aisling.Inventory.RemoveQuantity("Fishing Bait", 1);
            return;
        }

        var templateKey = FishData.PickRandomWeighted();
        var fish = itemFactory.Create(templateKey);

        if (!aisling.TryGiveItem(ref fish))
            return;

        aisling.SendOrangeBarMessage($"You caught a {fish.DisplayName}!");
        aisling.Inventory.RemoveQuantity("Fishing Bait", 1);
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

        var reactorTile = Subject.MapInstance.GetEntities<ReactorTile>()
                                 .FirstOrDefault(x => (x.X == fishingSpot.X) && (x.Y == fishingSpot.Y));

        reactorTile?.OnWalkedOn(Subject);
    }
}