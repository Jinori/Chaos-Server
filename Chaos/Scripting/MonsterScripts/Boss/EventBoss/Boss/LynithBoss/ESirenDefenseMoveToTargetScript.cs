using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.LynithBoss;

public class ESirenDefenseMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ESirenDefenseMoveToTargetScript(Monster subject)
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

        var distance = Subject.ManhattanDistanceFrom(Target);

        if (Subject.Template.TemplateKey.Contains("siren"))
        {
            if (distance <= 5)
            {
                var pathtopoint = Subject.SpiralSearch(3)
                                         .OrderByDescending(point => point.ManhattanDistanceFrom(Target))
                                         .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, Subject.Type));

                Subject.Pathfind(pathtopoint);
            }
        } else
        {
            if (distance != 1)
                Subject.Pathfind(Target);
            else
            {
                var direction = Target.DirectionalRelationTo(Subject);
                Subject.Turn(direction);
            }
        }

        ResetAttackTimerIfMoved();
    }
}