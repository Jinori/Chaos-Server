using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
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
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IApplyHealScript ApplyHealScript;

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

    private readonly IIntervalTimer WalkInterval;

    private bool HasSetSpawnDirection;
    private Direction SpawnDirection;
    private readonly IPoint SpawnPoint;
    private int StepCounter;

    /// <inheritdoc />
    public ShadowtouchScript(Monster subject)
        : base(subject)
    {
        WalkInterval = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyHealScript = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();
        ApplyHealScript.HealFormula = HealFormulae.Default;
        SpawnPoint = new Point(Subject.X, Subject.Y);
    }

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

            // Determine the next point based on the current direction
            var nextPoint = Subject.DirectionalOffset(Subject.Direction);

            // Check if the next point is walkable
            if (!Subject.MapInstance.IsWalkable(nextPoint, collisionType: Subject.Type))

                // Reverse direction if facing a wall
                Subject.Direction = Subject.Direction.Reverse();

            // Ensure the monster does not move further from the spawn point during return
            if ((StepCounter >= 4) && (StepCounter <= 7))
            {
                var nextDistanceFromSpawn = nextPoint.ManhattanDistanceFrom(SpawnPoint);
                var currentDistanceFromSpawn = Subject.ManhattanDistanceFrom(SpawnPoint);

                // If the next move takes it further from the spawn point, reverse direction
                if (nextDistanceFromSpawn > currentDistanceFromSpawn)
                    Subject.Direction = Subject.Direction.Reverse();
            }

            // Movement logic
            switch (StepCounter)
            {
                case < 4: // First 4 steps forward
                    Subject.Walk(Subject.Direction);
                    StepCounter++;

                    break;

                case >= 4 and <= 7: // Next 3 steps back
                    if (Subject.ManhattanDistanceFrom(SpawnPoint) == 0)

                        // Stop moving if the monster has returned to the spawn point
                        Map.RemoveEntity(Subject);
                    else
                    {
                        Subject.Walk(Subject.Direction);
                        StepCounter++;
                    }

                    break;

                case >= 7: // Remove the monster from the map after 7 steps
                    Map.RemoveEntity(Subject);

                    break;
            }

            // Combat logic
            if (Subject.PetOwner is not null)
            {
                var monster = Subject.MapInstance
                                     .GetEntitiesAtPoints<Creature>(Subject)
                                     .FirstOrDefault(x => Subject.PetOwner.IsHostileTo(x) && !x.Equals(Subject));

                if (monster is not null)
                {
                    // Deal damage to the hostile monster
                    monster.Animate(ShadowAnimation);
                    var damage = (int)(Subject.PetOwner.StatSheet.EffectiveInt * Subject.PetOwner.StatSheet.EffectiveWis * 0.35);

                    ApplyDamageScript.ApplyDamage(
                        Subject.PetOwner,
                        monster,
                        this,
                        damage,
                        Element.Darkness);

                    // Heal the pet owner or their group
                    if (Subject.PetOwner.Group is { Count: > 0 })
                        foreach (var player in Subject.PetOwner.Group)
                        {
                            if (!player.WithinRange(Subject))
                                continue;

                            player.Animate(HealAnimation);

                            ApplyHealScript.ApplyHeal(
                                Subject,
                                player,
                                this,
                                damage / Subject.PetOwner.Group.Count);
                        }
                    else
                    {
                        Subject.PetOwner.Animate(HealAnimation);

                        ApplyHealScript.ApplyHeal(
                            Subject,
                            Subject.PetOwner,
                            this,
                            damage / 2);
                    }
                }
            }
        }
    }
}