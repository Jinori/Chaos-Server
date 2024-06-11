using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.WestWoodlands.Twink;

public sealed class TwinkBossEnrageScript : MonsterScriptBase
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
    public TwinkBossEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            var attrib = new Attributes { AtkSpeedPct = 25 };
            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("WW_Faerie2", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 2; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("WW_Wisp4", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;
            var rectangle = new Rectangle(Subject, 5, 5);
            Subject.StatSheet.SetHealthPct(50);
            var attrib = new Attributes
            {
                AtkSpeedPct = 15,
                MagicResistance = 25,   
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            for (var i = 0; i <= 2; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("WW_Faerie2", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.Animate(UpgradeAnimation);
        }
    }
}