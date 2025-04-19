using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.EffectScripts.Monk;

public class SiphoningEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(8);

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
        if (AislingSource != null)
            AislingSubject?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"You feel a life-siphoning energy connecting you to {AislingSource.Name}.");
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => SiphonHealth();

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The life-siphoning connection fades away.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Effects.Contains("Siphoning"))
        {
            AislingSource?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The target is already linked by the Siphoning Effect.");

            return false;
        }

        return true;
    }

    private void SiphonHealth()
    {
        if (!Source.IsAlive)
        {
            Subject.Effects.Dispel("siphoning");

            return;
        }

        var damage = DamageHelper.CalculatePercentDamage(
            Source,
            Subject,
            0.01m,
            true);
        
        ApplyDamageScript.ApplyDamage(
            Source,
            Subject,
            ApplyDamageScript,
            damage);

        var maxSiphon = damage / 2.0m;
        var pctHeal = MathEx.GetPercentOf<int>((int)Source.StatSheet.EffectiveMaximumHp, 2.5m);
        var pctMana = MathEx.GetPercentOf<int>((int)Source.StatSheet.EffectiveMaximumMp, 2.5m);
        var finalHeal = (int)Math.Min(maxSiphon, pctHeal);
        var finalMana = (int)Math.Min(maxSiphon, pctMana);
        
        AislingSubject?.SendOrangeBarMessage($"You are being siphoned by {Source.Name}");
        
        Subject.Animate(CreatureAnimation);
        
        // Heal the Source
        if ((Source.StatSheet.HealthPercent != 100) || (Source.StatSheet.ManaPercent != 100))
        {
            
            Source.StatSheet.AddHp(finalHeal);
            Source.StatSheet.AddMp(finalMana);
            Source.ShowHealth();
            
            AislingSource?.Client.SendAttributes(StatUpdateType.Vitality);
            AislingSource?.SendOrangeBarMessage("You siphon vitality from your target");
        }
    }
}