using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

// ReSharper disable once ClassCanBeSealed.Global
public class TeammateAttackingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TeammateAttackingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //if target is invalid or we're not close enough
        //reset attack delay and return
        if (Subject.PetOwner?.ManhattanDistanceFrom(Subject) >= 8)
        {
            Target = null;

            return;
        }

        if (Target is not { IsAlive: true } || (Subject.ManhattanDistanceFrom(Target) != 1))
            return;

        var direction = Target.DirectionalRelationTo(Subject);

        if (Subject.Direction != direction)
            return;

        if (DateTime.UtcNow.Subtract(Subject.Trackers.LastWalk ?? DateTime.MinValue).TotalMilliseconds < Subject.EffectiveAssailIntervalMs)
            return;

        var attacked = false;

        foreach (var assail in Skills.Where(skill => skill.Template.IsAssail))
            attacked |= Subject.TryUseSkill(assail);

        if (ShouldUseSkill)
            foreach (var skill in Skills.Where(skill => !skill.Template.IsAssail))
                if (IntegerRandomizer.RollChance(25) && Subject.TryUseSkill(skill))
                {
                    attacked = true;

                    break;
                }

        if (attacked)
            Subject.MoveTimer.Reset();
    }
}