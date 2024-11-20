using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class ShadowtouchScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ShadowtouchScript(Monster subject)
        : base(subject)
    {
        WalkInterval = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyHealScript = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();
        ApplyHealScript.HealFormula = HealFormulae.Default;
    }

    private readonly IIntervalTimer WalkInterval;
    private int StepCounter;
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IApplyHealScript ApplyHealScript;
    private Direction SpawnDirection;
    private readonly Animation ShadowAnimation = new()
    {
        TargetAnimation = 76,
        AnimationSpeed = 100
    };
    private readonly Animation HealAnimation = new()
    {
        TargetAnimation = 4,
        AnimationSpeed = 100
    };
    private bool HasSetSpawnDirection;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        WalkInterval.Update(delta);
        
        if (WalkInterval.IntervalElapsed)
        {
            if (!HasSetSpawnDirection)
            {
                SpawnDirection = Subject.Direction;
                HasSetSpawnDirection = true;
            }
            
            switch (StepCounter)
            {
                case < 4:
                    Subject.Walk(Subject.Direction);
                    StepCounter++;

                    break;
                case >= 4 and <= 7:
                {
                    if (Subject.Direction == SpawnDirection)
                        Subject.Direction = SpawnDirection.Reverse();
                
                    Subject.Walk(Subject.Direction);
                    StepCounter++;

                    break;
                }
                case >= 7:
                    Map.RemoveEntity(Subject);

                    break;
            }

            if (Subject.PetOwner is not null)
            {
                var monster = Subject.MapInstance
                                     .GetEntitiesAtPoint<Creature>(Subject)
                                     .FirstOrDefault(x => Subject.PetOwner.IsHostileTo(x) && !x.Equals(Subject));

                if (monster is not null)
                {
                    monster.Animate(ShadowAnimation);
                    var damage = (int)(Subject.PetOwner.StatSheet.EffectiveInt * Subject.PetOwner.StatSheet.EffectiveWis * 0.35);
                    ApplyDamageScript.ApplyDamage(Subject.PetOwner, monster, this, damage, Element.Darkness);

                    if (Subject.PetOwner.Group is { Count: > 0 })
                    {
                        foreach (var player in Subject.PetOwner.Group)
                        {
                            player.Animate(HealAnimation);
                            ApplyHealScript.ApplyHeal(Subject, player, this,  damage / Subject.PetOwner.Group.Count);
                        }
                    }
                    else
                    {
                        Subject.PetOwner.Animate(HealAnimation);
                        ApplyHealScript.ApplyHeal(Subject, Subject.PetOwner, this, damage / 2);   
                    }
                }   
            }
        }
    }
}