using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
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
    private readonly IIntervalTimer ActionTimer;
    private readonly Spell PitfallTrap;
    private readonly Skill ShadowFigure;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly Skill Throw;
    private readonly IIntervalTimer AggroWipeTimer;
    private static readonly int[] ClassWeights =
    [
        0, //peasant
        0, //warrior
        0, //rogue
        1, //wizard
        2, //priest
        0 //monk
    ];

    public LimboRogueScript(Monster subject, ISkillFactory skillFactory, ISpellFactory spellFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 25, startAsElapsed: false);
        AggroWipeTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(3), 25, startAsElapsed: false);

        ShadowFigure = SkillFactory.Create("shadowfigure");
        Throw = SkillFactory.Create("throw");
        PitfallTrap = SpellFactory.Create("pitfalltrap");
    }

    public override void Update(TimeSpan delta)
    {
        PitfallTrap.Update(delta);
        ShadowFigure.Update(delta);
        Throw.Update(delta);
        
        AggroWipeTimer.Update(delta);
        ActionTimer.Update(delta);

        if (AggroWipeTimer.IntervalElapsed)
        {
            Subject.ResetAggro();

            var possibleTargets = Map.GetEntitiesWithinRange<Aisling>(Subject, 10)
                                     .ThatAreVisibleTo(Subject)
                                     .Where(obj => !obj.Equals(Subject) && obj.IsAlive)
                                     .ToList();

            if (possibleTargets.Count > 0)
            {
                var classesByWeight = possibleTargets.Select(aisling => aisling.UserStatSheet.BaseClass)
                                                     .Distinct()
                                                     .Where(baseClass => baseClass is BaseClass.Wizard or BaseClass.Priest)
                                                     .ToDictionary(@class => @class, @class => ClassWeights[(int)@class]);

                if (classesByWeight.Count > 0)
                {
                    var targetClass = classesByWeight.PickRandomWeighted();
                    
                    Target = possibleTargets.Where(t => t.UserStatSheet.BaseClass == targetClass)
                                            .ToList()
                                            .PickRandom();

                    Subject.AggroList[Target.Id] = 100_000;
                }
            }
        }
        
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

            if (Map.IsWalkable(oppositePoint, collisionType: Subject.Type))
            {
                Subject.TryUseSpell(PitfallTrap);
                Subject.TryUseSkill(ShadowFigure);
                Subject.TryUseSkill(Throw);
            }
        }
    }
}