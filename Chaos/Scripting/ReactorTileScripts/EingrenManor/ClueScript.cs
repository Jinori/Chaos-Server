using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EingrenManor;

public class ClueScript : ReactorTileScriptBase
{
    private readonly IItemFactory _itemFactory;
    private readonly IMonsterFactory _monsterFactory;
    
    public ClueScript(ReactorTile subject, IItemFactory itemFactory, IMonsterFactory monsterFactory)
        : base(subject)
    {
        _itemFactory = itemFactory;
        _monsterFactory = monsterFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage);

        switch (Subject.MapInstance.Name)
        {
            case "Manor Room 3":
            {
                if (stage == ManorNecklaceStage.AcceptedQuest && !aisling.Inventory.HasCount("Clue One", 1))
                {
                    var clue = _itemFactory.Create("clue1");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        $"You've received the first clue!");
                }
                break;
            }
            case "Manor Room 6":
            {
                if (stage == ManorNecklaceStage.AcceptedQuest && !aisling.Inventory.HasCount("Clue Two", 1))
                {
                    var clue = _itemFactory.Create("clue2");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        $"You've received the second clue!");
                }
                break;
            }
            case "Manor Room 1":
            {
                if (stage == ManorNecklaceStage.AcceptedQuest && !aisling.Inventory.HasCount("Clue Three", 1))
                {
                    var clue = _itemFactory.Create("clue3");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        $"You've received the third clue!");
                }
                break;
            }
            case "Manor Room 7":
            {
                if (stage == ManorNecklaceStage.AcceptedQuest && !aisling.Inventory.HasCount("Clue Four", 1))
                {
                    var clue = _itemFactory.Create("clue4");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        $"You've received the fourth and final clue!");
                }
                break;
            }
            case "Manor Room 8":
            {
                if (stage == ManorNecklaceStage.AcceptedQuest)
                {
                    var monster = _monsterFactory.Create("phasedGhost", aisling.MapInstance, new Point(3, 6));
                    monster.AggroRange = 10;
                    monster.Experience = 2000;
                    var monster2 = _monsterFactory.Create("phasedGhost", aisling.MapInstance, new Point(3, 3));
                    monster2.AggroRange = 10;
                    monster2.Experience = 2000;
                    var monster3 = _monsterFactory.Create("phasedGhost", aisling.MapInstance, new Point(5, 3));
                    monster3.AggroRange = 10;
                    monster3.Experience = 2000;
                    var monster4 = _monsterFactory.Create("phasedGhost", aisling.MapInstance, new Point(5, 5));
                    monster4.AggroRange = 10;
                    monster4.Experience = 2000;
                    aisling.MapInstance.AddObject(monster, monster);
                    aisling.MapInstance.AddObject(monster2, monster2);
                    aisling.MapInstance.AddObject(monster3, monster3);
                    aisling.MapInstance.AddObject(monster4, monster4);

                    var group = aisling.Group?.Where(x => x.WithinRange(aisling)).ToList();
                    
                    if (group is { Count: > 1 })
                    {
                        foreach (var member in group)
                        {
                            var hasNeck = member.Trackers.Enums.TryGetValue(out ManorNecklaceStage neckStage);
                            if (neckStage == ManorNecklaceStage.AcceptedQuest)
                            {
                                var necklace = _itemFactory.Create("zulerasnecklace");
                                member.TryGiveItem(ref necklace);
                                member.Trackers.Enums.Set(ManorNecklaceStage.ObtainedNecklace);
                                member.Client.SendServerMessage(
                                    ServerMessageType.OrangeBar1,
                                    $"You've found Zulera's necklace but have disturbed some ghosts.");   
                            }
                        }
                    }
                    else
                    {
                        var necklace = _itemFactory.Create("zulerasnecklace");
                        aisling.TryGiveItem(ref necklace);
                        aisling.Trackers.Enums.Set(ManorNecklaceStage.ObtainedNecklace);
                        aisling.Client.SendServerMessage(
                            ServerMessageType.OrangeBar1,
                            $"You've found Zulera's necklace but have disturbed some ghosts.");
                    }
                }
                break;
            }
        }
    }
}