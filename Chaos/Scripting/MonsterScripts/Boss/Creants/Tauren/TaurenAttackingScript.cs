using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

// ReSharper disable once ClassCanBeSealed.Global
public class TaurenAttackingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TaurenAttackingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (Subject.MapInstance
                   .GetEntities<Aisling>()
                   .Any(x => x.Trackers.Enums.HasValue(CreantPhases.InPhase)))
            return;

        if (Target is not { IsAlive: true } || (Subject.ManhattanDistanceFrom(Target) != 1))
            return;

        var direction = Target.DirectionalRelationTo(Subject);

        if (Subject.Direction != direction)
            return;

        var lastWalk = Subject.Trackers.LastWalk ?? DateTime.MinValue;

        if (DateTime.UtcNow.Subtract(lastWalk)
                    .TotalMilliseconds
            < Subject.EffectiveAssailIntervalMs)
            return;

        var attacked = false;

        foreach (var skill in Skills)
            if (skill.Template.IsAssail)
                attacked |= Subject.TryUseSkill(skill);

        if (ShouldUseSkill)
        {
            var skill = Skills.Where(skill => Subject.CanUse(skill, out _))
                              .PickRandomWeightedSingleOrDefault(7);

            if (skill is not null)
                attacked |= Subject.TryUseSkill(skill);
        }

        if (attacked)
        {
            Subject.WanderTimer.Reset();
            Subject.MoveTimer.Reset();
        }
    }
}