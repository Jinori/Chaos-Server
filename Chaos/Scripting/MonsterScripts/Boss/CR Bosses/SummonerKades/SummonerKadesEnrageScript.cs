using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CR_Bosses.SummonerKades;

public sealed class SummonerKadesEnrageScript : MonsterScriptBase
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
    public SummonerKadesEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            //Give Bonuses
            var attrib = new Attributes { AtkSpeedPct = 25 };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs1 = MonsterFactory.Create("Gale_guardian", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs1, point);
            }
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;
            
            var attrib = new Attributes
            {
                Dmg = 10,
                MagicResistance = 10,
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };
            
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
            
            var rectangle = new Rectangle(Subject, 5, 5);
            
            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs1 = MonsterFactory.Create("Ignis_guardian", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs1, point);
            }
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Str = 10,
                Ac = 15,
                AtkSpeedPct = 25,
                Dmg = 10,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 20,
                MagicResistance = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
            
            var rectangle = new Rectangle(Subject, 5, 5);
            
            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs1 = MonsterFactory.Create("terra_guardian", Subject.MapInstance, point);
                var mobs2 = MonsterFactory.Create("tide_guardian", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs1, point);
                Subject.MapInstance.AddEntity(mobs2, point);
            }
        }
    }
}