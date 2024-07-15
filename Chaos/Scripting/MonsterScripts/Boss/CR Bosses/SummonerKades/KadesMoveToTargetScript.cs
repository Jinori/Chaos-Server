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

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        if (Map.GetEntities<Aisling>().Any(x => x.Trackers.Enums.HasValue(SummonerBossFight.FirstStage) 
                                                || x.Trackers.Enums.HasValue(SummonerBossFight.SecondStage) 
                                                || x.Trackers.Enums.HasValue(SummonerBossFight.ThirdStage) 
                                                || x.Trackers.Enums.HasValue(SummonerBossFight.FourthStage)
                                                || x.Trackers.Enums.HasValue(SummonerBossFight.FifthStage)))
            return;

        var distance = Subject.DistanceFrom(Target);

        if (distance != 1)
            Subject.Pathfind(Target);
        else
        {
            var direction = Target.DirectionalRelationTo(Subject);
            Subject.Turn(direction);
        }

        Subject.WanderTimer.Reset();
        Subject.SkillTimer.Reset();
    }
}