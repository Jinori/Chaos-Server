using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;


public class GatherBlackRose2Script : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GatherBlackRose2Script(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

        if (aisling.Trackers.TimedEvents.HasActiveEvent("blackrose2cd", out var timedEvent))
        {
            aisling.SendOrangeBarMessage($"You can pick another Black Rose in {timedEvent.Remaining.ToReadableString()}");

            return;
        }

        var blackrose = ItemFactory.Create("blackrose");
        var dialog = DialogFactory.Create("wildernessblackrose", blackrose);
        dialog.Display(aisling);
        aisling.TryGiveItem(blackrose);
        aisling.SendOrangeBarMessage("You found a Black Rose.");
        aisling.Trackers.TimedEvents.AddEvent("blackrose2cd", TimeSpan.FromHours(24), true);

    }
}