namespace Chaos.Scripting.ReactorTileScripts.Events.Easter;

public sealed class HopocalypseLeaderboardObj
{
    public Dictionary<string, HopPlayerStats> Entries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class HopPlayerStats
{
    public int HighestLevelAchieved { get; set; }
    public int GoldenEggsReturned { get; set; }
    public int SilverEggsReturned { get; set; }
}