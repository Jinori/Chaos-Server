using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.UndineFields;

public sealed class EUFOrcRegenScript : MonsterScriptBase
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
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public EUFOrcRegenScript(Monster subject)
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

                if (Subject.Effects.Contains("poison") || Subject.Effects.Contains("miasma"))
                    return;

                Subject.StatSheet.SetHp(newHP);

                HPRegenTimer = 0f;
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;
            HPRegenInterval = 12f;
            HPMultiplier = 0.01f;
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            HPRegenInterval = 8f;
            HPMultiplier = 0.02f;
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            HPRegenInterval = 6f;
            HPMultiplier = 0.03f;
        }
    }
}