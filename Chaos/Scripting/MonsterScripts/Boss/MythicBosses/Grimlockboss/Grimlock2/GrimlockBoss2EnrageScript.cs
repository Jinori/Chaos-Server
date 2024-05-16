using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.Grimlockboss.Grimlock2;

public sealed class GrimlockBoss2EnrageScript : MonsterScriptBase
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
    public GrimlockBoss2EnrageScript(Monster subject, IMonsterFactory monsterFactory)
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

                var mobs = MonsterFactory.Create("grimlock2-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 20,
                Ac = -15,
                Str = 10
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

                var mobs = MonsterFactory.Create("grimlock2-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 35,
                SkillDamagePct = 15
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

                var mobs = MonsterFactory.Create("grimlock2-3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
            
            var attrib = new Attributes
            {
                AtkSpeedPct = 60,
                Dmg = 15,
                Ac = -20,
                Str = 15
            };
            Subject.StatSheet.AddBonus(attrib);
        }
    }
}