using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.HorseBoss.Horse1;

public sealed class HorseBoss1EnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private static float HPRegenInterval = 8f;
    private float HPRegenTimer = 0f;
    private static float HPMultiplier = 0.03f;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public HorseBoss1EnrageScript(Monster subject, IMonsterFactory monsterFactory)
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
            var attrib = new Attributes { AtkSpeedPct = 10 };
            Subject.StatSheet.AddBonus(attrib);
            HPRegenInterval = 8f;
            HPMultiplier = 0.04f;
            Subject.Animate(UpgradeAnimation);
            //Spawn Monsters
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 5,
                MagicResistance = 10,
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };
            
            HPRegenInterval = 6f;
            HPMultiplier = 0.06f;

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Int = 3,
                Str = 5,
                Ac = -10,
                AtkSpeedPct = 10,
                Hit = 10,
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };

            HPRegenInterval = 4f;
            HPMultiplier = 0.08f;
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}