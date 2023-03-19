using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;


public class GatherGoldRoseScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GatherGoldRoseScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

        if (aisling.Trackers.TimedEvents.HasActiveEvent("goldrose1cd", out var timedEvent))
        {
            aisling.SendOrangeBarMessage($"You can pick another Gold Rose in {timedEvent.Remaining.ToReadableString()}");

            return;
        }

        var goldrose = ItemFactory.Create("goldrose");
        var dialog = DialogFactory.Create("wildernessgoldrose", goldrose);
        dialog.Display(aisling);
        aisling.TryGiveItem(goldrose);
        aisling.SendOrangeBarMessage("You found a Gold Rose.");
        aisling.Trackers.TimedEvents.AddEvent("goldrose1cd", TimeSpan.FromHours(24), true);

    }
}