using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.PentagramBoss;

public sealed class PentagramBossEnrageScript : MonsterScriptBase
{
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
    public PentagramBossEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        SpellToCast = SpellFactory.Create("darkcone");
        SpellToCast1 = SpellFactory.Create("shadowflare");
        SpellToCast2 = SpellFactory.Create("morcradh");

        SpellCastTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(10),
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
                case < 40:
                    Subject.Say("You've made a mistake coming here!");
                    Subject.TryUseSpell(SpellToCast);

                    break;
                case < 75:
                    Subject.Say("Demonic Despair!");
                    Subject.TryUseSpell(SpellToCast1);

                    break;
                case < 101:
                    Subject.Say("You're not going to win this fight!");

                    foreach (var target in Subject.MapInstance
                                                  .GetEntitiesWithinRange<Aisling>(Subject, 10)
                                                  .ThatAreObservedBy(Subject)
                                                  .ThatAreVisibleTo(Subject))
                    {
                        if (target.IsDead)
                            continue;

                        Subject.TryUseSpell(SpellToCast2, target.Id);
                        Subject.TryUseSpell(SpellToCast, target.Id);
                    }

                    break;
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                AtkSpeedPct = 25
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            //Spawn Monsters
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 10,
                MagicResistance = 10,
                SkillDamagePct = 5,
                SpellDamagePct = 5
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
                Str = 10,
                Ac = -20,
                AtkSpeedPct = 20,
                Dmg = 15,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 20,
                MagicResistance = 10,
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}