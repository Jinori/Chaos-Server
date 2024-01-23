using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

public sealed class TeammateAggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public TeammateAggroTargetingScript(Monster subject)
        : base(subject) =>
        TargetUpdateTimer =
            new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(250, Subject.Template.SkillIntervalMs)));

    private Monster? FindAggroedMonster(Aisling? owner)
    {
        if (owner == null)
            return null;

        return Map.GetEntitiesWithinRange<Monster>(owner)
                  .FirstOrDefault(x => x.IsAlive && x.AggroList.ContainsKey(owner.Id) != x.Script.Is<NightmareTeammateScript>());
    }

    private Monster? FindClosestMonster() =>
        Map.GetEntitiesWithinRange<Monster>(Subject, AggroRange)
           .ThatAreObservedBy(Subject)
           .Where(
               obj => !obj.Equals(Subject)
                      && obj.IsAlive
                      && !obj.Script.Is<NightmareTeammateScript>()
                      && !obj.Name.Contains("Wind Wall")
                      && Subject.ApproachTime.TryGetValue(obj.Id, out var time)
                      && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
           .ClosestOrDefault(Subject);

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

        var owner = Subject.PetOwner;

        Target = FindAggroedMonster(owner) ?? FindClosestMonster() ?? Target;

        //since we grabbed a new target, give them some initial aggro so we stick to them
        if (Target != null)
            AggroList[Target.Id] = InitialAggro++;
    }
}