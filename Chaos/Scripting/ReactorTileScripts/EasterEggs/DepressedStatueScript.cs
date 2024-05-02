using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EasterEggs;

public sealed class DepressedStatueScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public DepressedStatueScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        
        if (source.Trackers.Flags.HasFlag(Definitions.EasterEggs.DepressedStatue))
        {
            source.SendOrangeBarMessage("You've already found this.");
            return;
        }
        
        source.Trackers.Flags.AddFlag(Definitions.EasterEggs.DepressedStatue);
        var toydoll = ItemFactory.Create("bluepolyp");
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "You noticed the depressed monk...");
        source.TryGiveGamePoints(5);
        source.TryGiveGold(50000);
        source.GiveItemOrSendToBank(toydoll);
        source.SendOrangeBarMessage("Tell WHUG you found the depressed monk.");
        return;
    }
}