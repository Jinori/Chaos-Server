using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.WestWoodlands.FireDraco;

public sealed class EFireDracoBossEnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus10Applied;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public EFireDracoBossEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            var attrib = new Attributes
            {
                AtkSpeedPct = 25,
                Hit = 5,
                Dmg = 5
            };

            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 3; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("ELW_Wisp2", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;
            var rectangle = new Rectangle(Subject, 5, 5);
            Subject.StatSheet.SetHealthPct(45);
            var attrib = new Attributes
            {
                AtkSpeedPct = 10,
                MagicResistance = 10,   
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };

            for (var i = 0; i <= 4; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("ELW_Faerie2", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.Animate(UpgradeAnimation);
        }
        
        if (!Bonus10Applied && (Subject.StatSheet.HealthPercent <= 10))
        {
            Bonus10Applied = true;
            var rectangle = new Rectangle(Subject, 5, 5);
            Subject.StatSheet.SetHealthPct(25);
            var attrib = new Attributes
            {
                AtkSpeedPct = 10,
                MagicResistance = 5,   
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            Subject.Animate(UpgradeAnimation);
        }
    }
}