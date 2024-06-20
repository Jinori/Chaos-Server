using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.ServantBoss;

public sealed class ServantBossEnrageScript : MonsterScriptBase
{
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private readonly IIntervalTimer SpellCastTimer;
    private readonly IIntervalTimer SkillUseTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast1;
    private readonly Spell SpellToCast2;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public ServantBossEnrageScript(Monster subject, ISpellFactory spellFactory, ISkillFactory skillFactory)
        : base(subject)
    {
        var spellFactory1 = spellFactory;
        SpellToCast = spellFactory1.Create("MorrocSpell1");
        SpellToCast1 = spellFactory1.Create("Havoc");
        SpellToCast2 = spellFactory1.Create("ardcradh");
        SkillUseTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);
        SpellCastTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(10), 20, RandomizationType.Balanced, false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);
        SkillUseTimer.Update(delta);

        if (SpellCastTimer.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(100);
            
            switch (roll)
            {
                case < 40:
                    Subject.Say("Feel my power!");
                    Subject.TryUseSpell(SpellToCast);

                    break;
                case < 75:
                    Subject.Say("Die you fool!");
                    Subject.TryUseSpell(SpellToCast1);

                    break;
                case < 101:
                    Subject.Say("Your end is near!");

                    foreach (var target in Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 10))
                    {
                        target.TryUseSpell(SpellToCast2);
                        target.TryUseSpell(SpellToCast);
                    }

                    break;
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            //Give Bonuses
            var attrib = new Attributes { AtkSpeedPct = 25 };
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