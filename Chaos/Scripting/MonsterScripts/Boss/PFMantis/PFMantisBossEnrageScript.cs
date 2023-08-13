using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.PFMantis;

public sealed class PFMantisBossEnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus20Applied;
    private bool Bonus40Applied;
    private bool Bonus60Applied;
    private bool Bonus90Applied;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public PFMantisBossEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus90Applied && (Subject.StatSheet.HealthPercent <= 90))
        {
            Bonus90Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                AtkSpeedPct = 15,
                Str = 5,
                Int = 2,
                MagicResistance = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus60Applied && (Subject.StatSheet.HealthPercent <= 60))
        {
            Bonus60Applied = true;

            var attrib = new Attributes
            {
                Dmg = 6,
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus40Applied && (Subject.StatSheet.HealthPercent <= 40))
        {
            Bonus40Applied = true;

            var attrib = new Attributes
            {
                Con = 5,
                Dex = 5,
                Int = 5,
                Str = 5,
                Wis = 5,
                Ac = 10,
                AtkSpeedPct = 15,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus20Applied && (Subject.StatSheet.HealthPercent <= 20))
        {
            Bonus20Applied = true;

            var attrib = new Attributes
            {
                Ac = 5,
                AtkSpeedPct = 10,
                Dmg = 6,
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                Hit = 6,
                MagicResistance = 10,
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}