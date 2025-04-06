using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class CastingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public CastingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.WithinRange(Subject))
            return;

        var chance = 10;

        if (Target.WithinRange(Subject, 1))
            chance /= 5;
        
        var spell = Spells.Where(
                              spell => Subject.CanUse(
                                  spell,
                                  Target,
                                  null,
                                  out _))
                          .PickRandomWeightedSingleOrDefault(chance);

        if (spell is null)
            return;

        uint? targetId = spell.Template.SpellType switch
        {
            SpellType.Prompt      => null,
            SpellType.Targeted    => Target.Id,
            SpellType.Prompt4Nums => null,
            SpellType.Prompt3Nums => null,
            SpellType.NoTarget    => null,
            SpellType.Prompt2Nums => null,
            SpellType.Prompt1Num  => null,
            _                     => throw new ArgumentOutOfRangeException()
        };

        if (Subject.TryUseSpell(spell, targetId))
        {
            Subject.WanderTimer.Reset();
            Subject.MoveTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }
}