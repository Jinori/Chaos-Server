using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.ServantBoss;

public sealed class ServantBossRegenScript : MonsterScriptBase
{
    private static float _hpRegenInterval = 9f;
    private static float _hpMultiplier = 0.02f;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private float HpRegenTimer;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 520
    };

    /// <inheritdoc />
    public ServantBossRegenScript(Monster subject)
        : base(subject) { }

    public override void Update(TimeSpan delta)
    {
        if (Subject.StatSheet.HealthPercent < 100)
        {
            HpRegenTimer += (float)delta.TotalSeconds;

            if (HpRegenTimer >= _hpRegenInterval)
            {
                var hpToRegen = Subject.StatSheet.MaximumHp * _hpMultiplier;

                var newHp = (int)MathF.Min(Subject.StatSheet.CurrentHp + hpToRegen, Subject.StatSheet.MaximumHp);

                if (Subject.Effects.Contains("poison"))
                    return;

                Subject.Animate(UpgradeAnimation);
                Subject.StatSheet.SetHp(newHp);

                HpRegenTimer = 0f;
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            _hpRegenInterval = 6f;
            _hpMultiplier = 0.005f;
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            _hpRegenInterval = 5f;
            _hpMultiplier = 0.01f;
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            _hpRegenInterval = 4f;
            _hpMultiplier = 0.02f;
        }
    }
}