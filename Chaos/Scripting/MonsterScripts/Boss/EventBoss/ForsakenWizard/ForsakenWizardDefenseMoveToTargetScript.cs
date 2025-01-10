using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.ForsakenWizard;

public class ForsakenWizardDefenseMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ForsakenWizardDefenseMoveToTargetScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        
        if (Target == null || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);
        
        if (Subject.Template.TemplateKey.Contains("event_forsakenwizard"))
        {
            if (distance <= 3)
            {
                var pathtopoint = Subject.SpiralSearch(3).OrderByDescending(point => point.ManhattanDistanceFrom(Target))
                    .FirstOrDefault(point => Subject.MapInstance.IsWalkable(point, Subject.Type));
            
                Subject.Pathfind(pathtopoint);
            }
        }
        else
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