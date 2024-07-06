using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.GargoyleBoss.Gargoyle3;

public sealed class Gargoyle3BossRegenScript : MonsterScriptBase
{
    private static float HPRegenInterval = 12f;
    private static float HPMultiplier = 0.01f;
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
    public Gargoyle3BossRegenScript(Monster subject)
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
            HPRegenInterval = 8f;
            HPMultiplier = 0.01f;
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            HPRegenInterval = 7f;
            HPMultiplier = 0.02f;
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            HPRegenInterval = 6f;
            HPMultiplier = 0.02f;

            Subject.Animate(UpgradeAnimation);
        }
    }
}