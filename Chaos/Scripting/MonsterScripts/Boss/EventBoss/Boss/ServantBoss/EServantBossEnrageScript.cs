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

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.ServantBoss;

public sealed class EServantBossEnrageScript : MonsterScriptBase
{
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private readonly IIntervalTimer SpellCastTimer;
    private readonly IIntervalTimer ActionTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast2;
    private readonly Spell SpellToCast3;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public EServantBossEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        var spellFactory1 = spellFactory;
        SpellToCast = spellFactory1.Create("MorrocSpell1");
        SpellToCast2 = spellFactory1.Create("ardcradh");
        SpellToCast3 = spellFactory1.Create("MorrocSpell2");
        ActionTimer = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);
        SpellCastTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(8), 10, RandomizationType.Balanced, false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);
        ActionTimer.Update(delta);

        if (SpellCastTimer.IntervalElapsed)
        {
                var roll = IntegerRandomizer.RollSingle(100);

                switch (roll)
                {
                    case < 30:
                        Subject.Say("Master is going to Cthonic Remains!");
                        Subject.TryUseSpell(SpellToCast);

                        break;
                    case < 60:
                        Subject.Say("He will rule the world with the creants!");
                        Subject.TryUseSpell(SpellToCast3);

                        break;
                    case < 101:
                        Subject.Say("Nothing will stop master from summoning them!");

                        foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>()
                                     .Where(x => x.WithinRange(Subject, 10) && !x.IsGodModeEnabled() && !x.IsDead))
                        {
                            Subject.TryUseSpell(SpellToCast2, aisling.Id);
                        }
                        break;
                }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            //Give Bonuses
            var attrib = new Attributes { AtkSpeedPct = 10 };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
            //Spawn Monsters
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 2,
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
                Ac = -10,
                AtkSpeedPct = 10,
                Dmg = 8,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 20,
                MagicResistance = 10,
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}