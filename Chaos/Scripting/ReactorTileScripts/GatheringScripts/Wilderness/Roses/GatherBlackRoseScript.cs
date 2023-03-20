using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;


public class GatherBlackRoseScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IItemFactory _itemFactory;

    /// <inheritdoc />
    public GatherBlackRoseScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

        if (aisling.Trackers.TimedEvents.HasActiveEvent("blackrose1cd", out var timedEvent))
        {
            aisling.SendOrangeBarMessage($"You can pick another Black Rose in {timedEvent.Remaining.ToReadableString()}");
            return;
        }

        var blackrose = _itemFactory.Create("blackrose");
        var dialog = _dialogFactory.Create("wildernessblackrose", blackrose);
        dialog.Display(aisling);
        aisling.TryGiveItem(blackrose);
        aisling.SendOrangeBarMessage("You found a Black Rose.");
        aisling.Trackers.TimedEvents.AddEvent("blackrose1cd", TimeSpan.FromHours(24), true);

    }
}