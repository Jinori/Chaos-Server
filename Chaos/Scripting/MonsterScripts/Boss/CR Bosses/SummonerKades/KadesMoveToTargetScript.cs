using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CR_Bosses.SummonerKades;

public class KadesMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public KadesMoveToTargetScript(Monster subject)
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

        if (Map.GetEntities<Aisling>()
               .Any(
                   x => x.Trackers.Enums.HasValue(SummonerBossFight.FirstStage)
                        || x.Trackers.Enums.HasValue(SummonerBossFight.SecondStage)
                        || x.Trackers.Enums.HasValue(SummonerBossFight.ThirdStage)
                        || x.Trackers.Enums.HasValue(SummonerBossFight.FourthStage)
                        || x.Trackers.Enums.HasValue(SummonerBossFight.FifthStage)))
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