using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness;

public class GatherCottonScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GatherCottonScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (!aisling.Trackers.TimedEvents.HasActiveEvent("cottoncd", out var timedEvent))
            aisling.Trackers.Counters.Remove("cottonpicked", out _);

        if (aisling.Trackers.TimedEvents.HasActiveEvent("cottoncd", out _))
            if (aisling.Trackers.Counters.TryGetValue("cottonpicked", out var cotton) && (cotton >= 5))
            {
                aisling.SendOrangeBarMessage($"You can pick another cotton in {timedEvent?.Remaining.ToReadableString()}");

                return;
            }

        var cottonitem = ItemFactory.Create("cotton");
        aisling.GiveItemOrSendToBank(cottonitem);
        aisling.SendOrangeBarMessage("You've gathered some cotton.");
        aisling.Trackers.Counters.AddOrIncrement("cottonpicked");

        if (!aisling.Trackers.TimedEvents.HasActiveEvent("cottoncd", out _))
            aisling.Trackers.TimedEvents.AddEvent("cottoncd", TimeSpan.FromHours(3), true);
    }
}