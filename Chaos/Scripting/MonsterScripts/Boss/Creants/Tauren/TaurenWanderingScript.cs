using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using FluentAssertions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

// ReSharper disable once ClassCanBeSealed.Global
public class TaurenWanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TaurenWanderingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Target != null) || !ShouldWander)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var script = Subject.Script.As<TaurenPhaseScript>();

        if (script!.InPhase)
            return;

        Subject.Wander();
        Subject.MoveTimer.Reset();
    }
}