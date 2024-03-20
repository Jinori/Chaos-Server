using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EasterEggs;

public sealed class CrackInWallScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public CrackInWallScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        
        if (source.Trackers.Flags.HasFlag(Definitions.EasterEggs.CrackInWall))
        {
            source.SendOrangeBarMessage("You've already found this.");
            return;
        }

        var item = ItemFactory.Create("Sparkles");
        source.Trackers.Flags.AddFlag(Definitions.EasterEggs.CrackInWall);
        source.TryGiveItem(ref item);
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "You notice a crack in the wall then reach your hand through.");
        source.TryGiveGamePoints(5);
        source.TryGiveGold(45000);
        source.SendOrangeBarMessage("Tell WHUG you found this crack.");
        return;
    }
}