using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CryptBosses.CryptReaper;

public sealed class CryptReaperEnrageScript : MonsterScriptBase
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
    public CryptReaperEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            //Give Bonuses
            var attrib = new Attributes { SpellDamagePct = 30};
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 4; i++)
            {
                var point = rectangle.GetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type) && !Subject.MapInstance.IsWall(x));
                var mobs = MonsterFactory.Create("crypt_succubus15", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 30,
                MagicResistance = 50,
                SkillDamagePct = 20,
                SpellDamagePct = 20
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;
            
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 4; i++)
            {
                var point = rectangle.GetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type) && !Subject.MapInstance.IsWall(x));
                var mobs = MonsterFactory.Create("crypt_succubus15", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            var attrib = new Attributes
            {
                Str = 20,
                Hit = 40,
                MagicResistance = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}