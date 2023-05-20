using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;


public class WildernessGatherCherryScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IItemFactory _itemFactory;

    /// <inheritdoc />
    public WildernessGatherCherryScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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
          
        if (!aisling.Trackers.TimedEvents.HasActiveEvent("cherrycd", out var timedEvent))
        {
            aisling.Trackers.Counters.Remove("cherrypicked", out _);

        }
        
        if (aisling.Trackers.TimedEvents.HasActiveEvent("cherrycd", out _))
        {
            if (aisling.Trackers.Counters.TryGetValue("cherrypicked", out var wildernesscherry) && (wildernesscherry >= 5))
            {
                aisling.SendOrangeBarMessage($"You can pick another cherry in {timedEvent.Remaining.ToReadableString()}");
                return;
            }
        }

        var cherryitem = _itemFactory.Create("cherry");
        aisling.TryGiveItem(cherryitem);
        aisling.SendOrangeBarMessage("You've gathered some cherries.");
        aisling.Trackers.Counters.AddOrIncrement("cherrypicked");
        
        if (!aisling.Trackers.TimedEvents.HasActiveEvent("cherrycd", out _))
        {
            aisling.Trackers.TimedEvents.AddEvent("cherrycd", TimeSpan.FromHours(3), true);
        }

        return;
    }
}