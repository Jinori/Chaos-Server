using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
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
    }
    
    private readonly IIntervalTimer UpdateTimer;
    private readonly IIntervalTimer DialogueTimer;
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
        DialogueTimer = new IntervalTimer(TimeSpan.FromSeconds(7),false);
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);
        Subject.SummonerState2 = SummonerState2.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;
    public bool HasSaidDialog4;
    public bool HasSummonedTauren;
    public bool HasSummonedPhoenix;
    public bool HasSummonedSham;
    public bool HasSummonedMedusa;
    public bool HasOpenedPortals;
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

        UpdateTimer.Reset();
    }

    private void HandleSummonerConversation(TimeSpan delta)
    {
        if (!DialogueTimer.IntervalElapsed) return;
        
        if (!HasSaidGreeting)
        {
            Subject.Say("Tá sé deas duit teacht chun féachaint.");
            foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
            {
                player.SendMessage("Summoner Kades: It is nice of you to come watch.");
            }
            HasSaidGreeting = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog1 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("Glacfaimid Unora le chéile, na Creants agus mé féin.");
            foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
            {
                player.SendMessage("Summoner Kades: The Creants and I will take Unora together.");
            }
            HasSaidDialog1 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog2 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("Is anois ár n-am! Lean mise isteach sa dorchadas.");
            foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
            {
                player.SendMessage("Summoner Kades: Now is our time! Follow me into the darkness.");
            }
            HasSaidDialog2 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSummonedTauren && DialogueTimer.IntervalElapsed)
        {
            var ani = new Animation
            {
                TargetAnimation = 614,
                AnimationSpeed = 2500
            };
            var point2 = new Point(15, 16);
            var merchant2 = MerchantFactory.Create("tauren_merchant", Subject.MapInstance, point2);
            Subject.MapInstance.AddEntity(merchant2, point2);
            merchant2.Animate(ani);
            
            Subject.Say("Eirigh, a chara dhil, Tauren, Eilimint an Domhain.");
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.SendMessage("Summoner Kades: Rise, my dear friend, Tauren, Element of the Earth.");
                player.SendOrangeBarMessage("The Tauren Creant appears before your eyes.");
            }
            HasSummonedTauren = true;
            DialogueTimer.Reset();
        }
        else if (!HasSummonedPhoenix && DialogueTimer.IntervalElapsed)
        {
            var ani = new Animation
            {
                TargetAnimation = 640,
                AnimationSpeed = 2500
            };
            
            var point1 = new Point(13, 14);
            var merchant1 = MerchantFactory.Create("phoenix_merchant", Subject.MapInstance, point1);
            Subject.MapInstance.AddEntity(merchant1, point1);
            merchant1.Animate(ani);
            
            Subject.Say("Tar anuas go Unora, Lady Phoenix, Eilimint an Ghaoith.");
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.SendMessage("Summoner Kades: Come down to Unora, Lady Phoenix, Element of the Wind.");
                player.SendOrangeBarMessage("Lady Phoenix appears from the sky.");
            }
            HasSummonedPhoenix = true;
            DialogueTimer.Reset();
        }
        else if (!HasSummonedSham && DialogueTimer.IntervalElapsed)
        {
            var ani = new Animation
            {
                TargetAnimation = 847,
                AnimationSpeed = 2500
            };
            
            var point3 = new Point(17, 14);
            var merchant3 = MerchantFactory.Create("shamensyth_merchant", Subject.MapInstance, point3);
            Subject.MapInstance.AddEntity(merchant3, point3);
            merchant3.Animate(ani);
            
            Subject.Say("Rugadh as na lasracha, Shamensyth, Eilimint an Dóiteáin.");
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.SendMessage("Summoner Kades: Born of the flames, Shamensyth, Element of Fire.");
                player.SendOrangeBarMessage("Shamensyth rises from a quick flame.");
            }
            HasSummonedSham = true;
            DialogueTimer.Reset();
        }
        else if (!HasSummonedMedusa && DialogueTimer.IntervalElapsed)
        {
            var ani = new Animation
            {
                TargetAnimation = 625,
                AnimationSpeed = 2500
            };
            
            var point4 = new Point(15, 12);
            var merchant4 = MerchantFactory.Create("medusa_merchant", Subject.MapInstance, point4);
            Subject.MapInstance.AddEntity(merchant4, point4);
            merchant4.Animate(ani);
            
            Subject.Say("Rí na Farraige, Medusa, Eilimint an Uisce.");
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.SendMessage("Summoner Kades: Queen of the Sea, Medusa, Element of Water.");
                player.SendOrangeBarMessage("A wave washes over the area and Medusa appears.");
            }
            HasSummonedMedusa = true;
            DialogueTimer.Reset();
        }
        else if (!HasOpenedPortals && DialogueTimer.IntervalElapsed)
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
            
            Subject.Say("Téigh abhaile. Caithfidh mé na trioblóidí seo a mharú.");
            
            var players = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)).ToList();

            foreach (var player in players)
            {
                player.SendMessage("Summoner Kades: Return home. I must kill these troubles.");
                player.Trackers.Enums.Set(MainStoryEnums.SpawnedCreants);
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
                player.Trackers.Enums.Set(SummonerBossFight.StartedSummonerFight);
            }
            
            Subject.Say("My friends have returned to the world.");
            HasCreantsEscaped = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog3 && DialogueTimer.IntervalElapsed)
        {
            Subject.Say("You have been following me, it is time to perish.");
            HasSaidDialog3 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog4 && DialogueTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight) || x.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory)).ToList();
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
            
            HasSaidDialog4 = true;
            DialogueTimer.Reset();
            Subject.MapInstance.RemoveEntity(Subject);
        }
    }

    public override void Update(TimeSpan delta)
    {
        DialogueTimer.Update(delta);
        
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