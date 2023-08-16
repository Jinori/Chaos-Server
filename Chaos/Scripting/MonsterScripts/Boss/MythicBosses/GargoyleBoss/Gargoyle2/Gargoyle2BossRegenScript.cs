using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.GargoyleBoss.Gargoyle2;

public sealed class Gargoyle2BossRegenScript : MonsterScriptBase
{
    private static float HPRegenInterval = 8f;
    private static float HPMultiplier = 0.02f;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private float HPRegenTimer;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 267
    };

    /// <inheritdoc />
    public Gargoyle2BossRegenScript(Monster subject)
        : base(subject) { }

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
            HPRegenInterval = 6f;
            HPMultiplier = 0.03f;
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            HPRegenInterval = 4f;
            HPMultiplier = 0.04f;
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            HPRegenInterval = 3f;
            HPMultiplier = 0.05f;

            Subject.Animate(UpgradeAnimation);
        }
    }
}