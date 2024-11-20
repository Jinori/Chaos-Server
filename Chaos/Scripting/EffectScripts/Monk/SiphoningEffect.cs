using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class SiphoningEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);

    private Aisling? SourceOfEffect { get; set; }
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 162
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(2), false);
    protected IApplyDamageScript ApplyDamageScript { get; }

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 162
    };
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(2000), false);

    /// <inheritdoc />
    public override byte Icon => 2;
    /// <inheritdoc />
    public override string Name => "Siphoning";

    public SiphoningEffect() => ApplyDamageScript = ApplyAttackDamageScript.Create();

    public override void OnApplied()
    {
        if (SourceOfEffect != null)
            AislingSubject?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You feel a life-siphoning energy connecting you to {SourceOfEffect.Name}.");
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        SiphonHealth();
    }

    private void SiphonHealth()
    {
            // Calculate heal amount based on the target's current health
            int healAmount = (int)(Subject.StatSheet.CurrentHp * 0.01); // Example: Heal 3% of the target's current health

            if (healAmount > 10000)
                healAmount = 10000;

            // Ensure the heal amount does not exceed the target's current health
            if (healAmount < Subject.StatSheet.CurrentHp)
            {
                // Drain health from the target
                if (SourceOfEffect != null)
                {
                    ApplyDamageScript.ApplyDamage(SourceOfEffect, Subject, ApplyDamageScript, healAmount);
                    AislingSubject?.SendOrangeBarMessage($"You are being siphoned by {SourceOfEffect?.Name}");
                }

                AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
                Subject.Animate(CreatureAnimation);
                Subject.ShowHealth();
            }

            if (SourceOfEffect is { IsDead: true })
            {
                Subject.Effects.Dispel("siphoning");
                return;
            }
            
            
            // Heal the SourceOfEffect
            if (SourceOfEffect != null && SourceOfEffect.StatSheet.CurrentHp != SourceOfEffect.StatSheet.EffectiveMaximumHp)
            {
                SourceOfEffect.StatSheet.AddHp(healAmount);
                SourceOfEffect.ShowHealth();
                SourceOfEffect.Client.SendAttributes(StatUpdateType.Vitality);
                SourceOfEffect.SendOrangeBarMessage($"You heal for {healAmount} from your life-siphon.");
            }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "The life-siphoning connection fades away.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
            SourceOfEffect = source as Aisling;

            if (target.Effects.Contains("Siphoning"))
            {
                SourceOfEffect?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The target is already linked by the Siphoning Effect.");
                return false;
            }

            return true;
    }
}
