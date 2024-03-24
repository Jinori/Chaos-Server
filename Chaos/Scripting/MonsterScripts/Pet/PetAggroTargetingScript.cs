using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public sealed class PetAggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;
    public PetMode CurrentMode { get; set; } = PetMode.Defensive;


    /// <inheritdoc />
    public PetAggroTargetingScript(Monster subject)
        : base(subject) =>
        TargetUpdateTimer =
            new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(250, Subject.Template.SkillIntervalMs)));

    private Monster? FindAggroedMonster(Aisling? owner)
    {
        if (owner == null)
            return null;

        return Map.GetEntitiesWithinRange<Monster>(owner)
                  .FirstOrDefault(x => x.IsAlive && x.AggroList.ContainsKey(owner.Id));
    }

    private Monster? FindClosestMonster(Aisling? owner)
    {
        if (owner == null)
            return null;

        // Get all alive monsters within the specified range
        var monstersInRange = Map.GetEntitiesWithinRange<Monster>(owner, 12)
                                 .Where(x => x.IsAlive && !x.Equals(Subject));

        // Find the closest monster by using the DistanceFrom extension method
        return monstersInRange.MinBy(x => x.DistanceFrom(owner));
    }


    private bool IsCurrentTargetValid() => Target is { IsAlive: true } && Target.OnSameMapAs(Subject);

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        if (source.Equals(Subject) || source is Aisling)
            return;

        var aggro = aggroOverride ?? damage;

        if (aggro == 0)
            return;

        AggroList.AddOrUpdate(source.Id, _ => aggro, (_, currentAggro) => currentAggro + aggro);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        TargetUpdateTimer.Update(delta);

        if (!IsCurrentTargetValid())
        {
            if (Target != null)
                AggroList.Remove(Target.Id, out _);

            Target = null;
        }

        if (!TargetUpdateTimer.IntervalElapsed)
            return;
        
        var value = PetMode.Defensive;
        Subject.PetOwner?.Trackers.Enums.TryGetValue(out value);

        Target = value switch
        {
            PetMode.Offensive => FindClosestMonster(Subject.PetOwner) ?? Target,
            PetMode.Defensive => FindAggroedMonster(Subject.PetOwner) ?? FindClosestMonster(Subject.PetOwner) ?? Target,
            _                 => Target
        };

        if (Target != null)
            AggroList[Target.Id] = InitialAggro++; 
    }
}