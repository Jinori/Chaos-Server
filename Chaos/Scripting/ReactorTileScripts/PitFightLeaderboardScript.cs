#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.ReactorTileScripts;

public class PitFightLeaderboardScript : ReactorTileScriptBase
{
    private static IStorage<PitFightLeaderboardObj>? Leaderboard { get; set; }

    /// <inheritdoc />
    public PitFightLeaderboardScript(ReactorTile subject, IStorage<PitFightLeaderboardObj>? storage)
        : base(subject)
        => Leaderboard = storage;

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        // Retrieve leaderboard entries
        var leaderboard = Leaderboard?.Value.Entries;

        // Build the leaderboard display
        var builder = new StringBuilder();
        builder.AppendLineFColored(MessageColor.Silver, "Pit Fight Leaderboard:");
        builder.AppendLine();
        builder.AppendLineFColored(MessageColor.Orange, $"{"Name",-15} {"Victories",-10} {"Losses",-10}");
        builder.AppendLineFColored(MessageColor.Gainsboro, new string('-', 40));

        // Order by victories (descending), then losses (ascending)
        var isSilver = true;

        if (leaderboard != null)
            foreach (var entry in leaderboard.OrderByDescending(kvp => kvp.Value.Victories)
                                             .ThenBy(kvp => kvp.Value.Losses))
            {
                var stats = entry.Value;

                // Alternate colors
                var currentColor = isSilver ? MessageColor.Silver : MessageColor.Gainsboro;
                builder.AppendLineFColored(currentColor, $"{entry.Key,-15} {stats.Victories,-10} {stats.Losses,-10}");

                // Toggle the color for the next entry
                isSilver = !isSilver;
            }

        // Send leaderboard to the player
        source.SendServerMessage(ServerMessageType.ScrollWindow, builder.ToString());
    }

    public sealed class PitFightLeaderboardObj
    {
        public Dictionary<string, PlayerStats> Entries { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        // Add a loss to a player
        public void AddLoss(string playerName)
        {
            if (Entries.TryGetValue(playerName, out var stats))
                stats.Losses++;
            else
                Entries[playerName] = new PlayerStats
                {
                    Losses = 1
                };
            Leaderboard?.Save();
        }

        // Add a victory to a player
        public void AddVictory(string playerName)
        {
            if (Entries.TryGetValue(playerName, out var stats))
                stats.Victories++;
            else
                Entries[playerName] = new PlayerStats
                {
                    Victories = 1
                };
            Leaderboard?.Save();
        }

        public class PlayerStats
        {
            public int Losses { get; set; }
            public int Victories { get; set; }
        }
    }
}