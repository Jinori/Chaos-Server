using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.ZombieBoss.Zombie3;

public sealed class ZombieBoss3EnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public ZombieBoss3EnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            Subject.Animate(UpgradeAnimation);
            //Spawn Monsters
            var rectange = new Rectangle(Subject, 4, 4);

            for (var i = 0; i <= 4; i++)
            {
                var point = rectange.GetRandomPoint();
                var mobs = MonsterFactory.Create("zombie3-4", Subject.MapInstance, point);
                Subject.MapInstance.AddObject(mobs, point);
            }
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;
            Subject.Animate(UpgradeAnimation);
            var rectange = new Rectangle(Subject, 4, 4);

            for (var i = 0; i <= 6; i++)
            {
                var point = rectange.GetRandomPoint();
                var mobs = MonsterFactory.Create("zombie3-4", Subject.MapInstance, point);
                Subject.MapInstance.AddObject(mobs, point);
            }
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;
            Subject.Animate(UpgradeAnimation);
            Subject.Animate(UpgradeAnimation);
            var rectange = new Rectangle(Subject, 4, 4);

            for (var i = 0; i <= 8; i++)
            {
                var point = rectange.GetRandomPoint();
                var mobs = MonsterFactory.Create("zombie3-4", Subject.MapInstance, point);
                Subject.MapInstance.AddObject(mobs, point);
            }
        }
    }
}