using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.UndineFields;

public sealed class CarnunBossRoomHazardScript : MonsterScriptBase
{
    private Rectangle? SafeRectangle;
    private readonly Rectangle ArenaRectangle;
    private Animation HazardAnimation { get; }
    private Animation SafeAnimation { get; }
    private IApplyDamageScript ApplyDamageScript { get; }
    private IIntervalTimer HazardExpirationTimer { get; }
    private IIntervalTimer SafeTimer { get; }
    private IIntervalTimer HazardTimer { get; }
    private ISequentialTimer SequenceTimer { get; }
    private IIntervalTimer SafeAnimationTimer { get; }
    private IIntervalTimer HazardAnimationTimer { get; }
    private bool GetToSafety => SequenceTimer.CurrentTimer == SafeTimer;

    private bool ApplyHazard => SequenceTimer.CurrentTimer == HazardExpirationTimer;

    private bool NoHazard => SequenceTimer.CurrentTimer == HazardTimer;

    public CarnunBossRoomHazardScript(Monster subject)
        : base(subject)
    {
        HazardTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(45),
            10,
            RandomizationType.Positive,
            false);

        HazardAnimation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 13,
            Priority = 1
        };
        SafeAnimation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 214,
            Priority = 1
        };
        ArenaRectangle = new Rectangle(new Point(18, 18), 15, 15);
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
        HazardExpirationTimer = new IntervalTimer(TimeSpan.FromSeconds(7), false);
        HazardAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        SafeTimer = new IntervalTimer(TimeSpan.FromSeconds(12), false);
        SafeAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        SequenceTimer = new SequentialEventTimer(HazardTimer, SafeTimer, HazardExpirationTimer);
    }

    private bool ContainsWallOrReactor(IRectangle rectangle) =>
        rectangle.GetPoints().Any(point => Map.IsWall(point) || Map.IsBlockingReactor(point));
    
    private void GenerateSafeRectangle()
    {
        const int MAX_ATTEMPTS = 100;
        var attempts = 0;
        const int WIDTH = 3; // Set the width of the rectangle
        const int HEIGHT = 3; // Set the height of the rectangle
        
        do
        {
            var randomSafeCenterPoint = ArenaRectangle.GetRandomPoint();
            
            SafeRectangle = new Rectangle(randomSafeCenterPoint, WIDTH, HEIGHT);
            attempts++;

            if (attempts >= MAX_ATTEMPTS)
                break;
        } while (ContainsWallOrReactor(SafeRectangle));
    }
    
    public override void Update(TimeSpan delta)
    {
        if (!Map.GetEntities<Aisling>().Any())
            return;
        
        SequenceTimer.Update(delta);
        
        if (ApplyHazard)
        {
            HazardAnimationTimer.Update(delta);
            
            if (!HazardAnimationTimer.IntervalElapsed)
                return;

            var hazardPoints = ArenaRectangle.GetPoints().Except(SafeRectangle!.GetPoints())
                .ToListCast<IPoint>();
            
            foreach (var point in hazardPoints) 
                Map.ShowAnimation(HazardAnimation.GetPointAnimation(point));

            var creaturesToDamage = Map.GetEntitiesAtPoints<Creature>(hazardPoints)
                .Where(c => c.Id != Subject.Id);

            foreach (var creature in creaturesToDamage)
            {
                var damage = Convert.ToInt32(creature.StatSheet.EffectiveMaximumHp * 0.3m);

                if (creature.IsDead)
                    return;
                
                ApplyDamageScript.ApplyDamage(Subject, creature, this, damage);
            }
        }
        else if (GetToSafety)
        {
            SafeAnimationTimer.Update(delta);
            
            if (!SafeAnimationTimer.IntervalElapsed) 
                return;
            
            foreach (var point in SafeRectangle!.GetPoints())
                Map.ShowAnimation(SafeAnimation.GetPointAnimation(point));
        }
        else if (NoHazard && HazardTimer.IntervalElapsed) 
        {
            Subject.Say("Better run Aisling, it's about to get hot.");

            GenerateSafeRectangle();
        }
    }
}