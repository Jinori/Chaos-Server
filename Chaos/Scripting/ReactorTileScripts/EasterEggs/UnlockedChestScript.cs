using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EasterEggs;

public sealed class UnlockedChestScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public UnlockedChestScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        
        if (source.Trackers.Flags.HasFlag(Definitions.EasterEggs.UnlockedChest))
        {
            source.SendOrangeBarMessage("You've already found this.");
            return;
        }
        
        source.Trackers.Flags.AddFlag(Definitions.EasterEggs.UnlockedChest);
        var toydoll = ItemFactory.Create("toydoll");
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "This chest is unlocked! You open and rummage through its contents.");
        source.TryGiveGamePoints(5);
        source.TryGiveGold(20000);
        source.GiveItemOrSendToBank(toydoll);
        source.SendOrangeBarMessage("Do not share Easter Eggs with others.");
        return;
    }
}