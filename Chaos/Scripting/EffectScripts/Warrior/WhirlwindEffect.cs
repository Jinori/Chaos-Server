using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class WhirlwindEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromDays(1);
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 185
    };
    protected IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);
    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();
    /// <inheritdoc />
    public override byte Icon => 98;
    protected IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
    /// <inheritdoc />
    public override string Name => "Whirlwind";
    
    protected void OnIntervalElapsed()
    {
        if (Subject.StatSheet.HealthPercent < 33)
        {
            Subject.Effects.Terminate(Name);

            return;
        }

        // 3% of health per second
        // but as we gain attack speed, this will tick faster
        // so we need to adjust the health subtraction based on attack speed to maintain 3% per second
        var healthPercentToSubtract = 3.0m;
        var effectiveAtkSpeedPct = 1.0m + (decimal)(Subject.StatSheet.EffectiveAttackSpeedPct / 100.0);
        
        healthPercentToSubtract /= effectiveAtkSpeedPct;
        
        Subject.StatSheet.SubtractHealthPct(healthPercentToSubtract);
        AislingSubject?.ShowHealth();
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);

        var options = new AoeShapeOptions
        {
            Source = new Point(Subject.X, Subject.Y),
            Range = 1
        };

        var points = AoeShape.AllAround.ResolvePoints(options);

        var targets = Subject.MapInstance
                             .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                             .WithFilter(Subject, TargetFilter.HostileOnly)
                             .ToList();

        var damage = 15 + Subject.StatSheet.EffectiveStr * 2;

        foreach (var target in targets)
        {
            ApplyDamageScript.ApplyDamage(
                Subject,
                target,
                this,
                damage);

            target.ShowHealth();
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "Your whirlwind calms down.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.StatSheet.HealthPercent <= 33)
            return false;

        return true;
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //scale animation and effect with attack speed
        var effectiveAttackSpeedPct = Subject.StatSheet.EffectiveAttackSpeedPct;
        var modifier = 1.0 + effectiveAttackSpeedPct / 100.0;
        var modifiedDelta = delta.Multiply(modifier);
        
        AnimationInterval.Update(modifiedDelta);
        Interval.Update(modifiedDelta);

        if (AnimationInterval.IntervalElapsed)
        {
            Animation.AnimationSpeed = (ushort)(100 / modifier);
            Subject.Animate(Animation);
        }
        
        if(Interval.IntervalElapsed)
            OnIntervalElapsed();

        base.Update(delta);
    }
}