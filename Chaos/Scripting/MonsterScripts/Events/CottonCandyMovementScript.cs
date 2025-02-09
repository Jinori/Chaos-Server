using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events;

public class CottonCandyMovementScript : MonsterScriptBase
{
    /// <inheritdoc />
    public CottonCandyMovementScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (Subject.WanderTimer.IntervalElapsed)
        {
            var nearestAislingDistance = Map.GetEntitiesWithinRange<Aisling>(Subject, 9)
                                            .ThatAreObservedBy(Subject)
                                            .ThatAreVisibleTo(Subject)
                                            .ThatAreNotInGodMode()
                                            .ThatAreAlive()
                                            .Select(aisling => (int?)Subject.ManhattanDistanceFrom(aisling))
                                            .Min();
            
            var chanceToMove = nearestAislingDistance switch
            {
                0 => 10,
                1 => 20,
                2 => 30,
                3 => 40,
                4 => 50,
                5 => 60,
                6 => 70,
                7 => 80,
                8 => 90,
                _ => 100
            };
            
            if(IntegerRandomizer.RollChance(chanceToMove))
                Subject.Wander();
        }
    }
}