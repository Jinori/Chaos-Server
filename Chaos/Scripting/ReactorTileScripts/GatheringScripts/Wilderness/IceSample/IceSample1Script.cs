using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.IceSample1;

public class IceSample1Script : ReactorTileScriptBase
{
    private readonly IItemFactory _itemFactory;
    
    public IceSample1Script(ReactorTile subject, IItemFactory itemFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        _itemFactory = itemFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out IceWallQuest stage);

        switch (Subject.MapInstance.Name)
        {
            case "Wilderness":
            {
                if (stage == IceWallQuest.Start && !aisling.Inventory.HasCount("Ice Sample 1", 1))
                {
                    var sample = _itemFactory.Create("icesample1");
                    aisling.TryGiveItem(sample);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        $"You've collected an ice sample!");
                }

                break;
            }
        }
    }
}