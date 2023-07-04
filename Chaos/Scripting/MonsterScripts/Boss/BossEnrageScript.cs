using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public class BossEnrageScript : MonsterScriptBase
{
    private bool Bonus30Applied;
    private bool Bonus50Applied;

    private bool Bonus75Applied;

    protected Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public BossEnrageScript(Monster subject)
        : base(subject) { }

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            var attrib = new Attributes { AtkSpeedPct = 25 };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 25,
                MagicResistance = 10,
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
                Con = 10,
                Dex = 10,
                Int = 10,
                Str = 10,
                Wis = 10,
                Ac = 15,
                AtkSpeedPct = 25,
                Dmg = 10,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 20,
                MagicResistance = 10,
                SkillDamagePct = 20,
                SpellDamagePct = 20
            };

            var end = Spells.FirstOrDefault(x => x.Template.TemplateKey.ToLower() == "end_all");

            if (end != null)
                Subject.TryUseSpell(end);

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}