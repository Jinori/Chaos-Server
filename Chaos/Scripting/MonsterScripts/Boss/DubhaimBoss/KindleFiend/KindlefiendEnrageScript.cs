using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.DubhaimBoss.KindleFiend;

public sealed class KindlefiendEnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer SpellCastTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast1;
    private readonly Spell SpellToCast2;
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

    {
        MonsterFactory = monsterFactory;
        SpellFactory = spellFactory;
        SpellToCast = spellFactory.Create("diacradhlamh");
        SpellToCast1 = spellFactory.Create("pramhlamh");
        SpellToCast2 = spellFactory.Create("dalllamh");

        SpellCastTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(20),
            20,
            RandomizationType.Balanced,
            false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);

        if (SpellCastTimer.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(100);

            switch (roll)
            {
                case < 15:
                    Subject.Say("What are your intentions Aisling?");
                    Subject.TryUseSpell(SpellToCast);

                    break;
                case < 75:
                    Subject.Say("Sleep forever Aisling!");
                    Subject.TryUseSpell(SpellToCast1);

                    break;
                case < 101:
                    Subject.Say("You'll never see again!");
                    Subject.TryUseSpell(SpellToCast2);

                    break;
            }
        }

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