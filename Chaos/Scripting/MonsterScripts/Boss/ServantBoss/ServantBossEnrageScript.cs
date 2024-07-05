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

namespace Chaos.Scripting.MonsterScripts.Boss.ServantBoss;

public sealed class ServantBossEnrageScript : MonsterScriptBase
{
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private bool Attack1;
    private bool Attack2;
    private bool Attack3;
    private bool Attack4;
    private readonly IIntervalTimer SpellCastTimer;
    private readonly IIntervalTimer WaitTimer;
    private readonly IIntervalTimer ActionTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast1;
    private readonly Spell SpellToCast2;
    private readonly Spell SpellToCast3;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public ServantBossEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        var spellFactory1 = spellFactory;
        SpellToCast = spellFactory1.Create("MorrocSpell1");
        SpellToCast1 = spellFactory1.Create("Havoc");
        SpellToCast2 = spellFactory1.Create("ardcradh");
        SpellToCast3 = spellFactory1.Create("MorrocSpell2");
        ActionTimer = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);
        SpellCastTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(8), 10, RandomizationType.Balanced, false);
        WaitTimer = new IntervalTimer(TimeSpan.FromSeconds(4), false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);
        ActionTimer.Update(delta);

        if (ActionTimer.IntervalElapsed)
        {

            if (Attack1)
            {
                Attack1 = false;
                Subject.TryUseSpell(SpellToCast);
            }

            if (Attack2)
            {
                Attack2 = false;
                Subject.TryUseSpell(SpellToCast3);
            }

            if (Attack3)
            {
                Attack3 = false;

                foreach (var target in Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 10))
                {
                    if (target.IsDead)
                        return;

                    if (target.IsGodModeEnabled())
                        return;

                    Subject.TryUseSpell(SpellToCast2, target.Id);
                }
            }

            if (Attack4)
            {
                WaitTimer.Update(delta);
                
                var ani = new Animation
                {
                    AnimationSpeed = 300,
                    TargetAnimation = 576
                };

                Subject.Animate(ani);
                if (WaitTimer.IntervalElapsed)
                {
                    Attack4 = false;
                    Subject.TryUseSpell(SpellToCast1);
                    WaitTimer.Reset();
                }
            }
            ActionTimer.Reset();
        }

        if (SpellCastTimer.IntervalElapsed)
        {
            if (!Attack1 && !Attack2 && !Attack3 && !Attack4)
            {
                var roll = IntegerRandomizer.RollSingle(100);

                switch (roll)
                {
                    case < 20:
                        Subject.Say("Master is going to Cthonic Remains!");
                        Attack1 = true;

                        break;
                    case < 45:
                        Subject.Say("He will rule the world with the creants!");
                        Attack2 = true;

                        break;
                    case < 70:
                        Subject.Say("Nothing will stop master from summoning them!");
                        Attack3 = true;

                        break;
                    case < 101:
                        Attack4 = true;
                        Subject.Say("You all will suffer!");
                        break;
                }
            }
            SpellCastTimer.Reset();
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