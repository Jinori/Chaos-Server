using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TypingMonsterScript : MonsterScriptBase
{
    private IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();
    private readonly IIntervalTimer RandomWalkInterval;

    /// <inheritdoc />
    public TypingMonsterScript(Monster subject)
        : base(subject)
    {
        RandomWalkInterval = new RandomizedIntervalTimer(TimeSpan.FromSeconds(1), 70, RandomizationType.Negative);
    }

    private readonly Animation TypingDeathAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 218,
        Priority = 80,
    };
    
    public override void OnPublicMessage(Creature source, string message)
    {
        base.OnPublicMessage(source, message);

        if (source.MapInstance.InstanceId == "arena_typing" && source is Aisling)
        {
            // Iterate over creatures within the specified range (13) and check if the typed message matches their TypingWord
            foreach (var creature in Subject.MapInstance
                         .GetEntitiesWithinRange<Monster>(source, 13)
                         .Where(x => x.TypingWord.EqualsI(message)))
            {
                var point = new Point(creature.X, creature.Y);
                creature.MapInstance.ShowAnimation(TypingDeathAnimation.GetPointAnimation(point));
                ApplyDamageScript.ApplyDamage(Subject, creature, this, 999999);
            }   
        }
    }
    
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
        Subject.MapInstance.RemoveEntity(Subject);
    }
}