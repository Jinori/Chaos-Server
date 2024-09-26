using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.DragonScaleBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class EDragonScaleWanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public EDragonScaleWanderingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Subject.Effects.TryGetEffect("Invulnerability", out _) && ShouldWander)
        {
            Subject.Wander();
            Subject.MoveTimer.Reset();
            return;
        }

        if ((Target != null) || !ShouldWander)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        Subject.Wander();
        Subject.MoveTimer.Reset();
    }
}