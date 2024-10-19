using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Rogue;

public class LimboRogueScript : MonsterScriptBase
{
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer ActionTimer;
    private readonly Skill ShadowFigure;
    private readonly Skill Throw;
    private readonly Spell PitfallTrap;
    
    public LimboRogueScript(Monster subject, ISkillFactory skillFactory, ISpellFactory spellFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 25, startAsElapsed: false);

        ShadowFigure = SkillFactory.Create("shadowfigure");
        Throw = SkillFactory.Create("throw");
        PitfallTrap = SpellFactory.Create("pitfalltrap");
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;
        
        var target = Subject.Target;

        if (target is null)
            return;

        var targetDirection = target.DirectionalRelationTo(Subject);

        //if standing next to target and facing them
        if (Subject.WithinRange(target, 1)
            && (Subject.Direction == targetDirection)
            && Subject.CanUse(ShadowFigure, out _)
            && Subject.CanUse(Throw, out _)
            && Subject.CanUse(
                PitfallTrap,
                Subject,
                null,
                out _))
        {
            var oppositePoint = target.DirectionalOffset(targetDirection);

            if (Map.IsWalkable(oppositePoint, Subject.Type))
            {
                Subject.TryUseSpell(PitfallTrap);
                Subject.TryUseSkill(ShadowFigure);
                Subject.TryUseSkill(Throw);
            }
        }
    }
}