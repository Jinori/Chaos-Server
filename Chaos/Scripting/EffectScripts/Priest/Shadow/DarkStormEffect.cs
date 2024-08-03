using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;
using NLog.Targets;

namespace Chaos.Scripting.EffectScripts.Priest.Shadow;

public class DarkStormEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 76
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    private IApplyDamageScript ApplyDamageScript { get; } = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override byte Icon => 125;

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000));

    /// <inheritdoc />
    public override string Name => "DarkStorm";
    
    private Creature SourceOfEffect { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.StatSheet.CurrentMp < 20)
        {
            Subject.Effects.Terminate(Name);
            return;
        }

        Subject.StatSheet.SubtractMp(20);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);


        // Get all non-wall points on the map within 13 spaces of the subject
        var nonWallPoints = Enumerable.Range(0, Subject.MapInstance.Template.Width)
            .SelectMany(x => Enumerable.Range(0, Subject.MapInstance.Template.Height)
                .Where(y => !Subject.MapInstance.IsWall(new Point(x, y)) && 
                            Math.Sqrt(Math.Pow(x - Subject.X, 2) + Math.Pow(y - Subject.Y, 2)) <= 4)
                .Select(y => new Point(x, y))).ToList();

        if (nonWallPoints.Count <= 0)
            return;

        // Select a random non-wall point
        var targetPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

        Subject.MapInstance.ShowAnimation(Animation.GetPointAnimation(targetPoint));

        // Check if an entity is standing on the point and apply damage
        var target = Subject.MapInstance
            .GetEntitiesAtPoint<Creature>(targetPoint).FirstOrDefault(x => x is not Merchant && x.IsAlive && x.IsHostileTo(Subject));

        if (target == null) 
            return;
        
        var damage = (int)(target.StatSheet.EffectiveMaximumHp * 0.1); // 10% of max HP
        ApplyDamageScript.ApplyDamage(SourceOfEffect, target, this, damage);
        target.ShowHealth();
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(
        ServerMessageType.OrangeBar1,
        "The chaos subsides.");
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        SourceOfEffect = source;
        
        return true;
    }
}
