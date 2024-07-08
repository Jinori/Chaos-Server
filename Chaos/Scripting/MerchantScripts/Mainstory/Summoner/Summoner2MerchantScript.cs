using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Mainstory.Summoner;

public class Summoner2MerchantScript : MerchantScriptBase
{
    public enum SummonerState2
    {
        Idle,
        SeenByAislingWithEnum,
        Fight
    }
    
    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IIntervalTimer WalkTimer;
    private readonly IIntervalTimer PortalClose;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IMerchantFactory MerchantFactory;
    private readonly IReactorTileFactory ReactorTileFactory;

    /// <inheritdoc />
    public Summoner2MerchantScript(Merchant subject, IMonsterFactory monsterFactory, IReactorTileFactory reactorTileFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ReactorTileFactory = reactorTileFactory;
        MerchantFactory = merchantFactory;
        PortalClose = new IntervalTimer(TimeSpan.FromSeconds(2), false);
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(800), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);
        Subject.SummonerState2 = SummonerState2.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;
    public bool HasSaidDialog4;
    public bool HasSummonedCreants;
    public bool HasOpenedPortals;
    public bool CreantsSummoned;
    public bool HasCreantsEscaped;

    private void HandleIdleState()
    {
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2) ||
                      x.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory)))
        {
            Subject.SummonerState2 = SummonerState2.SeenByAislingWithEnum;

            var players = Subject.MapInstance.GetEntities<Aisling>().Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2) && !x.IsGodModeEnabled()).ToList();

            foreach (var player in players)
            {
                player.Trackers.Enums.Set(MainStoryEnums.FoundSummoner2);
            }

            return;
        }

        ActionTimer.Reset();
    }

    private void HandleSummonerConversation(TimeSpan delta)
    {
        DialogueTimer.Update(delta);

        if (!DialogueTimer.IntervalElapsed) return;
        
        if (!HasSaidGreeting)
        {
            Subject.Say("Tagaigí anois, a chlann, cuirfimid an domhan glan.");
            HasSaidGreeting = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog1 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("Glacfaimid an domhan le chéile.");
            HasSaidDialog1 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog2 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("Is anois ár n-am! Lean mise isteach sa dorchadas.");
            HasSaidDialog2 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSummonedCreants)
        {
            var ani = new Animation
            {
                TargetAnimation = 52,
                AnimationSpeed = 300
            };
            
            var point1 = new Point(13, 14);
            var point2 = new Point(15, 16);
            var point3 = new Point(17, 14);
            var point4 = new Point(15, 12);
            var merchant1 = MerchantFactory.Create("phoenix_merchant", Subject.MapInstance, point1);
            var merchant2 = MerchantFactory.Create("tauren_merchant", Subject.MapInstance, point1);
            var merchant3 = MerchantFactory.Create("shamensyth_merchant", Subject.MapInstance, point1);
            var merchant4 = MerchantFactory.Create("medusa_merchant", Subject.MapInstance, point1);
            Subject.MapInstance.AddEntity(merchant1, point1);
            Subject.MapInstance.AddEntity(merchant2, point2);
            Subject.MapInstance.AddEntity(merchant3, point3);
            Subject.MapInstance.AddEntity(merchant4, point4);
            merchant1.Animate(ani);
            merchant2.Animate(ani);
            merchant3.Animate(ani);
            merchant4.Animate(ani);
            HasSummonedCreants = true;
            DialogueTimer.Reset();
        }
        else if (!CreantsSummoned)
        {
            Subject.Say("Téigh chuig bhur dtithe cearta, beidh mé ann go luath.");
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.Trackers.Enums.Set(MainStoryEnums.SpawnedCreants);
                player.SendOrangeBarMessage("You witness the summoning of the Creants.");
            }

            CreantsSummoned = true;
            DialogueTimer.Reset();
        }
        else if (!HasOpenedPortals)
        {
            var point1 = new Point(15, 6);
            var point2 = new Point(8, 14);
            var point3 = new Point(15, 21);
            var point4 = new Point(22, 14);
            var portal1 = ReactorTileFactory.Create("portalanimation2", Subject.MapInstance, point1);
            var portal2 = ReactorTileFactory.Create("portalanimation2", Subject.MapInstance, point2);
            var portal3 = ReactorTileFactory.Create("portalanimation2", Subject.MapInstance, point3);
            var portal4 = ReactorTileFactory.Create("portalanimation2", Subject.MapInstance, point4);
            Subject.MapInstance.SimpleAdd(portal1);
            Subject.MapInstance.SimpleAdd(portal2);
            Subject.MapInstance.SimpleAdd(portal3);
            Subject.MapInstance.SimpleAdd(portal4);
            
            Subject.Say("Caithfidh mé aire a thabhairt do na trioblóidí seo.");
            
            Subject.Say("Téigh chuig bhur dtithe cearta, beidh mé ann go luath.");
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.Trackers.Enums.Set(MainStoryEnums.SpawnedCreants);
                player.SendOrangeBarMessage("You witness the summoning of the Creants.");
            }
            
            HasOpenedPortals = true;
            DialogueTimer.Reset();
        }
        else if (!HasCreantsEscaped && DialogueTimer.IntervalElapsed)
        {
            if (!Subject.MapInstance.GetEntities<Merchant>().Any(x =>
                    x.Name != "Tauren" && x.Name != "Phoenix" && x.Name != "Medusa" && x.Name != "Shamensyth")) return;
            
            foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => !x.IsGodModeEnabled() && x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)))
            {
                player.Trackers.Enums.Set(MainStoryEnums.StartedSummonerFight);
            }
            
            Subject.Say("My babies have returned to the world.");
            HasCreantsEscaped = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog4 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("You have been following me, it is time to perish.");
            HasSaidDialog4 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog3 && DialogueTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants) || x.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory)).ToList();
            foreach (var aisling in aislings)
            {
                aisling.SendOrangeBarMessage("Summoner Kades becomes hostile.");
            }

            var ani = new Animation
            {
                TargetAnimation = 52,
                AnimationSpeed = 300
            };
            
            var summoner = new Point(Subject.X, Subject.Y);
            var summonermonster = MonsterFactory.Create("summoner_boss", Subject.MapInstance, summoner);
            Subject.MapInstance.AddEntity(summonermonster, summoner);
            summonermonster.Animate(ani);
            
            HasSaidDialog3 = true;
            DialogueTimer.Reset();
            Subject.MapInstance.RemoveEntity(Subject);
        }
    }


    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;
        
        switch (Subject.SummonerState2)
        {
            case SummonerState2.Idle:
            {
                HandleIdleState();
                break;
            }

            case SummonerState2.SeenByAislingWithEnum:
            {
                HandleSummonerConversation(delta);
                break;
            }
        }
    }
}