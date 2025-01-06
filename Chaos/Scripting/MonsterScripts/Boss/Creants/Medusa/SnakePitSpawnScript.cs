using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Medusa;

public sealed class SnakePitSpawnScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer SpawnTimer;

    private Animation SpawnAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 200
    };

    /// <inheritdoc />
    public SnakePitSpawnScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(25), false);
    }

    public override void Update(TimeSpan delta)
    {
        SpawnTimer.Update(delta);
        
        if (SpawnTimer.IntervalElapsed)
        {
            Subject.Animate(SpawnAnimation);
            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                return;

            var whitesnake = MonsterFactory.Create("OR_whitemedusasnake", Subject.MapInstance, point);
            Subject.MapInstance.AddEntity(whitesnake, point);
            var redsnake = MonsterFactory.Create("OR_redmedusasnake", Subject.MapInstance, point);
            Subject.MapInstance.AddEntity(redsnake, point);
            var brownsnake = MonsterFactory.Create("OR_brownmedusasnake", Subject.MapInstance, point);
            Subject.MapInstance.AddEntity(brownsnake, point);
        }
    }
}