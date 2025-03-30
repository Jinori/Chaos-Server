using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.LynithPirateShip.LynithBoss;

public sealed class LynithBossEnrageScript(Monster subject) : MonsterScriptBase(subject)
{
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                AtkSpeedPct = 15
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
                Int = 10,
                Str = 10,
                Ac = -30,
                FlatSkillDamage = 100,
                FlatSpellDamage = 100,
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