using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Piet;

public class PietWerewolfMerchantScript : MerchantScriptBase
{
    public enum PietWerewolfState
    {
        Idle,
        Wandering,
        SeenByAislingWithEnum,
        InsideHouse
    }

    private IPathOptions Options => PathOptions.Default with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true
    };



    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IMonsterFactory MonsterFactory;
    public readonly Location PietEmptyHouseDoorInsidePoint = new("piet_empty_room2", 5, 10);
    public readonly Location PietEmptyRoomDoorPoint = new("piet", 30, 12);

    private readonly IIntervalTimer WalkTimer;
    private readonly IIntervalTimer FasterWalkTimer;

    /// <inheritdoc />
    public PietWerewolfMerchantScript(Merchant subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        FasterWalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.PietWerewolfState = PietWerewolfState.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;
    public bool HasSaidDialog4;
    
    private void HandleApproachToEmptyRoom(TimeSpan delta)
    {
        if (ShouldWalkTo(PietEmptyRoomDoorPoint))
            WalkTowards(PietEmptyRoomDoorPoint, delta);
        else if (ShouldWalkTo(PietEmptyHouseDoorInsidePoint))
            WalkTowards(PietEmptyHouseDoorInsidePoint, delta);
    }
    
    private bool ShouldWalkTo(Location destination) =>
        (Subject.DistanceFrom(destination) > 0) && Subject.OnSameMapAs(destination);
    
    private void AttemptToOpenDoor(IPoint doorPoint)
    {
        var door = Subject.MapInstance.GetEntitiesAtPoint<Door>(doorPoint).FirstOrDefault();

        if (door is { Closed: true })
            door.OnClicked(Subject);
    }

    private void HandleIdleState()
    {
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
            Subject.PietWerewolfState = PietWerewolfState.SeenByAislingWithEnum;
        
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject).Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant)))
            Subject.PietWerewolfState = PietWerewolfState.InsideHouse;

        Subject.PietVillagerState = PietVillagerScript.PietVillagerState.Wandering;

        ActionTimer.Reset();
    }

    private void HandleWanderingState()
    {
        Subject.Wander();

        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
            Subject.PietWerewolfState = PietWerewolfState.SeenByAislingWithEnum;

        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            Subject.PietWerewolfState = PietWerewolfState.Idle;
            ActionTimer.Reset();
        }

        Subject.WanderTimer.Reset();
    }

    private void HandleSeenByAislingWithEnumState(TimeSpan delta)
    {
        if (Subject.MapInstance.Name == "Piet Empty Room")
        {
            Subject.PietWerewolfState = PietWerewolfState.InsideHouse;
            return;
        }

        if (Subject.WithinRange(PietEmptyRoomDoorPoint, 1))
        {
            var aislingsOnQuest = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
                .Where(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)).ToList();

            foreach (var aisling in aislingsOnQuest)
            {
                // Assign the FollowedMerchant enum to the aisling
                aisling.Trackers.Enums.Set(WerewolfOfPiet.FollowedMerchant);
                aisling.SendOrangeBarMessage("That must be the werewolf's hut!");

                // Check if aisling is in a group
                if (aisling.Group != null)
                {
                    // Get group members who are on the map and have started the quest
                    var groupMembers = aisling.Group
                        .Where(member => member.MapInstance == Subject.MapInstance &&
                                         member.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest))
                        .ToList();

                    // Assign the FollowedMerchant enum to group members
                    foreach (var member in groupMembers)
                    {
                        member.Trackers.Enums.Set(WerewolfOfPiet.FollowedMerchant);
                        member.SendOrangeBarMessage("That must be the werewolf's hut!");
                    }
                }
            }

            Subject.MapInstance.RemoveEntity(Subject);
        }
        HandleApproachToEmptyRoom(delta);
    }

    private void WalkTowards(Location destination, TimeSpan delta)
    {
        WalkTimer.Update(delta);
        FasterWalkTimer.Update(delta);
        
        if (FasterWalkTimer.IntervalElapsed && Subject.MapInstance.Name == "Piet")
        {
            if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 4)
                .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
            {
                Subject.Pathfind(destination, 0, Options);
            }
        }
        
        if (WalkTimer.IntervalElapsed)
        {
            if (destination == PietEmptyRoomDoorPoint)
                AttemptToOpenDoor(PietEmptyRoomDoorPoint);

            Subject.Pathfind(destination, 0, Options);
        }
    }
    private void HandleInsideHouse(TimeSpan delta)
    {
        var point = new Point(5, 10);
        
        if (!point.Equals(Subject))
            WalkTowards(PietEmptyHouseDoorInsidePoint, delta);

        if (point.Equals(Subject)) 
            HandleWerewolfConversation(delta);

    }

    private static string PickRandom(ICollection<string> phrases) => phrases.PickRandom();

    private void HandleWerewolfConversation(TimeSpan delta)
    {
        DialogueTimer.Update(delta);

        if (!DialogueTimer.IntervalElapsed) return;
        
        if (!HasSaidGreeting)
        {
            Subject.Turn(Direction.Right);
            var greeting = string.Format(PickRandom(Greeting));
            Subject.Say(greeting);
            HasSaidGreeting = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog1 && DialogueTimer.IntervalElapsed)
        {
            var dialog1 = PickRandom(Dialog1);
            Subject.Say(dialog1);
            HasSaidDialog1 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog2 && DialogueTimer.IntervalElapsed)
        {
            var greeting = string.Format(PickRandom(Dialog2));
            Subject.Say(greeting);
            HasSaidDialog2 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog3 && DialogueTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant)).ToList();
            foreach (var aisling in aislings)
            {
                aisling.SendOrangeBarMessage("You watch as Toby's body warps unnaturally.");
            }

            HasSaidDialog3 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog4 && DialogueTimer.IntervalElapsed)
        {
            SpawnMonsters("werewolfTobyMonster", 1);
            Subject.MapInstance.RemoveEntity(Subject);
            HasSaidDialog4 = true;
            DialogueTimer.Reset();
        }
    }
    
    private void SpawnMonsters(string monsterType, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var point = new Point(5, 10);
                var groupLevel = Subject.MapInstance.GetEntities<Aisling>().Select(aisling => aisling.StatSheet.Level).ToList();
                var monster = MonsterFactory.Create(monsterType, Subject.MapInstance, point);

                // Calculate the excess vitality over 20,000 for any player
                var excessVitality = Subject.MapInstance.GetEntities<Aisling>()
                    .Where(aisling => !aisling.Trackers.Enums.HasValue(GodMode.Yes))
                    .Select(aisling => (aisling.StatSheet.MaximumHp + aisling.StatSheet.MaximumMp) - 20000)
                    .Where(excess => excess > 0)
                    .Sum(excess => excess / 1000);

                var attrib = new Attributes
                {
                    AtkSpeedPct = groupLevel.Count * 8 + 3,
                    MaximumHp = (int)(monster.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 600),
                    MaximumMp = (int)(monster.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 600),
                    Int = (int)(monster.StatSheet.Int + groupLevel.Average() * groupLevel.Count / 8),
                    Str = (int)(monster.StatSheet.Str + groupLevel.Average() * groupLevel.Count / 6),
                    SkillDamagePct = (int)(monster.StatSheet.SkillDamagePct + groupLevel.Average() / 3 +
                                           groupLevel.Count + 20),
                    SpellDamagePct = (int)(monster.StatSheet.SpellDamagePct + groupLevel.Average() / 3 +
                                           groupLevel.Count + 20)
                };

                // Adjust the attributes based on the excess vitality
                if (excessVitality > 0)
                {
                    // Example adjustments: increase stats by a percentage of the excess vitality
                    attrib = attrib with
                    {
                        MaximumHp = (int)(excessVitality * 0.05)
                    }; // 5% of excess vitality added to HP
                    attrib = attrib with
                    {
                        MaximumMp = (int)(excessVitality * 0.05)
                    }; // 5% of excess vitality added to MP
                    attrib.Int += (int)(excessVitality * 0.01); // 1% of excess vitality added to Int
                    attrib.Str += (int)(excessVitality * 0.01); // 1% of excess vitality added to Str
                    attrib.SkillDamagePct +=
                        (int)(excessVitality * 0.01); // 1% of excess vitality added to Skill Damage
                    attrib.SpellDamagePct +=
                        (int)(excessVitality * 0.01); // 1% of excess vitality added to Spell Damage
                }

                // Add the attributes to the monster
                monster.StatSheet.AddBonus(attrib);
                // Add HP and MP to the monster
                monster.StatSheet.SetHealthPct(100);
                monster.StatSheet.SetManaPct(100);
                // Add the monster to the subject
                Subject.MapInstance.AddEntity(monster, monster);
            }
        }


    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        
        switch (Subject.PietWerewolfState)
        {
            case PietWerewolfState.Idle:
            {
                HandleIdleState();
                break;
            }

            case PietWerewolfState.Wandering:
            {
                HandleWanderingState();
                break;
            }

            case PietWerewolfState.SeenByAislingWithEnum:
            {
                HandleSeenByAislingWithEnumState(delta);
                break;
            }

            case PietWerewolfState.InsideHouse:
            {
                HandleInsideHouse(delta);
                break;
            }
        }
    }
    private static readonly List<string> Greeting =
    [
        "Oh goody, you followed me. I was out looking for a meal",
        "This is my home! You don't belong here!",
        "I am surprised you followed, good...",
        "I didn't expect you to follow, that is good."
    ];

    private static readonly List<string> Dialog1 =
    [
        "I love visitors! It's time to meet your fate.",
        "Visitors don't come here often, you'll see why.",
        "I miss having visitors, they're tasty.",
        "Did you wish to die here?"
    ];

    private static readonly List<string> Dialog2 =
    [
        "Argggggh I can't control this any longer...",
        "RwaaAAaRRr you should escape now...",
        "GrawWWwwgh I think you should go...",
        "SKRAHHHH It... is... time..."
    ];
}