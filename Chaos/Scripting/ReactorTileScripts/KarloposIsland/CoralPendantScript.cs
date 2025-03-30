using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.KarloposIsland;

public class CoralPendantScript : ReactorTileScriptBase
{
    private readonly IItemFactory _itemFactory;

    public CoralPendantScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        aisling.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);

        if ((stage == QueenOctopusQuest.Pendant2) && !aisling.Inventory.HasCount("coral pendant", 1))
        {
            var coralpendant = _itemFactory.Create("coralpendant");
            aisling.GiveItemOrSendToBank(coralpendant);
            source.Trackers.Enums.Set(QueenOctopusQuest.Pendant3);

            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've found the Coral Pendant!");
        }
    }
}