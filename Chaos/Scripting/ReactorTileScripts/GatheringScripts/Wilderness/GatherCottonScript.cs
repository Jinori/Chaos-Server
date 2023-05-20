using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;


public class GatherCottonScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IItemFactory _itemFactory;

    /// <inheritdoc />
    public GatherCottonScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        _itemFactory = itemFactory;
        _dialogFactory = dialogFactory;
    }
    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;
          
        if (!aisling.Trackers.TimedEvents.HasActiveEvent("cottoncd", out var timedEvent))
        {
            aisling.Trackers.Counters.Remove("cottonpicked", out _);

        }
        
        if (aisling.Trackers.TimedEvents.HasActiveEvent("cottoncd", out _))
        {
            if (aisling.Trackers.Counters.TryGetValue("cottonpicked", out var cotton) && (cotton >= 5))
            {
                aisling.SendOrangeBarMessage($"You can pick another cotton in {timedEvent.Remaining.ToReadableString()}");
                return;
            }
        }

        var cottonitem = _itemFactory.Create("cotton");
        aisling.TryGiveItem(cottonitem);
        aisling.SendOrangeBarMessage("You've gathered some cotton.");
        aisling.Trackers.Counters.AddOrIncrement("cottonpicked");
        
        if (!aisling.Trackers.TimedEvents.HasActiveEvent("cottoncd", out _))
        {
            aisling.Trackers.TimedEvents.AddEvent("cottoncd", TimeSpan.FromHours(3), true);
        }

        return;
    }
}