using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Chaos.ZarathTheForsaken;

public sealed class ZarathTheForsakenEnrageScript : MonsterScriptBase
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
    public ZarathTheForsakenEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject)
        => MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                MagicResistance = 10
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs1 = MonsterFactory.Create("chaos_Deathknight", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs1, point);
            }
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                AtkSpeedPct = 25
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Int = 10,
                Ac = 10,
                AtkSpeedPct = 20,
                Dmg = 5,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 20,
                MagicResistance = 10
            };

            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs1 = MonsterFactory.Create("chaos_Deathknight", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs1, point);
            }

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}