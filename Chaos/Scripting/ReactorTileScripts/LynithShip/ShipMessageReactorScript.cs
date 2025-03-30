using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.LynithShip;

public class ShipMessageReactorScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;

    public ShipMessageReactorScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.MapInstance.Name == "Main Hall")
        {
            aisling.SendOrangeBarMessage("The hallways is dark, best not to go there.");

            return;
        }

        if (aisling.MapInstance.Name == "Pirate Ship Deck")
        {
            if (aisling is { X: 14, Y: 8 })
            {
                aisling.SendOrangeBarMessage("You are defending the deck! You can't jump off.");

                return;
            }

            aisling.SendOrangeBarMessage("You're currently defending the ship deck! You can't go inside.");
        }
    }
}