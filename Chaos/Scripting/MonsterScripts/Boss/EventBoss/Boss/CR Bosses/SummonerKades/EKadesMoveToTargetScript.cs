using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CR_Bosses.SummonerKades;

public class EKadesMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public EKadesMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;
        
        if (Subject.Effects.Contains("invulnerability"))
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
    
    private void ResetAttackTimerIfMoved()
    {
        var now = DateTime.UtcNow;
        var lastWalk = Subject.Trackers.LastWalk;
        var lastTurn = Subject.Trackers.LastTurn;
        var walkedRecently = lastWalk.HasValue && (now.Subtract(lastWalk.Value).TotalMilliseconds < Subject.Template.MoveIntervalMs);
        var turnedRecently = lastTurn.HasValue && (now.Subtract(lastTurn.Value).TotalMilliseconds < Subject.Template.MoveIntervalMs);

        if (walkedRecently || turnedRecently)
        {
            Subject.WanderTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }
}