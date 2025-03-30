using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

public class TaurenMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TaurenMoveToTargetScript(Monster subject)
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

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var script = Subject.Script.As<TaurenPhaseScript>()!;

        if (script.InPhase)
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        switch (distance)
        {
            case > 1:
                Subject.Pathfind(Target);

                break;
            case 1:
            {
                var direction = Target.DirectionalRelationTo(Subject);
                Subject.Turn(direction);

                break;
            }
            case 0:
            {
                Subject.Wander();

                break;
            }
        }

        ResetAttackTimerIfMoved();
    }
}