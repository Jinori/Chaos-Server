using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.HorseBoss.Horse2;

public sealed class HorseBoss2EnrageScript : MonsterScriptBase
{
    private static float HPRegenInterval = 15f;
    private static float HPMultiplier = 0.01f;
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
    public HorseBoss2EnrageScript(Monster subject, IMonsterFactory monsterFactory)
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
            var attrib = new Attributes { AtkSpeedPct = 35 };
            Subject.StatSheet.AddBonus(attrib);
            HPRegenInterval = 9f;
            HPMultiplier = 0.01f;
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

            HPRegenInterval = 8f;
            HPMultiplier = 0.02f;

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
                Ac = -15,
                AtkSpeedPct = 15,
                Hit = 20,
                MagicResistance = 10,
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };

            HPRegenInterval = 7f;
            HPMultiplier = 0.3f;
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}