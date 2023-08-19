using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class AirElementalChaosScript : MapScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript;
    private IIntervalTimer AirElementalChaosTimer { get; }
    private Animation LightningAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 15
    };
    
    /// <inheritdoc />
    public AirElementalChaosScript(MapInstance subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        AirElementalChaosTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        AirElementalChaosTimer.Update(delta);
        
        if (!AirElementalChaosTimer.IntervalElapsed)
            return;
        
        // Get all non-wall points on the map
        var nonWallPoints = Enumerable.Range(0, Subject.Template.Width)
                                      .SelectMany(x => Enumerable.Range(0, Subject.Template.Height)
                                                                 .Where(y => !Subject.IsWall(new Point(x, y)))
                                                                 .Select(y => new Point(x, y))).ToList();
        
        if (nonWallPoints.Count > 0)
        {
            // Select a random non-wall point
            var targetPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];
            
            Subject.ShowAnimation(LightningAnimation.GetPointAnimation(targetPoint));

            // Check if a player is standing on the point and apply damage
            var targetPlayer = Subject.GetEntitiesAtPoint<Aisling>(targetPoint).FirstOrDefault();
            if (targetPlayer != null)
            {
                var damage = (int)(targetPlayer.StatSheet.EffectiveMaximumHp * 0.1); // 10% of max HP
                ApplyDamageScript.ApplyDamage(targetPlayer, targetPlayer, this, damage);
            }
        }
    }
}