using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;

public class GatherWhiteRose2Script : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IItemFactory _itemFactory;

    /// <inheritdoc />
    public GatherWhiteRose2Script(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

        if (aisling.Trackers.TimedEvents.HasActiveEvent("whiterose2cd", out var timedEvent))
        {
            aisling.SendOrangeBarMessage($"You can pick another White Rose in {timedEvent.Remaining.ToReadableString()}");

            return;
        }

        var whiterose = _itemFactory.Create("whiterose");
        var dialog = _dialogFactory.Create("wildernesswhiterose", whiterose);
        dialog.Display(aisling);
        aisling.TryGiveItem(ref whiterose);
        aisling.SendOrangeBarMessage("You found a White Rose.");
        aisling.Trackers.TimedEvents.AddEvent("whiterose2cd", TimeSpan.FromHours(24), true);
    }
}