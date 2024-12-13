using System.Text.Json;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.FileSystem;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Events;

public class MoveInDirectionReindeerScript : MonsterScriptBase
{
    private readonly IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true)
                                                                              .Build();

    private readonly List<Point> ExitLinePoints;
    private readonly IIntervalTimer RandomWalkInterval;
    private readonly IIntervalTimer SpawnDelayTimer;
    private bool SpawnTimerOver;

    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public MoveInDirectionReindeerScript(Monster subject)
        : base(subject)
    {
        RandomWalkInterval = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(400), 70, RandomizationType.Negative);

        SpawnDelayTimer = new IntervalTimer(TimeSpan.FromSeconds(2), false); // 2-second delay before walking

        // Generate the exit line points (25x, 8y to 25x, 20y)
        ExitLinePoints = GenerateExitLinePoints();
    }

    public int CalculateExperience(int seconds, Aisling aisling)
    {
        const int MIN_EXPERIENCE = 10000;
        const int MAX_EXPERIENCE = 7500000;
        const int MAX_SECONDS = 120;

        // Cap seconds at the maximum allowed
        seconds = Math.Min(seconds, MAX_SECONDS);

        // Scale experience linearly between minExperience and maxExperience
        var experience = MIN_EXPERIENCE + (MAX_EXPERIENCE - MIN_EXPERIENCE) * ((double)seconds / MAX_SECONDS);

        if (aisling.UserStatSheet.Level < 98)
        {
            var tnl = LevelUpFormulae.Default.CalculateTnl(aisling);
            var percentage = seconds / 6;
            var expReward = Convert.ToInt32(percentage * tnl);

            return expReward;
        }

        return (int)experience;
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

    private List<Point> GenerateExitLinePoints()
    {
        // Generate all points in the line (25x, 8y to 25x, 20y)
        var points = new List<Point>();

        for (var y = 7; y <= 21; y++)
            points.Add(new Point(25, y));

        return points;
    }

    private void HandleTrample(Aisling aisling)
    {
        if (aisling.IsGodModeEnabled())
            return;

        var npc = Subject.MapInstance
                         .GetEntities<Merchant>()
                         .FirstOrDefault();

        aisling.Trackers.Counters.TryGetValue("frostychallenge", out var seconds);

        if (seconds > 120)
        {
            aisling.Trackers.Counters.AddOrIncrement("frostysurvived2minutes");
            aisling.SendOrangeBarMessage("You survived over 2 Minutes! Nice job!");
        }

        if (seconds > 0)
        {
            var expReward = CalculateExperience(seconds, aisling);
            ExperienceDistributionScript.GiveExp(aisling, expReward);
            aisling.Trackers.Counters.Remove("frostychallenge", out _);
        }

        npc?.Say($"{aisling.Name} lasted {seconds} seconds that round!");

        FrostyChallenge(aisling, seconds, Configuration);

        // Warp the Aisling to the designated point
        aisling.WarpTo(new Point(4, 11));

        // Send feedback message to the Aisling
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You got ran over by a reindeer!");

        // Optional: Add additional logic (e.g., stagger the reindeer, cooldown mechanics)
    }

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
        // Update walk interval
        RandomWalkInterval.Update(delta);

        if (Subject.Direction != Direction.Right)
            Subject.Turn(Direction.Right);

        if (!SpawnTimerOver)
        {
            // Update spawn delay timer
            SpawnDelayTimer.Update(delta);

            // Wait until the delay is complete
            if (!SpawnDelayTimer.IntervalElapsed)
                return;

            // Mark delay as complete and allow normal walking behavior
            SpawnTimerOver = true;
        }

        if (!RandomWalkInterval.IntervalElapsed)
            return;

        // Check if the reindeer is at any exit point
        if (ExitLinePoints.Contains(new Point(Subject.X, Subject.Y)))
        {
            // Remove the reindeer from the map
            Map.RemoveEntity(Subject);

            return;
        }

        var targetDirection = Subject.Direction; // Reindeer's current direction
        var nextPosition = Subject.DirectionalOffset(targetDirection);

        // Check if there's a wall, blocking reactor, or any other obstacle
        if (Subject.MapInstance.IsWall(nextPosition) || Subject.MapInstance.IsBlockingReactor(nextPosition))
            return;

        // Check if an Aisling is directly in front
        var aislingInFront = Subject.MapInstance
                                    .GetEntitiesAtPoints<Aisling>(nextPosition)
                                    .TopOrDefault();

        if ((aislingInFront != null) && !aislingInFront.IsGodModeEnabled())
        {
            HandleTrample(aislingInFront);

            return;
        }

        // Move the reindeer forward if no obstacles or Aislings
        Subject.Walk(Direction.Right);
    }
}