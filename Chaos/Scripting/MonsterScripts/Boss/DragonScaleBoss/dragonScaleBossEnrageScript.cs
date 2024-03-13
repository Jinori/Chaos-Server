using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.DragonScaleBoss;

public sealed class dragonScaleBossEnrageScript : MonsterScriptBase
{
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private readonly IIntervalTimer SpellCastTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast1;
    private readonly Spell SpellToCast2;
    
    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public dragonScaleBossEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        SpellToCast = SpellFactory.Create("firecone");
        SpellToCast1 = SpellFactory.Create("fireflare");
        SpellCastTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(10), 20, RandomizationType.Balanced, false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);
        
        if (SpellCastTimer.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(100);
            
            switch (roll)
            {
                case < 50:
                {
                    Subject.Say("Dissssturb meee? I jussst wanted a meal!");
                    Subject.TryUseSpell(SpellToCast);
                    break; 
                }

                case < 101:
                {
                    Subject.Say("ROAAARRRRR");
                    Subject.TryUseSpell(SpellToCast1);

                    break;
                }
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            //Give Bonuses
            var attrib = new Attributes { AtkSpeedPct = 15 };
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
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Int = 5,
                Str = 10,
                Ac = -10,
                AtkSpeedPct = 20,
                Dmg = 15,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 10,
                MagicResistance = 10,
                SkillDamagePct = 20,
                SpellDamagePct = 20,
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}