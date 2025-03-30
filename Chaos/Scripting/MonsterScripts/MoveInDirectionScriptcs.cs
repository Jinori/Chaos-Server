using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class MoveInDirectionScript : MonsterScriptBase
{
    private readonly IIntervalTimer RandomWalkInterval;

    /// <inheritdoc />
    public MoveInDirectionScript(Monster subject)
        : base(subject)
        => RandomWalkInterval = new RandomizedIntervalTimer(TimeSpan.FromSeconds(1), 70, RandomizationType.Negative);

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        RandomWalkInterval.Update(delta);

        if (RandomWalkInterval.IntervalElapsed)
            Subject.Walk(Direction.Right);
    }
}