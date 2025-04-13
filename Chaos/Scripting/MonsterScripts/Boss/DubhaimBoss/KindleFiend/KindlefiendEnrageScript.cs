using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.DubhaimBoss.KindleFiend;

public sealed class KindlefiendEnrageScript : MonsterScriptBase
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
    public KindlefiendEnrageScript(Monster subject, ISpellFactory spellFactory, IMonsterFactory monsterFactory)
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
                AtkSpeedPct = 25,
                Ac = -5
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 2; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("dc_bloodgargoyle1", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Ac = -10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            if (Subject.MapInstance
                       .GetEntities<Monster>()
                       .Where(x => x.Template.TemplateKey == "dc_bloodgargoyle1")
                       .ToList()
                       .Count
                > 0)
                foreach (var gargoyle in Subject.MapInstance
                                                .GetEntities<Monster>()
                                                .Where(x => x.Template.TemplateKey == "dc_bloodgargoyle1"))
                    Subject.StatSheet.AddHealthPct(5);

            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 3; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("dc_bloodgargoyle2", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Dmg = 15,
                Hit = 20,
                MagicResistance = 20
            };

            if (Subject.MapInstance
                       .GetEntities<Monster>()
                       .Where(x => x.Template.TemplateKey == "dc_bloodgargoyle2")
                       .ToList()
                       .Count
                > 0)
                foreach (var gargoyle in Subject.MapInstance
                                                .GetEntities<Monster>()
                                                .Where(x => x.Template.TemplateKey == "dc_bloodgargoyle1"))
                    Subject.StatSheet.AddHealthPct(5);

            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 4; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("dc_bloodgargoyle3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}