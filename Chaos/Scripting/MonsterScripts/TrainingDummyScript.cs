using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TrainingDummyScript(Monster subject) : MonsterScriptBase(subject)
{
    public readonly IIntervalTimer DamageTimer = new IntervalTimer(TimeSpan.FromSeconds(1));

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        DamageTimer.Update(delta);

        if (!DamageTimer.IntervalElapsed)
            return;

        var damageTaken = (int)(Subject.StatSheet.EffectiveMaximumHp - Subject.StatSheet.CurrentHp);

        if (damageTaken > 0)
            Subject.Say($"{damageTaken:N0} damage.");

        Subject.StatSheet.SetHealthPct(100);
    }
}