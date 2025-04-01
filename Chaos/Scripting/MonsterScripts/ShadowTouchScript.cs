using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class ShadowTouchScript : MonsterScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IApplyHealScript ApplyHealScript;
    public IScript? SourceScript { get; set; }

    private readonly Animation HealAnimation = new()
    {
        TargetAnimation = 4,
        AnimationSpeed = 100
    };

    private readonly Animation ShadowAnimation = new()
    {
        TargetAnimation = 76,
        AnimationSpeed = 100
    };

    private readonly IPoint SpawnPoint;

    private readonly IIntervalTimer WalkInterval;
    private int StepCounter;

    /// <inheritdoc />
    public ShadowTouchScript(Monster subject)
        : base(subject)
    {
        StepCounter = -1;
        WalkInterval = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyHealScript = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();
        SpawnPoint = Point.From(subject);
    }

    private void DoDamageAndHeal()
    {
        var targets = Subject.MapInstance
                              .GetEntitiesAtPoints<Creature>(Subject)
                              .WithFilter(Subject.PetOwner!, TargetFilter.AliveOnly | TargetFilter.HostileOnly)
                              .ToList();
        
        if(targets.Count == 0)
            return;
        
        var manaDmg = MathEx.GetPercentOf<int>((int)Subject.PetOwner!.StatSheet.EffectiveMaximumMp, 0.015m);
        var damage = 750;
        damage += Subject.PetOwner.StatSheet.EffectiveInt * 5 + (int)(Subject.PetOwner.StatSheet.EffectiveWis * 2.5m);
        var finalDmg = manaDmg + damage;

        foreach (var target in targets)
        {
            target.Animate(ShadowAnimation);

            ApplyDamageScript.ApplyDamage(
                Subject.PetOwner,
                target,
                SourceScript!,
                finalDmg,
                Element.Darkness);

            DoHeal(finalDmg);
        }
    }

    private void DoHeal(int damageAmount)
    {
        var nearbyGroupMembers = Map.GetEntitiesWithinRange<Aisling>(Subject)
                                    .WithFilter(Subject.PetOwner!, TargetFilter.AliveOnly | TargetFilter.GroupOnly)
                                    .ToList();
        
        if(nearbyGroupMembers.Count == 0)
            return;

        var healAmount = damageAmount / 2;

        foreach (var member in nearbyGroupMembers)
        {
            member.Animate(HealAnimation);

            ApplyHealScript.ApplyHeal(
                Subject.PetOwner!,
                member,
                SourceScript!,
                healAmount);
        }
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //initial dmg
        if(StepCounter is -1)
        {
            StepCounter = 0;
            DoDamageAndHeal();
        }
        
        WalkInterval.Update(delta);

        if (!WalkInterval.IntervalElapsed)
            return;
        
        var pointInFront = Subject.DirectionalOffset(Subject.Direction);

        //if we encounter a wall or blocking reactor
        //skip to step 4 which turns the pet around
        if (Map.IsWall(pointInFront) || Map.IsBlockingReactor(pointInFront))
            StepCounter = 4;

        //on step 4, do damage and reverse direction
        if (StepCounter is 4)
        {
            DoDamageAndHeal();
            
            var reverseDirection = Subject.Direction.Reverse();
            Subject.Turn(reverseDirection);
        }

        Subject.Walk(Subject.Direction, false);
        StepCounter++;
        DoDamageAndHeal();
        
        if (SpawnPoint.Equals(Subject))
            Map.RemoveEntity(Subject);
    }
}