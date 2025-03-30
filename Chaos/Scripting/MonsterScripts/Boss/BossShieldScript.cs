using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossShieldScript : MonsterScriptBase
{
    private const float SHIELD_DURATION = 10f; // Shield lasts for 10 seconds
    private const float RECHARGE_DURATION = 30f; // Shield can be recharged every 30 seconds
    private const float SHIELD_ABSORPTION_RATE = 0.5f; // Shield absorbs 50% of incoming damage
    private bool IsShieldActive;
    private IIntervalTimer RechargeTimer { get; }
    private IIntervalTimer ShieldTimer { get; }

    public BossShieldScript(Monster subject)
        : base(subject)
    {
        ShieldTimer = new IntervalTimer(TimeSpan.FromSeconds(SHIELD_DURATION));
        RechargeTimer = new IntervalTimer(TimeSpan.FromSeconds(RECHARGE_DURATION));
    }

    private void ActivateShield()
    {
        IsShieldActive = true;
        ShieldTimer.Reset();
        Subject.Say("You can't harm me now!");

        // Additional visual or sound effects for shield activation
    }

    private void DeactivateShield()
    {
        IsShieldActive = false;
        RechargeTimer.Reset();
        Subject.Say("My shield is down!");

        // Additional visual or sound effects for shield deactivation
    }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage)
    {
        if (IsShieldActive)
            damage = (int)(damage * (1 - SHIELD_ABSORPTION_RATE));
        base.OnAttacked(source, damage);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        RechargeTimer.Update(delta);

        if (IsShieldActive)
        {
            ShieldTimer.Update(delta);

            if (ShieldTimer.IntervalElapsed)
                DeactivateShield();
        } else if (RechargeTimer.IntervalElapsed)
            ActivateShield();
    }
}