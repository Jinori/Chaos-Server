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

    private static readonly List<KeyValuePair<string, decimal>> FishData = new()
    {
        new KeyValuePair<string, decimal>("uselessboot", 20),
        new KeyValuePair<string, decimal>("trout", 30),
        new KeyValuePair<string, decimal>("bass", 25),
        new KeyValuePair<string, decimal>("perch", 20),
        new KeyValuePair<string, decimal>("pike", 15),
        new KeyValuePair<string, decimal>("rockfish", 10),
        new KeyValuePair<string, decimal>("lionfish", 8),
        new KeyValuePair<string, decimal>("purplewhopper", 5)
    };
    
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
        
        if (!IntegerRandomizer.RollChance(2))
        {
            return;
        }

        var templateKey = FishData.PickRandomWeighted();
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