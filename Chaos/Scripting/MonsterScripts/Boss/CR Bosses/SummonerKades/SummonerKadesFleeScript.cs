using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CR_Bosses.SummonerKades;

public sealed class SummonerKadesFleeScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IReactorTileFactory ReactorTileFactory;
    private bool PortalOpened;
    private readonly IIntervalTimer WalkTimer;
    private Point PortalPoint;
    private bool HitFirstHp;
    private bool HitSecondHp;
    private bool HitThirdHp;
    private bool HitFourthHp;
    private bool HitFifthHp;

    /// <inheritdoc />
    public SummonerKadesFleeScript(Monster subject, IMonsterFactory monsterFactory, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ReactorTileFactory = reactorTileFactory;
        WalkTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    public override void Update(TimeSpan delta)
    {
        WalkTimer.Update(delta);

        if (Subject.MapInstance.Name == "Cthonic Domain" && Subject.StatSheet.CurrentHp <= 1508320)
        {
            HitFirstHp = true;
        }
        
        if (HitFirstHp)
        {
            Subject.StatSheet.SetHp(1508320);
            
            if (!WalkTimer.IntervalElapsed)
                return;
            
            if (!PortalOpened)
            {
                var rectangle = new Rectangle(Subject.X, Subject.Y, 7, 7);
                PortalPoint = rectangle.GetRandomPoint();
                PortalOpened = true;
                var portal = ReactorTileFactory.Create("SummonerEscapePortal", Subject.MapInstance, PortalPoint);
                Subject.MapInstance.SimpleAdd(portal);
                Subject.RemoveScript<MoveToTargetScript>();
                Subject.RemoveScript<AggroTargetingScript>();
                Subject.RemoveScript<AttackingScript>();
                Subject.RemoveScript<DefaultBehaviorsScript>();
         
                Subject.Say("You will see my true power.");
            }

            if (Subject.WithinRange(PortalPoint, 1))
            {
                HitFirstHp = false;
                PortalOpened = false;
                Subject.MapInstance.RemoveEntity(Subject);
            }
            else
            { 
                Subject.Pathfind(PortalPoint);
            }
        }

        if (Subject.MapInstance.Name == "Terra Guardian's Domain" && Subject.StatSheet.CurrentHp <= 1131240)
        {
            HitSecondHp = true;
        }
        
        if (HitSecondHp)
        {
            Subject.StatSheet.SetHp(1131240);
            
            if (!WalkTimer.IntervalElapsed)
                return;

            if (!PortalOpened)
            {
                PortalOpened = true;
                var rectangle = new Rectangle(Subject.X, Subject.Y, 7, 7);
                rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var portalpoint1);
                if (portalpoint1 != null) PortalPoint = portalpoint1.Value;
                var portal = ReactorTileFactory.Create("SummonerEscapePortal", Subject.MapInstance, PortalPoint);
                Subject.MapInstance.SimpleAdd(portal);
                Subject.Say("Destroy them all. I must go now."); 
                var point2 = rectangle.GetRandomPoint();
                var point3 = rectangle.GetRandomPoint();
                var monster = MonsterFactory.Create("terra_guardian", Subject.MapInstance, point2);
                var monster2 = MonsterFactory.Create("terra_guardian", Subject.MapInstance, point3);
                Subject.MapInstance.AddEntity(monster, point2);
                Subject.MapInstance.AddEntity(monster2, point3);
                Subject.RemoveScript<MoveToTargetScript>();
                Subject.RemoveScript<AggroTargetingScript>();
                Subject.RemoveScript<AttackingScript>();
                Subject.RemoveScript<DefaultBehaviorsScript>();
         
            }

            if (Subject.WithinRange(PortalPoint, 1))
            {
                HitSecondHp = false;
                PortalOpened = false;
                Subject.MapInstance.RemoveEntity(Subject);
            }
            else
            { 
                Subject.Pathfind(PortalPoint); 
            }
        }

        if (Subject.MapInstance.Name == "Gale Guardian's Domain" && (Subject.StatSheet.CurrentHp <= 754160))
        {
            HitThirdHp = true;
        }
        
        if (HitThirdHp)
        {
            Subject.StatSheet.SetHp(754160);
            
            if (!WalkTimer.IntervalElapsed)
                return;
            
            if (!PortalOpened)
            {
                PortalOpened = true;
                var rectangle = new Rectangle(Subject.X, Subject.Y, 7, 7);
                rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var portalpoint1);
                if (portalpoint1 != null) PortalPoint = portalpoint1.Value;
                var portal = ReactorTileFactory.Create("SummonerEscapePortal", Subject.MapInstance, PortalPoint);
                Subject.MapInstance.SimpleAdd(portal);
                Subject.Say("Destroy them all. I must go now.");
                var point2 = rectangle.GetRandomPoint();
                var point3 = rectangle.GetRandomPoint();
                var monster = MonsterFactory.Create("gale_guardian", Subject.MapInstance, point2);
                var monster2 = MonsterFactory.Create("gale_guardian", Subject.MapInstance, point3);
                Subject.MapInstance.AddEntity(monster, point2);
                Subject.MapInstance.AddEntity(monster2, point3);
                Subject.RemoveScript<MoveToTargetScript>();
                Subject.RemoveScript<AggroTargetingScript>();
                Subject.RemoveScript<AttackingScript>();
                Subject.RemoveScript<DefaultBehaviorsScript>();
         
            }

            if (Subject.WithinRange(PortalPoint, 1))
            {
                HitThirdHp = false;
                PortalOpened = false;
                Subject.MapInstance.RemoveEntity(Subject);
            }
            else
            { 
                Subject.Pathfind(PortalPoint); 
            }
            
        }

        if (Subject.MapInstance.Name == "Tide Guardian's Domain" && (Subject.StatSheet.CurrentHp <= 377080))
        {
            HitFourthHp = true;
        }
        if (HitFourthHp)
        {
            Subject.StatSheet.SetHp(377080);
            
            if (!WalkTimer.IntervalElapsed)
                return;

            if (!PortalOpened)
            {
                PortalOpened = true;
                var rectangle = new Rectangle(Subject.X, Subject.Y, 7, 7);
                rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var portalpoint1);
                if (portalpoint1 != null) PortalPoint = portalpoint1.Value;
                var portal = ReactorTileFactory.Create("SummonerEscapePortal", Subject.MapInstance, PortalPoint);
                Subject.MapInstance.SimpleAdd(portal);
                Subject.Say("Destroy them all. I must go now.");
                var point2 = rectangle.GetRandomPoint();
                var point3 = rectangle.GetRandomPoint();
                var monster = MonsterFactory.Create("tide_guardian", Subject.MapInstance, point2);
                var monster2 = MonsterFactory.Create("tide_guardian", Subject.MapInstance, point3);
                Subject.MapInstance.AddEntity(monster, point2);
                Subject.MapInstance.AddEntity(monster2, point3);
                Subject.RemoveScript<MoveToTargetScript>();
                Subject.RemoveScript<AggroTargetingScript>();
                Subject.RemoveScript<AttackingScript>();
                Subject.RemoveScript<DefaultBehaviorsScript>();
         
            }

            if (Subject.WithinRange(PortalPoint, 1))
            {
                HitFourthHp = false;
                PortalOpened = false;
                Subject.MapInstance.RemoveEntity(Subject);
            }
            else
            { 
                Subject.Pathfind(PortalPoint); 
            }
            
        }

        if (Subject.MapInstance.Name == "Ignis Guardian's Domain" && Subject.StatSheet.CurrentHp <= 94270)
        {
            HitFifthHp = true;
        }
        
        if (HitFifthHp)
        {
            Subject.StatSheet.SetHp(94270);
            
            if (!WalkTimer.IntervalElapsed)
                return;

            if (!PortalOpened)
            {
                PortalOpened = true;
                var rectangle = new Rectangle(Subject.X, Subject.Y, 7, 7);
                rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var portalpoint1);
                if (portalpoint1 != null) PortalPoint = portalpoint1.Value;
                var portal = ReactorTileFactory.Create("SummonerEscapePortal", Subject.MapInstance, PortalPoint);
                Subject.MapInstance.SimpleAdd(portal);
                Subject.Say("Destroy them all. I must go now.");
                var point2 = rectangle.GetRandomPoint();
                var point3 = rectangle.GetRandomPoint();
                var monster = MonsterFactory.Create("ignis_guardian", Subject.MapInstance, point2);
                var monster2 = MonsterFactory.Create("ignis_guardian", Subject.MapInstance, point3);
                Subject.MapInstance.AddEntity(monster, point2);
                Subject.MapInstance.AddEntity(monster2, point3);
                Subject.RemoveScript<MoveToTargetScript>();
                Subject.RemoveScript<AggroTargetingScript>();
                Subject.RemoveScript<AttackingScript>();
                Subject.RemoveScript<DefaultBehaviorsScript>();
         
            }

            if (Subject.WithinRange(PortalPoint, 1))
            {
                HitFifthHp = false;
                PortalOpened = false;
                Subject.MapInstance.RemoveEntity(Subject);
            }
            else
            { 
                Subject.Pathfind(PortalPoint); 
            }
            
        }
    }
}