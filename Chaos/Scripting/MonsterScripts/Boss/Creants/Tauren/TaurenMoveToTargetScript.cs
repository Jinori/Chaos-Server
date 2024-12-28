using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using FluentAssertions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

public class TaurenMoveToTargetScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TaurenMoveToTargetScript(Monster subject)
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

        var script = Subject.Script.As<TaurenPhaseScript>();

        if (script!.InPhase)
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

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