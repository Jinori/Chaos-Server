using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.MythicBosses.WolfBoss.Wolf2;

public sealed class EWolfBoss2EnrageScript : MonsterScriptBase
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
    public EWolfBoss2EnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            Subject.Animate(UpgradeAnimation);
            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 2; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("wolf2-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 15,
                Ac = -10
            };
            Subject.StatSheet.AddBonus(attrib);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;
            Subject.Animate(UpgradeAnimation);
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 3; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("wolf2-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes { AtkSpeedPct = 25 };
            Subject.StatSheet.AddBonus(attrib);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;
            Subject.Animate(UpgradeAnimation);
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 4; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("wolf2-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 15,
                Dmg = 20,
                Ac = -45
            };
            Subject.StatSheet.AddBonus(attrib);
        }
    }
}