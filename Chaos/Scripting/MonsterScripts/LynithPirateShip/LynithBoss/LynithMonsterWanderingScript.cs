using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.LynithPirateShip.LynithBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class LynithMonsterWanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public LynithMonsterWanderingScript(Monster subject)
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

        var point = new Point(16, 13);
        Subject.Pathfind(point);
        Subject.MoveTimer.Reset();
    }
}