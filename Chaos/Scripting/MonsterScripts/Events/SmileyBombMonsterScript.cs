using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events;

public class SmileyBombMonsterScript : MonsterScriptBase
{
    private readonly IIntervalTimer ExplosionTimer;
    private readonly Animation ExplosionAnimation = new()
    {
        TargetAnimation = 992,
        AnimationSpeed = 100
    };
    
    private readonly Animation SideAnimation = new()
    {
        TargetAnimation = 408,
        AnimationSpeed = 100
    };

    public SmileyBombMonsterScript(Monster subject)
        : base(subject)
    {
        // Random explosion time between 3 to 10 seconds
        ExplosionTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(3), 40, RandomizationType.Positive, false);
    }

    public override void Update(TimeSpan delta)
    {
        // Update the explosion timer
        ExplosionTimer.Update(delta);

        // Show side animations while the bomb is waiting to explode
        ShowSideAnimations();

        if (ExplosionTimer.IntervalElapsed)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Play explosion animation on the bomb's tile
        Subject.MapInstance.ShowAnimation(ExplosionAnimation.GetPointAnimation(Subject));

        // Damage entities on the bomb's tile
        DamageTile(new Point(Subject.X, Subject.Y));

        // Damage entities on adjacent tiles
        var adjacentTiles = GetAdjacentTiles(new Point(Subject.X, Subject.Y));
        foreach (var tile in adjacentTiles)
        {
            DamageTile(tile);
            Subject.MapInstance.ShowAnimation(ExplosionAnimation.GetPointAnimation(tile));
        }

        // Remove the bomb monster from the map after explosion
        Map.RemoveEntity(Subject);
    }

    private void ShowSideAnimations()
    {
        var center = new Point(Subject.X, Subject.Y);
        var adjacentTiles = GetAdjacentTiles(center);

        foreach (var tile in adjacentTiles)
        {
            if (!Subject.MapInstance.GetEntitiesAtPoints<Monster>(tile).Any() && !Map.IsWall(tile)) 
                Subject.MapInstance.ShowAnimation(SideAnimation.GetPointAnimation(tile));
        }
    }

    private void DamageTile(Point tile)
    {
        // Get all creatures on the tile
        var entities = Subject.MapInstance.GetEntitiesAtPoints<Aisling>(tile);

        // Deal damage to all creatures on the tile
        foreach (var entity in entities)
        {
            entity.WarpTo(new Point(4, 10));
            entity.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got blown up by a smiley bomb!");
        }
    }

    private IEnumerable<Point> GetAdjacentTiles(Point center)
    {
        // Return adjacent tiles (North, South, East, West)
        return new[]
        {
            new Point(center.X, center.Y - 1), // North
            new Point(center.X, center.Y + 1), // South
            new Point(center.X - 1, center.Y), // West
            new Point(center.X + 1, center.Y)  // East
        };
    }
}
