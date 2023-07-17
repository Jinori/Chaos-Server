using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.PFMantis;

public sealed class PFMantisBossEnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus40Applied;
    private bool Bonus60Applied;
    private bool Bonus90Applied;
    private bool Bonus20Applied;

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
                AtkSpeedPct = 45,
                Str = 20,
                Int = 20,
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
                Dmg = 35,
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus40Applied && (Subject.StatSheet.HealthPercent <= 40))
        {
            Bonus40Applied = true;

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

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
        
        if (!Bonus20Applied && (Subject.StatSheet.HealthPercent <= 20))
        {
            Bonus20Applied = true;

            var attrib = new Attributes
            {
                Ac = 25,
                AtkSpeedPct = 45,
                Dmg = 15,
                FlatSkillDamage = 15,
                FlatSpellDamage = 15,
                Hit = 25,
                MagicResistance = 10,
                SkillDamagePct = 20,
                SpellDamagePct = 20,
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
        
    }
}