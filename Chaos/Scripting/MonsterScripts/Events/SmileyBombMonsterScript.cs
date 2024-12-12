using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events;

public class SmileyBombMonsterScript : MonsterScriptBase
{
    private readonly Animation ExplosionAnimation = new()
    {
        TargetAnimation = 992,
        AnimationSpeed = 100
    };

    private readonly IIntervalTimer ExplosionTimer;

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public SmileyBombMonsterScript(Monster subject)
        : base(subject)

    // Random explosion time between 3 to 10 seconds
        => ExplosionTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            30,
            RandomizationType.Balanced,
            false);

    public int CalculateExperience(int seconds)
    {
        const double a = 1388.89;
        const int baseExperience = 25000;

        // Cap seconds at 120 to avoid exceeding max experience
        seconds = Math.Min(seconds, 120);

        return (int)(a * Math.Pow(seconds, 2) + baseExperience);
    }

    private void DamageTile(Point tile)
    {
        // Get all creatures on the tile
        var entities = Subject.MapInstance
                              .GetEntitiesAtPoints<Aisling>(tile)
                              .Where(x => !x.IsGodModeEnabled());

        // Deal damage to all creatures on the tile
        foreach (var entity in entities)
        {
            var survivedSeconds = entity.Trackers.Counters.TryGetValue("frostychallenge", out var seconds);

            if (seconds > 120)
            {
                entity.Trackers.Counters.AddOrIncrement("frostysurvived2minutes");
                entity.SendOrangeBarMessage("You survived over 2 Minutes! Nice job!");
            }

            if (seconds > 0)
            {
                var expReward = CalculateExperience(seconds);
                ExperienceDistributionScript.GiveExp(entity, expReward);
                entity.Trackers.Counters.Remove("frostychallenge", out _);
            }

            entity.WarpTo(new Point(4, 10));
            entity.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got blown up by a smiley bomb!");
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

    private IEnumerable<Point> GetAdjacentTiles(Point center)
        =>

            // Return adjacent tiles (North, South, East, West)
            [
                new(center.X, center.Y - 1), // North
                new(center.X, center.Y + 1), // South
                new(center.X - 1, center.Y), // West
                new(center.X + 1, center.Y) // East
            ];

    public override void Update(TimeSpan delta)
    {
        // Update the explosion timer
        ExplosionTimer.Update(delta);

        if (ExplosionTimer.IntervalElapsed)
            Explode();
    }
}