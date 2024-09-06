using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Arena;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class ColorClashDetermineWinnerScript : MapScriptBase
{
    //Arena Underground Team Warps
    private readonly Point GoldPoint = new(15, 15);
    private readonly Point GreenPoint = new(16, 6);
    private readonly Point RedPoint = new(6, 6);
    private readonly Point BluePoint = new(6, 15);
    private readonly Point CenterPoint = new(13, 10);
    private readonly IIntervalTimer GameDurationTimer;
    private readonly Dictionary<ArenaTeam, int> TeamScores = new();
    private readonly ISimpleCache SimpleCache;
    private bool LayAllColorClashReactors;
    private readonly IReactorTileFactory ReactorTileFactory;
    private bool HandleWin;
    
    public ColorClashDetermineWinnerScript(MapInstance subject, ISimpleCache simpleCache, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        SimpleCache = simpleCache;
        GameDurationTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(4),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "Match will conclude in {Time}.",
            SendMessage);

        TeamScores.Add(ArenaTeam.Blue, 0);
        TeamScores.Add(ArenaTeam.Gold, 0);
        TeamScores.Add(ArenaTeam.Green, 0);
        TeamScores.Add(ArenaTeam.Red, 0);
    }
    
    private void SendMessage(string message)
    {
        foreach (var player in Subject.GetEntities<Aisling>())
            player.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }

    public override void Update(TimeSpan delta)
    {
        GameDurationTimer.Update(delta);

        if (!Subject.GetEntities<Aisling>().Any() && HandleWin)
        {
            Subject.Destroy();
            return;
        }
        
        if (!LayAllColorClashReactors)
            PlaceColorClashReactors();
        
        if (!GameDurationTimer.IntervalElapsed)
            return;

        if (!HandleWin)
        {
            CalculateScores();

            // Determine the winning team
            var winningTeam = TeamScores.MaxBy(x => x.Value).Key;

            // Handle the winning team (e.g., announce the winner, reward players, etc.)
            HandleWinningTeam(winningTeam);
            HandleWin = true;
        }
    }

    private void PlaceColorClashReactors()
    {
        var rect = new Rectangle(new Point(16, 16), 15, 15);
        var points = rect.GetPoints();
        
        var excludedPoints = new HashSet<Point>
        {
            new(23, 17), new(23, 16), new(23, 15), new(23, 14),
            new(17, 23), new(16, 23), new(15, 23), new(14, 23)
        };

        foreach (var point in points)
        {
            if (!Subject.IsWall(point) && !excludedPoints.Contains(point))
            {
                var reactorTile = ReactorTileFactory.Create("ColorClash", Subject, point);
                Subject.SimpleAdd(reactorTile);
            }
        }

        LayAllColorClashReactors = true;
    }
    
    private void CalculateScores()
    {
        foreach (var reactorTile in Subject.GetEntities<ReactorTile>())
        {
            if (!reactorTile.Script.Is<ColorClashScript>(out var colorClashScript) ||
                !colorClashScript.CurrentTeam.HasValue) 
                continue;
        
            var team = colorClashScript.CurrentTeam.Value;
            IncrementTeamScore(team);
        }
    }

    private void IncrementTeamScore(ArenaTeam team)
    {
        if (TeamScores.TryGetValue(team, out var value))
        {
            TeamScores[team] = ++value;
        }
        else
        {
            TeamScores[team] = 1;
        }
    }


     
    private void HandleWinningTeam(ArenaTeam winningTeam)
    {
        ResetMaps();
        
        var playersThatWon = Subject.GetEntities<Aisling>()
                                    .Where(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value == winningTeam));

        foreach (var winner in playersThatWon)
            winner.SendActiveMessage($"Your team has won the round! Go {winningTeam}!");

        var playersThatLost = Subject.GetEntities<Aisling>()
                                     .Where(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value != winningTeam));

        foreach (var loser in playersThatLost)
            loser.SendActiveMessage($"{winningTeam} has won the round. Better luck next time.");
        
        var allToPort = Subject.GetEntities<Aisling>();

        foreach (var player in allToPort)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            
            player.Trackers.Enums.TryGetValue(out ArenaTeam value);
            
            switch (value)
            {
                case ArenaTeam.Blue:
                    player.TraverseMap(mapInstance, BluePoint);

                    break;
                case ArenaTeam.Green:
                    player.TraverseMap(mapInstance, GreenPoint);

                    break;
                case ArenaTeam.Gold:
                    player.TraverseMap(mapInstance, GoldPoint);

                    break;
                case ArenaTeam.Red:
                    player.TraverseMap(mapInstance, RedPoint);
                    
                    break;
                
                case ArenaTeam.None:
                    player.TraverseMap(mapInstance, CenterPoint);
                    break;
                
                default:
                    player.TraverseMap(mapInstance, CenterPoint);
                    break;
            }
        }
    }
    
    private void ResetMaps()
    {
        switch (Subject.Name)
        {
            case "Color Clash - Teams":
            {
                foreach (var tile in Subject.GetEntities<ReactorTile>())
                {
                    var script = tile.Script.As<ColorClashScript>();
                    if (script != null)
                        tile.RemoveScript<ColorClashScript>();
                }
                Subject.Morph("26014");
                LayAllColorClashReactors = false;
                HandleWin = false;
            }
                break;
        }
    }
}