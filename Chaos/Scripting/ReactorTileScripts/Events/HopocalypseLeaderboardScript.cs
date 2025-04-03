#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.ReactorTileScripts.Events;

public class HopocalypseLeaderboardScript : ReactorTileScriptBase
{
    private static IStorage<HopocalypseLeaderboardObj>? Leaderboard { get; set; }

    /// <inheritdoc />
    public HopocalypseLeaderboardScript(ReactorTile subject, IStorage<HopocalypseLeaderboardObj>? storage)
        : base(subject)
        => Leaderboard = storage;

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        // Retrieve leaderboard entries
        var leaderboard = Leaderboard?.Value.Entries;

        // Build the leaderboard display
        var builder = new StringBuilder();
        builder.AppendLineFColored(MessageColor.Silver, "Hopocalypse Leaderboard:");
        builder.AppendLine();
        builder.AppendLineFColored(MessageColor.Orange, $"{"Name",-15} {"Eggs",-10} {"Gold Eggs",-10} {"Level",-10}");
        builder.AppendLineFColored(MessageColor.Gainsboro, new string('-', 40));

        // Order by victories (descending), then losses (ascending)
        var isSilver = true;

        if (leaderboard != null)
            foreach (var entry in leaderboard.OrderByDescending(kvp => kvp.Value.HighestLevelAchieved)
                                             .ThenBy(kvp => kvp.Value.SilverEggsReturned))
            {
                var stats = entry.Value;

                // Alternate colors
                var currentColor = isSilver ? MessageColor.Silver : MessageColor.Gainsboro;
                builder.AppendLineFColored(currentColor, $"{entry.Key,-15} {stats.SilverEggsReturned,-10} {stats.GoldenEggsReturned,-10} {stats.HighestLevelAchieved, -10}");

                // Toggle the color for the next entry
                isSilver = !isSilver;
            }

        // Send leaderboard to the player
        source.SendServerMessage(ServerMessageType.ScrollWindow, builder.ToString());
    }

    public sealed class HopocalypseLeaderboardObj
    {
        public Dictionary<string, PlayerStats> Entries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        
        public void UpdateBoard(string playerName, int silvereggs, int goldEggs, int highestLevelAchieved)
        {
            if (Entries.TryGetValue(playerName, out var stats))
            {
                if (highestLevelAchieved > stats.HighestLevelAchieved)
                    stats.HighestLevelAchieved = highestLevelAchieved;
                
                if (silvereggs > stats.SilverEggsReturned)
                    stats.SilverEggsReturned = silvereggs;
                
                if (goldEggs > stats.GoldenEggsReturned)
                    stats.GoldenEggsReturned = goldEggs;
            }
            else
            {
                Entries[playerName] = new PlayerStats
                {
                    HighestLevelAchieved = highestLevelAchieved,
                    GoldenEggsReturned = goldEggs,
                    SilverEggsReturned = silvereggs
                };   
            }
            Leaderboard?.Save();
        }

        public sealed class PlayerStats
        {
            public int HighestLevelAchieved { get; set; }
            public int GoldenEggsReturned { get; set; }
            public int SilverEggsReturned { get; set; }
        }
    }
}