using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public class WrathEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1);
    
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 95
    };

    protected IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);

    protected IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();
    
    protected IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 98;

    /// <inheritdoc />
    public override string Name => "Wrath";
    
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
            Source = Point.From(Subject),
            Range = 1,
            Direction = Direction.All
        };

        var points = AoeShape.AllAround.ResolvePoints(options);

        var targets = Subject.MapInstance
                             .GetEntitiesAtPoints<Creature>(points)
                             .WithFilter(Subject, TargetFilter.HostileOnly | TargetFilter.AliveOnly);

        var damage = (Subject.StatSheet.EffectiveStr + 10) * 2;

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

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You end your wrath and your skin turns to normal.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.StatSheet.HealthPercent <= 3)
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