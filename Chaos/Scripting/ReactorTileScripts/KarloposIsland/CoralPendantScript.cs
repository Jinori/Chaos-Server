using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.KarloposIsland;

public class CoralPendantScript : ReactorTileScriptBase
{
    private readonly IItemFactory _itemFactory;
    private readonly IMonsterFactory _monsterFactory;
    
    public CoralPendantScript(ReactorTile subject, IItemFactory itemFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        _itemFactory = itemFactory;
        _monsterFactory = monsterFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);
        
        if ((stage == QueenOctopusQuest.Pendant2) && !aisling.Inventory.HasCount("coral pendant", 1))
        {
            var coralpendant = _itemFactory.Create("coralpendant");
            aisling.TryGiveItem(coralpendant);
            aisling.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "You've Found The Coral Pendant!");
        }

    }
}