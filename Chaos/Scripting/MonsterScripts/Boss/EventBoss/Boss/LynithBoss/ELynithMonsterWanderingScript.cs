using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.LynithBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class ELynithMonsterWanderingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ELynithMonsterWanderingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        // Don't proceed if the monster has a target or shouldn't wander
        if ((Target != null) || !ShouldWander)
            return;

        // Check if there are any players (Aislings) on the map
        if (!Map.GetEntities<Aisling>().Any())
            return;

        // Get the center point of the map
        var centerX = Subject.MapInstance.Template.Width / 2;
        var centerY = Subject.MapInstance.Template.Height / 2;
        var centerPoint = new Point(centerX, centerY);

        // Make the monster pathfind to the center of the map
        Subject.Pathfind(centerPoint);

        // Reset the monster's move timer
        Subject.MoveTimer.Reset();
    }

}