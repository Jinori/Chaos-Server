using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.MythicBosses.KoboldBoss;

public sealed class EKoboldBossRegenScript : MonsterScriptBase
{
    private const float HP_REGEN_INTERVAL = 6f;
    private const float HP_MULTIPLIER = 0.02f;
    private float HPRegenTimer;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 4
    };

    /// <inheritdoc />
    public EKoboldBossRegenScript(Monster subject)
        : base(subject) { }

    public override void Update(TimeSpan delta)
    {
        if (Subject.StatSheet.HealthPercent < 100)
        {
            HPRegenTimer += (float)delta.TotalSeconds;

            if (HPRegenTimer >= HP_REGEN_INTERVAL)
            {
                var hpToRegen = Subject.StatSheet.MaximumHp * HP_MULTIPLIER;

                var newHP = (int)MathF.Min(Subject.StatSheet.CurrentHp + hpToRegen, Subject.StatSheet.MaximumHp);

                Subject.StatSheet.SetHp(newHP);
                Subject.ShowHealth();

                HPRegenTimer = 0f;
                Subject.Animate(UpgradeAnimation);
                Subject.ShowHealth();
            }
        }
    }
}