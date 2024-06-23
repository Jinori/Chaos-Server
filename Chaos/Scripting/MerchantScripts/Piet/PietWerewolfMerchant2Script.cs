using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Piet;

public class PietWerewolfMerchant2Script : MerchantScriptBase
{
    public enum PietWerewolfState2
    {
        Idle,
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

    private readonly IIntervalTimer WalkTimer;

    /// <inheritdoc />
    public PietWerewolfMerchant2Script(Merchant subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.PietWerewolfState2 = PietWerewolfState2.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;
    public bool HasSaidDialog4;

    private void HandleIdleState()
    {
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject).Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant)))
            Subject.PietWerewolfState2 = PietWerewolfState2.InsideHouse;

        ActionTimer.Reset();
    }

    private void WalkTowards(Location destination, TimeSpan delta)
    {
        WalkTimer.Update(delta);
        
        if (WalkTimer.IntervalElapsed)
        {
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
            SpawnMonsters("werewolfTobyMonster");
            Subject.MapInstance.RemoveEntity(Subject);
            HasSaidDialog4 = true;
            DialogueTimer.Reset();
        }
    }
    
    private void SpawnMonsters(string monsterType)
        {
            var point = new Point(5, 10);
            var monster = MonsterFactory.Create(monsterType, Subject.MapInstance, point);
            Subject.MapInstance.AddEntity(monster, monster);
        }


    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        
        switch (Subject.PietWerewolfState2)
        {
            case PietWerewolfState2.Idle:
            {
                HandleIdleState();
                break;
            }

            case PietWerewolfState2.SeenByAislingWithEnum:
            {
                if (Subject.MapInstance.GetEntities<Aisling>().Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant)))
                    Subject.PietWerewolfState2 = PietWerewolfState2.InsideHouse;
                
                break;
            }

            case PietWerewolfState2.InsideHouse:
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