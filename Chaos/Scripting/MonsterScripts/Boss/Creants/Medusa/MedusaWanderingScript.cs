using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using FluentAssertions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Medusa;

// ReSharper disable once ClassCanBeSealed.Global
public class MedusaWanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public MedusaWanderingScript(Monster subject)
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

        var script = Subject.Script.As<MedusaPhaseScript>();

        if (script!.InPhase)
            return;

        Subject.Wander();
        Subject.MoveTimer.Reset();
    }
}