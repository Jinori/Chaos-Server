using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.DragonScaleBoss;

public class dragonScaleMoveToTargetScript : MonsterScriptBase
{
    
    private bool ReachedPoint;
    public readonly Location FightSpot = new("wilderness", 18, 16);
    private readonly IIntervalTimer TimeToGetToSpot;
    private readonly IIntervalTimer WalkTimer;
    private readonly Spell SpellToCast;

    /// <inheritdoc />
    public dragonScaleMoveToTargetScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellToCast = spellFactory.Create("fireblast");
        TimeToGetToSpot = new IntervalTimer(TimeSpan.FromSeconds(15), false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);

    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        if (Subject.DistanceFrom(FightSpot) > 0 && !ReachedPoint)
        {
            TimeToGetToSpot.Update(delta);
            WalkTimer.Update(delta);
            Subject.StatSheet.SetHealthPct(100);
            
            if (WalkTimer.IntervalElapsed)
                Subject.Pathfind(FightSpot);

            if (TimeToGetToSpot.IntervalElapsed) 
                Subject.MapInstance.RemoveEntity(Subject);

            if (Subject.DistanceFrom(FightSpot) <= 1)
            {
                var rockFish = Subject.MapInstance.GetEntitiesAtPoint<GroundItem>(FightSpot).FirstOrDefault(x => x.Name == "Lion Fish");

                if (rockFish != null)
                {
                    Subject.Say("DEATH TO ALL WHO DISTURB MY MEAL.");
                    Subject.TryUseSpell(SpellToCast);
                    Map.RemoveEntity(rockFish);
                    ReachedPoint = true;   
                }

                if (rockFish == null)
                {
                    Map.RemoveEntity(Subject);
                }
            }
            return;
        }
        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;
        
        
        var distance = Subject.DistanceFrom(Target);

        if (distance != 1)
            Subject.Pathfind(Target);
        else
        {
            var direction = Target.DirectionalRelationTo(Subject);
            Subject.Turn(direction);
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}