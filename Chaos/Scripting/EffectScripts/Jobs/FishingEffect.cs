using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Objects.World;
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

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        var fishingSpots = Subject.MapInstance.GetEntities<ReactorTile>()
                                  .Where(x => x.ScriptKeys.Contains("FishingSpot") && x.X.Equals(Subject.X) && x.Y.Equals(Subject.Y));

        if (!fishingSpots.Any())
        {
            Subject.Effects.Terminate("Fishing");
            return;
        }

        var chance = Randomizer.RollRange(5000, 99, RandomizationType.Negative);

        switch (chance)
        {
            //0.3%
            case >= 4985:
            {
                var item = _itemFactory.Create("giftbox");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a giftbox!");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            //0.5%
            case >= 4975 and < 4985:
            {
                var item = _itemFactory.Create("purplewhopper");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Purple Whopper");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            //.8%
            case >= 4960 and < 4975:
            {
                var item = _itemFactory.Create("lionfish");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Lion Fish");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            //1%
            case >= 4950 and < 4960:
            {
                var item = _itemFactory.Create("rockfish");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Rock Fish");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            //1.5%
            case >= 4925 and < 4950:
            {
                var item = _itemFactory.Create("pike");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Pike");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            //2%
            case >= 4900 and < 4925:
            {
                var item = _itemFactory.Create("Perch");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Perch");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                return;
            }
            //2.5%
            case >= 4875 and < 4900:
            {
                var item = _itemFactory.Create("Bass");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Bass");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            //3%
            case >= 4850 and < 4875:
            {
                var item = _itemFactory.Create("Trout");
                AislingSubject?.TryGiveItem(item);
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got a Trout");
                AislingSubject?.Inventory.RemoveQuantity("Fishing Bait", 1);
                break;
            }
            case <= 200:
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You recast your fishing rod in frustration.");
                AislingSubject?.AnimateBody(BodyAnimation.Assail);
                break;
            default:
            {
                var saying = _sayings.PickRandom();
                AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, saying);
                break;
            }
        }
    }

    public override void OnTerminated()
    {
        foreach (var reactor in Subject.MapInstance.GetEntities<ReactorTile>()
                                       .Where(x => x.ScriptKeys.Contains("FishingSpot") && x.X.Equals(Subject.X) && x.Y.Equals(Subject.Y)))
            reactor.OnWalkedOn(Subject);
    }
}