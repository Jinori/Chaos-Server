using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

// ReSharper disable once ClassCanBeSealed.Global
public class TaurenCastingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TaurenCastingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.WithinRange(Subject))
            return;

        var script = Subject.Script.As<TaurenPhaseScript>();

        if (script is { InPhase: true })
            return;

        Spells.ShuffleInPlace();

        var spell = Spells.Where(
                              spell => Subject.CanUse(
                                  spell,
                                  Target,
                                  null,
                                  out _))
                          .PickRandomWeightedSingleOrDefault(7);

        if (spell is not null && Subject.TryUseSpell(spell, Target.Id))
        {
            Subject.WanderTimer.Reset();
            Subject.MoveTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }
}