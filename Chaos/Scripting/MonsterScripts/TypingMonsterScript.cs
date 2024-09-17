using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TypingMonsterScript : MonsterScriptBase
{
    private readonly IIntervalTimer RandomWalkInterval;

    /// <inheritdoc />
    public TypingMonsterScript(Monster subject)
        : base(subject) =>
        RandomWalkInterval = new RandomizedIntervalTimer(TimeSpan.FromSeconds(1), 70, RandomizationType.Negative);

    private readonly Animation TypingDeathAnimation = new()
    {
        AnimationSpeed = 100,
        SourceAnimation = 187,
        Priority = 99
    };
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        RandomWalkInterval.Update(delta);

        if (!RandomWalkInterval.IntervalElapsed) 
            return;
        
        Subject.Walk(Direction.Right);
        Subject.Chant(Subject.TypingWord);
    }

    public override void OnDeath()
    {
        Subject.MapInstance.ShowAnimation(TypingDeathAnimation.GetPointAnimation(Subject));
        Subject.MapInstance.RemoveEntity(Subject);
    }
}