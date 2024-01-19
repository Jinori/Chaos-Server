using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
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

        aisling.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage);

        switch (Subject.MapInstance.Name)
        {
            case "Manor Room 3":
            {
                if ((stage == ManorNecklaceStage.AcceptedQuest) && !aisling.Inventory.HasCount("Clue One", 1))
                {
                    var clue = _itemFactory.Create("clue1");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "You've received the first clue!");
                }

                break;
            }
            case "Manor Room 6":
            {
                if ((stage == ManorNecklaceStage.AcceptedQuest) && !aisling.Inventory.HasCount("Clue Two", 1))
                {
                    var clue = _itemFactory.Create("clue2");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "You've received the second clue!");
                }

                break;
            }
            case "Manor Room 1":
            {
                if ((stage == ManorNecklaceStage.AcceptedQuest) && !aisling.Inventory.HasCount("Clue Three", 1))
                {
                    var clue = _itemFactory.Create("clue3");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "You've received the third clue!");
                }

                break;
            }
            case "Manor Room 7":
            {
                if ((stage == ManorNecklaceStage.AcceptedQuest) && !aisling.Inventory.HasCount("Clue Four", 1))
                {
                    var clue = _itemFactory.Create("clue4");
                    aisling.TryGiveItem(ref clue);

                    aisling.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "You've received the fourth clue!");
                }

                break;
            }
            case "Manor Room 8":
            {
                if (stage is ManorNecklaceStage.AcceptedQuest or ManorNecklaceStage.SawNecklace)
                {
                    // Check if the group is null or has only one member
                    if (aisling.Group is null || aisling.Group.Any(x => !x.OnSameMapAs(aisling) || !x.WithinRange(aisling)))
                    {
                        // Send a message to the Aisling
                        aisling.Client.SendServerMessage(
                            ServerMessageType.OrangeBar1,
                            "Make sure you are grouped or your group is near you.");

                        // Warp the source back
                        var point = source.DirectionalOffset(source.Direction.Reverse());
                        source.WarpTo(point);

                        return;
                    }

                    // Check if all members of the group have the quest flag and are within level range
                    // Check if all members of the group have the quest enum and are within level range
                    var allMembersHaveQuestFlag = aisling.Group.All(
                        member =>
                            member.Trackers.Enums.TryGetValue(out ManorNecklaceStage value)
                            && value is ManorNecklaceStage.AcceptedQuest or ManorNecklaceStage.SawNecklace
                            && member.WithinLevelRange(source));

                    // Check if all members have all four clues
                    var allMembersHaveAllClues = aisling.Group.All(
                        member => member.Inventory.HasCount("Clue One", 1)
                                  && member.Inventory.HasCount("Clue Two", 1)
                                  && member.Inventory.HasCount("Clue Three", 1)
                                  && member.Inventory.HasCount("Clue Four", 1));

                    if (allMembersHaveQuestFlag && allMembersHaveAllClues)
                    {
                        var monster = _monsterFactory.Create("airphasedGhost", aisling.MapInstance, new Point(3, 6));
                        monster.AggroRange = 10;
                        monster.Experience = 2000;
                        var monster2 = _monsterFactory.Create("earthphasedGhost", aisling.MapInstance, new Point(3, 3));
                        monster2.AggroRange = 10;
                        monster2.Experience = 2000;
                        var monster3 = _monsterFactory.Create("waterphasedGhost", aisling.MapInstance, new Point(5, 3));
                        monster3.AggroRange = 10;
                        monster3.Experience = 2000;
                        var monster4 = _monsterFactory.Create("firephasedGhost", aisling.MapInstance, new Point(5, 5));
                        monster4.AggroRange = 10;
                        monster4.Experience = 2000;
                        aisling.MapInstance.AddEntity(monster, monster);
                        aisling.MapInstance.AddEntity(monster2, monster2);
                        aisling.MapInstance.AddEntity(monster3, monster3);
                        aisling.MapInstance.AddEntity(monster4, monster4);

                        foreach (var member in aisling.Group)
                        {
                            member.Trackers.Enums.Set(ManorNecklaceStage.SawNecklace);

                            member.Client.SendServerMessage(
                                ServerMessageType.OrangeBar1,
                                "You catch a glimpse of the necklace before it disappears.");
                        }
                    }
                    else
                    {
                        // Send a message to the Aisling
                        aisling.Client.SendServerMessage(
                            ServerMessageType.OrangeBar1,
                            "Make sure everyone is within level range and has all four clues.");

                        // Warp the source back
                        var point = source.DirectionalOffset(source.Direction.Reverse());
                        source.WarpTo(point);
                    }
                }

                break;
            }
        }
    }
}