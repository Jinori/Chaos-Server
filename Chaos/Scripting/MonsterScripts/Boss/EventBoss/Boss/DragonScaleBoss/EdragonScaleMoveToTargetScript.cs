using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.DragonScaleBoss;

public class EdragonScaleMoveToTargetScript : MonsterScriptBase
{
    
    private bool ReachedPoint;
    private readonly IIntervalTimer WalkTimer;
    private readonly IEffectFactory EffectFactory;
    private readonly Spell SpellToCast;

    /// <inheritdoc />
    public EdragonScaleMoveToTargetScript(Monster subject, ISpellFactory spellFactory, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        SpellToCast = spellFactory.Create("fireblast");
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);

    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        var fightSpot = Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(Subject, 12)
            .FirstOrDefault(x => x.Name == "Lion Fish");

        if (fightSpot == null && !ReachedPoint)
        {
            if (!Subject.Effects.Contains("invulnerability"))
            {
                var invulnerability = EffectFactory.Create("invulnerability");
                Subject.Effects.Apply(Subject, invulnerability);
            }
        }

        if (fightSpot != null)
        {
            var point = new Point(fightSpot.X, fightSpot.Y);
        
            if (Subject.ManhattanDistanceFrom(point) > 0 && !ReachedPoint)
            {
                WalkTimer.Update(delta);
                Subject.StatSheet.SetHealthPct(100);
            
                if (WalkTimer.IntervalElapsed)
                    Subject.Pathfind(fightSpot);

                if (Subject.ManhattanDistanceFrom(fightSpot) <= 1)
                {
                    var lionFish = Subject.MapInstance.GetEntitiesAtPoints<GroundItem>(fightSpot).FirstOrDefault(x => x.Name == "Lion Fish");

                    if (lionFish != null)
                    {
                        Subject.Say("DEATH TO ALL WHO DISTURB MY MEAL.");
                        Subject.TryUseSpell(SpellToCast);
                        Map.RemoveEntity(lionFish);
                        Subject.Effects.Terminate("Invulnerability");
                        ReachedPoint = true;   
                    }
                }
                return;
            }
        }

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;
        
        if (!ReachedPoint)
            return;
        
        var distance = Subject.ManhattanDistanceFrom(Target);

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