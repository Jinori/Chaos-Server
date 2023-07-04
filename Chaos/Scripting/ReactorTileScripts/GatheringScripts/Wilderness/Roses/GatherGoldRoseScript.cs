using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts.Wilderness.Roses;

public class GatherGoldRoseScript : ReactorTileScriptBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IItemFactory _itemFactory;

    /// <inheritdoc />
    public GatherGoldRoseScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

        if (aisling.Trackers.TimedEvents.HasActiveEvent("goldrose1cd", out var timedEvent))
        {
            aisling.SendOrangeBarMessage($"You can pick another Gold Rose in {timedEvent.Remaining.ToReadableString()}");

            return;
        }

        var goldrose = _itemFactory.Create("goldrose");
        var dialog = _dialogFactory.Create("wildernessgoldrose", goldrose);
        dialog.Display(aisling);
        aisling.TryGiveItem(ref goldrose);
        aisling.SendOrangeBarMessage("You found a Gold Rose.");
        aisling.Trackers.TimedEvents.AddEvent("goldrose1cd", TimeSpan.FromHours(24), true);
    }
}