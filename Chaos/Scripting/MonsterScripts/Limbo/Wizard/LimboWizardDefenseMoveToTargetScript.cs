using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Wizard;

public class LimboWizardDefenseMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public LimboWizardDefenseMoveToTargetScript(Monster subject)
        : base(subject) { }

    private void ResetAttackTimerIfMoved()
    {
        var now = DateTime.UtcNow;
        var lastWalk = Subject.Trackers.LastWalk;
        var lastTurn = Subject.Trackers.LastTurn;

        var walkedRecently = lastWalk.HasValue
                             && (now.Subtract(lastWalk.Value)
                                    .TotalMilliseconds
                                 < Subject.Template.MoveIntervalMs);

        var turnedRecently = lastTurn.HasValue
                             && (now.Subtract(lastTurn.Value)
                                    .TotalMilliseconds
                                 < Subject.Template.MoveIntervalMs);

        if (walkedRecently || turnedRecently)
        {
            Subject.WanderTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())

            return;

        if (Target == null)
        {
            if (ShouldWander)
                Subject.Wander();

            return;
        }

        var distance = Subject.ManhattanDistanceFrom(Target);

        if (distance <= 5)
        {
            var pathtopoint = Subject.SpiralSearch(3)
                                     .OrderByDescending(point => point.ManhattanDistanceFrom(Target))
                                     .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, collisionType: Subject.Type));

            Subject.Pathfind(pathtopoint);
        } else
            Subject.Pathfind(Target);

        ResetAttackTimerIfMoved();
    }
}