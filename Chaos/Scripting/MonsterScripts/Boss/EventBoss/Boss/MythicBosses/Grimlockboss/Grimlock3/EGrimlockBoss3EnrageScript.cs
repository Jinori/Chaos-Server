using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.MythicBosses.Grimlockboss.Grimlock3;

public sealed class EGrimlockBoss3EnrageScript : MonsterScriptBase
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
    public EGrimlockBoss3EnrageScript(Monster subject, IMonsterFactory monsterFactory)
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

                var mobs = MonsterFactory.Create("grimlock3-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 25,
                Ac = -20,
                Str = 15
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

                var mobs = MonsterFactory.Create("grimlock3-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 45,
                SkillDamagePct = 20
            };
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

                var mobs = MonsterFactory.Create("grimlock3-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 70,
                Dmg = 20,
                Ac = -25,
                Str = 20
            };
            Subject.StatSheet.AddBonus(attrib);
        }
    }
}