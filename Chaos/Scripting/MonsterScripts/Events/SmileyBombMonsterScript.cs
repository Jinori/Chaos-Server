using System.Text.Json;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.IO.FileSystem;
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
    private readonly IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true)
                                                                              .Build();

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

    public int CalculateExperience(int seconds, Aisling entity)
    {
        const int MIN_EXPERIENCE = 10000;
        const int MAX_EXPERIENCE = 15000000;
        const int MAX_SECONDS = 300;

        seconds = Math.Min(seconds, MAX_SECONDS);

        if (entity.UserStatSheet.Level < 99)
        {
            var tnl = LevelUpFormulae.Default.CalculateTnl(entity);
            var minExperience2 = MathEx.GetPercentOf<decimal>(tnl, 2);
            var maxExperience2 = MathEx.GetPercentOf<decimal>(tnl, 50);
            var bonusExperience = MathEx.GetPercentOf<decimal>(tnl, 50);

            var reward = MathEx.ScaleRange(
                seconds,
                0,
                MAX_SECONDS,
                minExperience2,
                maxExperience2);

            if (seconds >= 300)
                reward += bonusExperience;

            return (int)reward;
        }

        var xpReward = MathEx.ScaleRange(
            seconds,
            0,
            MAX_SECONDS,
            MIN_EXPERIENCE,
            MAX_EXPERIENCE);

        if (seconds >= 300)
            xpReward += 20000000;

        // Get TNL for the Aisling
        return xpReward;
    }

    private void DamageTile(Point tile)
    {
        // Get all creatures on the tile
        var entities = Subject.MapInstance
                              .GetEntitiesAtPoints<Aisling>(tile)
                              .Where(x => !x.IsGodModeEnabled());

        var npc = Subject.MapInstance
                         .GetEntities<Merchant>()
                         .FirstOrDefault();

        // Deal damage to all creatures on the tile
        foreach (var entity in entities)
        {
            // Track survived seconds
            entity.Trackers.Counters.TryGetValue("frostychallenge", out var seconds);

            if (seconds > 120)
            {
                entity.Trackers.Counters.AddOrIncrement("frostysurvived2minutes");
                entity.SendOrangeBarMessage("You survived over 2 Minutes! Nice job!");
            }

            if (seconds > 0)
            {
                // Use the updated CalculateExperience method
                var expReward = CalculateExperience(seconds, entity);
                ExperienceDistributionScript.GiveExp(entity, expReward);
                entity.Trackers.Counters.Remove("frostychallenge", out _);
            }

            FrostyChallenge(entity, seconds, Configuration);

            // Warp the entity and send a message
            entity.WarpTo(new Point(4, 10));
            entity.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got blown up by a smiley bomb!");
            npc?.Say($"{entity.Name} lasted {seconds} seconds that round!");
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

    public static void FrostyChallenge(Aisling player, int seconds, IConfiguration configuration)
    {
        var stagingDirectory = configuration.GetSection("Options:ChaosOptions:StagingDirectory")
                                            .Value;

        var aislingDirectory = configuration.GetSection("Options:FrostyChallengeOptions:Directory")
                                            .Value;

        var directory = stagingDirectory + aislingDirectory;
        var filePath = Path.Combine(stagingDirectory + aislingDirectory, "frostyChallenge.json");

        directory.SafeExecute(
            _ =>
            {
                Save(filePath, seconds, player);
            });
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

    public static void Save(string filePath, int newseconds, Aisling player)
    {
        Dictionary<string, int>? secondsData;

        if (File.Exists(filePath))
        {
            var existingJson = File.ReadAllText(filePath);
            secondsData = JsonSerializer.Deserialize<Dictionary<string, int>>(existingJson);
        } else
            secondsData = new Dictionary<string, int>();

        if (secondsData != null)
        {
            // Update the damageDone for the player if it's higher than the previous value
            var playerName = player.Name;

            if (secondsData.TryGetValue(playerName, out var oldseconds))
            {
                if (newseconds > oldseconds)
                {
                    player.SendServerMessage(
                        ServerMessageType.Whisper,
                        $"New Record: {newseconds} seconds! Your last one was {oldseconds} seconds!");

                    secondsData[playerName] = newseconds;
                }
            } else
            {
                player.SendServerMessage(ServerMessageType.Whisper, $"New score of {newseconds} recorded!");
                secondsData.Add(playerName, newseconds);
            }

            // Serialize the updated dictionary and write it to the JSON file
            var updatedJson = JsonSerializer.Serialize(secondsData);
            FileEx.SafeWriteAllText(filePath, updatedJson);
        }
    }

    public override void Update(TimeSpan delta)
    {
        // Update the explosion timer
        ExplosionTimer.Update(delta);

        if (ExplosionTimer.IntervalElapsed)
            Explode();
    }
}