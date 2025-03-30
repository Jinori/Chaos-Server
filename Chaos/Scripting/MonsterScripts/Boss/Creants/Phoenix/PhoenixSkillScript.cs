using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

public class PhoenixSkillScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PhoenixSkillScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (Target is not { IsAlive: true } || (Subject.ManhattanDistanceFrom(Target) != 1))
            return;

        if (Map.LoadedFromInstanceId.ContainsI("sky"))
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

        //do not use skills while in abduct or drop phase
        var currentPhase = Subject.Script.As<PhoenixPhaseScript>()!.CurrentPhase;

        if (currentPhase != PhoenixPhaseScript.Phase.Normal)
            return;

        foreach (var skill in Skills)
            if (skill.Template.IsAssail)
                attacked |= Subject.TryUseSkill(skill);

        if (ShouldUseSkill)
        {
            var skill = Skills.Where(skill => Subject.CanUse(skill, out _))
                              .PickRandomWeightedSingleOrDefault(25);

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