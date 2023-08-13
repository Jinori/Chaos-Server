using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.HorseBoss.Horse3;

public sealed class HorseBoss3EnrageScript : MonsterScriptBase
{
    private static float HPRegenInterval = 8f;
    private static float HPMultiplier = 0.03f;
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private float HPRegenTimer;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public HorseBoss3EnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject) =>
        MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (Subject.StatSheet.HealthPercent < 100)
        {
            HPRegenTimer += (float)delta.TotalSeconds;

            if (HPRegenTimer >= HPRegenInterval)
            {
                var hpToRegen = Subject.StatSheet.MaximumHp * HPMultiplier;

                var newHP = (int)MathF.Min(Subject.StatSheet.CurrentHp + hpToRegen, Subject.StatSheet.MaximumHp);

                Subject.StatSheet.SetHp(newHP);
                Subject.ShowHealth();

                HPRegenTimer = 0f;

                Subject.Animate(UpgradeAnimation);
                Subject.ShowHealth();
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            //Give Bonuses
            var attrib = new Attributes { AtkSpeedPct = 40 };
            Subject.StatSheet.AddBonus(attrib);
            HPRegenInterval = 6f;
            HPMultiplier = 0.08f;
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
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            HPRegenInterval = 4f;
            HPMultiplier = 0.10f;

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Int = 10,
                Str = 15,
                Ac = -25,
                AtkSpeedPct = 20,
                Hit = 30,
                MagicResistance = 20,
                SkillDamagePct = 20,
                SpellDamagePct = 20
            };

            HPRegenInterval = 3f;
            HPMultiplier = 0.15f;
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}