using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
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
        SeenByAislingWithEnum,
        Escape
    }

    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer PortalClose;
    private readonly IReactorTileFactory ReactorTileFactory;
    private readonly IIntervalTimer WalkTimer;
    private Point EscapePoint;
    public bool HasEscapePoint;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;

    public bool HasSaidGreeting;
    public bool PortalOpened;
    private Point ReactorPoint;

    /// <inheritdoc />
    public SummonerMerchantScript(Merchant subject, IMonsterFactory monsterFactory, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ReactorTileFactory = reactorTileFactory;
        PortalClose = new IntervalTimer(TimeSpan.FromSeconds(2), false);

        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(800), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.SummonerState = SummonerState.Idle;
    }

    private void HandleIdleState()
    {
        if (Subject.MapInstance
                   .GetEntitiesWithinRange<Aisling>(Subject)
                   .Any(
                       x => x.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor)
                            || (x.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3) && !x.IsGodModeEnabled())))
            Subject.SummonerState = SummonerState.SeenByAislingWithEnum;

        ActionTimer.Reset();
    }

    private void HandleSummonerConversation(TimeSpan delta)
    {
        DialogueTimer.Update(delta);

        if (!DialogueTimer.IntervalElapsed)
            return;

        if (!HasSaidGreeting)
        {
            Subject.Say("You are very bold Aislings. Coming into my home...");
            HasSaidGreeting = true;
            DialogueTimer.Reset();
        } else if (!HasSaidDialog1 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("This world is mine now. Return to which you came.");
            HasSaidDialog1 = true;
            DialogueTimer.Reset();
        } else if (!HasSaidDialog2 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("I have other things to tend to, Servant! Get them!");
            HasSaidDialog2 = true;
            DialogueTimer.Reset();
        } else if (!HasSaidDialog3 && DialogueTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance
                                  .GetEntities<Aisling>()
                                  .Where(
                                      x => x.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor)
                                           || x.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3))
                                  .ToList();

            foreach (var aisling in aislings)
                aisling.SendOrangeBarMessage("Summoner Kades summons his Servant and escapes.");

            var ani = new Animation
            {
                TargetAnimation = 52,
                AnimationSpeed = 300
            };

            var servantpoint = new Point(Subject.X - 2, Subject.Y + 4);
            var servant = MonsterFactory.Create("em_servant", Subject.MapInstance, servantpoint);
            Subject.MapInstance.AddEntity(servant, servantpoint);
            servant.Animate(ani);

            Subject.SummonerState = SummonerState.Escape;
            HasSaidDialog3 = true;
            DialogueTimer.Reset();
        }
    }

    private void HandleSummonerEscape(TimeSpan delta)
    {
        WalkTimer.Update(delta);
        PortalClose.Update(delta);

        if (WalkTimer.IntervalElapsed)
        {
            if (!HasEscapePoint)
            {
                EscapePoint = new Point(Subject.X - 4, Subject.Y - 4); // Set the escape point three spaces away
                HasEscapePoint = true;
            }

            Subject.Pathfind(EscapePoint);

            if (Subject.WithinRange(EscapePoint, 1))
            {
                if (!PortalOpened)
                {
                    ReactorPoint = new Point(Subject.X - 2, Subject.Y - 2);
                    var reactortile = ReactorTileFactory.Create("portalanimation", Subject.MapInstance, ReactorPoint);
                    Subject.MapInstance.SimpleAdd(reactortile);
                    PortalOpened = true;
                } else if (PortalOpened)
                {
                    PortalClose.Update(delta);
                    Subject.Pathfind(ReactorPoint);
                }
            }

            if (PortalOpened && Subject.WithinRange(ReactorPoint, 2))
                Subject.MapInstance.RemoveEntity(Subject);
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
}