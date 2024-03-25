using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet
{
    public sealed class PetAggroTargetingScript(Monster subject) : MonsterScriptBase(subject)
    {
        private readonly IIntervalTimer TargetUpdateTimer = new IntervalTimer(
            TimeSpan.FromMilliseconds(Math.Max(250, subject.Template.SkillIntervalMs)));
        private int InitialAggro = 10;

        private Monster? FindAggroedMonster(Aisling owner) =>
            owner.MapInstance.GetEntitiesWithinRange<Monster>(owner)
                 .FirstOrDefault(x => x.IsAlive && x.AggroList.ContainsKey(owner.Id));

        private Monster? FindClosestMonster(Aisling owner) =>
            owner.MapInstance.GetEntitiesWithinRange<Monster>(owner, 12)
                 .Where(x => x.IsAlive && !x.Equals(Subject))
                 .MinBy(x => x.DistanceFrom(owner));

        private Monster? FindGroupAggroTarget(Aisling owner)
        {
            if (owner.Group == null)
                return null;

            var groupMembers = owner.Group.Select(x => x.Id).ToList();
            
            return owner.MapInstance.GetEntitiesWithinRange<Monster>(owner, 12)
                       .Where(monster => monster.IsAlive && monster.AggroList.Keys.Any(aggroId => groupMembers.Contains(aggroId)))
                       .MaxBy(monster => monster.AggroList.Where(kvp => groupMembers.Contains(kvp.Key)).Sum(kvp => kvp.Value));
        }

        private bool IsCurrentTargetValid() => Target is { IsAlive: true } && Target.OnSameMapAs(Subject);

        public override void OnAttacked(Creature source, int damage, int? aggroOverride)
        {
            if (source.Equals(Subject) || source is Aisling)
                return;

            var aggro = aggroOverride ?? damage;
            if (aggro > 0)
            {
                AggroList.AddOrUpdate(source.Id, _ => aggro, (_, currentAggro) => currentAggro + aggro);
            }
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


            if (!TargetUpdateTimer.IntervalElapsed || (Subject.PetOwner == null))
                return;

            Subject.PetOwner.Trackers.Enums.TryGetValue(out PetMode value);

            if (value == PetMode.Passive)
                return;
            
            Target = value switch
            {
                PetMode.Offensive => FindClosestMonster(Subject.PetOwner),
                PetMode.Defensive => FindAggroedMonster(Subject.PetOwner) ?? FindClosestMonster(Subject.PetOwner),
                PetMode.Assist    => FindGroupAggroTarget(Subject.PetOwner),
                _                 => Target
            };

            if (Target != null)
                AggroList[Target.Id] = InitialAggro++; 
        }
    }
}
