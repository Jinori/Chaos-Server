using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Jobs;

public class FishingEffect : ContinuousAnimationEffectBase
{
    private readonly IItemFactory _itemFactory;
    private readonly List<string> _sayings = new()
    {
        "Your bobber slightly dips but nothing happens.", "You feel a bite at the line.", "*yawn* The water is calm and serene.",
        "Cursing at the sky, you say you'll never give up!", "A small patch of water ripples."
    };

    /// <inheritdoc />
    public override byte Icon { get; } = 203;
    /// <inheritdoc />
    public override string Name { get; } = "Fishing";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 169
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(18);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(5));

    public FishingEffect(IItemFactory itemFactory) => _itemFactory = itemFactory;

    private static readonly List<string> FishList = new List<string> { "uselessboot", "trout", "bass", "perch", "pike", "rockfish", "lionfish", "purplewhopper"};
    private static readonly List<decimal> FishWeight = new List<decimal> { 20, 30, 25, 20, 15, 10, 8, 5 };
    
    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var aisling = AislingSubject!;
        
        var fishingSpots = Subject.MapInstance.GetEntities<ReactorTile>()
                                  .Where(x => x.ScriptKeys.Contains("FishingSpot") && x.X.Equals(Subject.X) && x.Y.Equals(Subject.Y));

        if (!fishingSpots.Any() || !aisling.Inventory.HasCount("Fishing Bait", 1))
        {
            Subject.Effects.Terminate("Fishing");
            return;
        }
        
        if (!Randomizer.RollChance(2))
        {
            return;
        }

        var templateKey = FishList.PickRandom(FishWeight);
        var fish = _itemFactory.Create(templateKey);
        if (!aisling.TryGiveItem(fish))
            return;
        
        aisling.SendOrangeBarMessage($"You caught a {fish.DisplayName}!");
        aisling.Inventory.RemoveQuantity("Fishing Bait", 1);
    }

    public override void OnTerminated()
    {
        foreach (var reactor in Subject.MapInstance.GetEntities<ReactorTile>()
                                       .Where(x => x.ScriptKeys.Contains("FishingSpot") && x.X.Equals(Subject.X) && x.Y.Equals(Subject.Y)))
            reactor.OnWalkedOn(Subject);
    }
}