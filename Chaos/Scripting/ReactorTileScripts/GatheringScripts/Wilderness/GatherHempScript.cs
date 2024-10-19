using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness;

public class GatherHempScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IIntervalTimer? RefreshTimer;

    /// <inheritdoc />
    public GatherHempScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (source.StatSheet.Level < 99)
        {
            aisling.SendOrangeBarMessage("You're afraid to pick this plant. (Must be level 99)");

            return;
        }

        //if a timer has been set, and that timer has not elapsed
        //the plant has been picked recently
        if (RefreshTimer is not null && !RefreshTimer.IntervalElapsed)
        {
            aisling.SendOrangeBarMessage("This hemp plant has been picked recently.");

            return;
        }

        //set random cooldown on plant between 2-3 hours
        var randomCooldown = Random.Shared.Next(2, 4); //4 here cuz upper bound is not inclusive
        RefreshTimer = new IntervalTimer(TimeSpan.FromHours(randomCooldown), false);

        var item = ItemFactory.Create("hemp");
        aisling.GiveItemOrSendToBank(item);
        aisling.SendOrangeBarMessage("You've gathered some hemp.");
    }

    public override void Update(TimeSpan delta)
    {
        RefreshTimer?.Update(delta);

        //if cooldown elapses, set timer to null
        if (RefreshTimer?.IntervalElapsed ?? false)
            RefreshTimer = null;
    }
}