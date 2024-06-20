using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Mainstory.Summoner;

public class SummonerMerchantScript : MerchantScriptBase
{
    public enum SummonerState
    {
        Idle,
        Wandering,
        SeenByAislingWithEnum,
        Escape
    }
    
    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IIntervalTimer WalkTimer;
    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public SummonerMerchantScript(Merchant subject, IMonsterFactory monsterFactory, Point point)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        WalkTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.SummonerState = SummonerState.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;

    private void HandleIdleState()
    {
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner)))
            Subject.SummonerState = SummonerState.SeenByAislingWithEnum;

        Subject.SummonerState = SummonerState.Wandering;

        ActionTimer.Reset();
    }

    private void HandleSummonerEscape(TimeSpan delta)
    {
        WalkTimer.Update(delta);

        if (!WalkTimer.IntervalElapsed)
            return;
        
        var escapePoints = new Point[]
        {
            new Point(181, 141),
            new Point(26, 195),
            new Point(195, 19)
        };

        var closestPoint = escapePoints.OrderBy(point2 => point2.WithinRange(Subject)).FirstOrDefault();

        if (!Subject.WithinRange(closestPoint, 2))
            Subject.Pathfind(closestPoint);
        else
        {
            var ani = new Animation
            {
                TargetAnimation = 78,
                AnimationSpeed = 300
            };
            Subject.Animate(ani);
            Subject.MapInstance.RemoveEntity(Subject);
        }
    }

    private void HandleWanderingState()
    {
        Subject.Wander();

        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner)))
            Subject.SummonerState = SummonerState.SeenByAislingWithEnum;

        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            Subject.SummonerState = SummonerState.Idle;
            ActionTimer.Reset();
        }

        Subject.WanderTimer.Reset();
    }

    private static string PickRandom(ICollection<string> phrases) => phrases.PickRandom();

    private void HandleSummonerConversation(TimeSpan delta)
    {
        DialogueTimer.Update(delta);

        if (!DialogueTimer.IntervalElapsed) return;
        
        if (!HasSaidGreeting)
        {
            Subject.Turn(Direction.Up);
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
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner)).ToList();
            foreach (var aisling in aislings)
            {
                aisling.SendOrangeBarMessage("Summoner Kades summons his Servant and escapes.");
            }

            var ani = new Animation
            {
                TargetAnimation = 52,
                AnimationSpeed = 300
            };
            
            var servantpoint = new Point(Subject.X, Subject.Y);
            var servant = MonsterFactory.Create("servant", Subject.MapInstance, servantpoint);
            Subject.MapInstance.AddEntity(servant, servantpoint);
            servant.Animate(ani);

            Subject.SummonerState = SummonerState.Escape;
            HasSaidDialog3 = true;
            DialogueTimer.Reset();
        }
    }


    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        
        switch (Subject.SummonerState)
        {
            case SummonerState.Idle:
            {
                HandleIdleState();
                break;
            }

            case SummonerState.Wandering:
            {
                HandleWanderingState();
                break;
            }

            case SummonerState.SeenByAislingWithEnum:
            {
                HandleSummonerConversation(delta);
                break;
            }
            case SummonerState.Escape:
            {
                HandleSummonerEscape(delta);
                break;
            }
        }
    }
    private static readonly List<string> Greeting =
    [
        "You are very bold Aislings. Coming into my home threatening me...",
    ];

    private static readonly List<string> Dialog1 =
    [
        "This world is mine now. There's nothing you can do to stop me.",
    ];

    private static readonly List<string> Dialog2 =
    [
        "I have other things to tend to, Servant! Get them!"
    ];
}