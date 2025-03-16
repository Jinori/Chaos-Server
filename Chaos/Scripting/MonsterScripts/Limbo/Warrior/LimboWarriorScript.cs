using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Warrior;

public class LimboWarriorScript : MonsterScriptBase
{
    private readonly Skill Charge;
    private readonly Skill TempestBlade;
    private readonly Skill Shockwave;
    private readonly ISkillFactory SkillFactory;
    private readonly IIntervalTimer ActionTimer;
    
    
    public LimboWarriorScript(Monster subject, ISkillFactory skillFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;
        Charge = SkillFactory.Create("charge");
        TempestBlade = SkillFactory.Create("tempestBlade");
        Shockwave = SkillFactory.Create("shockwave");
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 25, startAsElapsed: false);
    }

    public override void Update(TimeSpan delta)
    {
        Charge.Update(delta);
        TempestBlade.Update(delta);
        Shockwave.Update(delta);
        
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;
        
        var target = Subject.Target;

        if (target is null)
            return;

        //if target within 5 spaces
        //and on the same x or y axis
        //and we're facing it
        if (Subject.WithinRange(target, 5)
            && ((Subject.X == target.X) || (Subject.Y == target.Y))
            && (target.DirectionalRelationTo(Subject) == Subject.Direction))
        {
            if (Subject.TryUseSkill(Charge))
                return;
        }

        if (Subject.WithinRange(target, 2) && Subject.CanUse(TempestBlade, out _))
        {
            var targetDirection = target.DirectionalRelationTo(Subject);
            var optimalDirection = GetOptimalDirection(AoeShape.Front, 3, targetDirection);
            
            if(Subject.Direction != optimalDirection)
                Subject.Turn(optimalDirection);

            if (Subject.TryUseSkill(TempestBlade))
                return;
        }

        if (Subject.WithinRange(target, 2) && Subject.CanUse(Shockwave, out _))
        {
            var targetDirection = target.DirectionalRelationTo(Subject);
            var optimalDirection = GetOptimalDirection(AoeShape.FrontalCone, 4, targetDirection);
            
            if(Subject.Direction != optimalDirection)
                Subject.Turn(optimalDirection);

            Subject.TryUseSkill(Shockwave);
        }
    }

    private Direction GetOptimalDirection(AoeShape aoeShape, int range, Direction initialDirection = Direction.All)
    {
        //indexes are directions
        var numTargetsByDirection = new int[4];
        
        //consider target direction first
        foreach (var direction in initialDirection.AsEnumerable())
        {
            var options = new AoeShapeOptions
            {
                Source = new Point(Subject.X, Subject.Y),
                Range = range,
                Direction = direction
            };

            var points = AoeShape.FrontalCone.ResolvePoints(options);

            var numTargets = Subject.MapInstance
                                    .GetEntitiesAtPoints<Aisling>(points)
                                    .Count();
                
            numTargetsByDirection[(int)direction] = numTargets;
        }

        var maxTargets = numTargetsByDirection.Max();
        return (Direction)Array.IndexOf(numTargetsByDirection, maxTargets);
    }
}