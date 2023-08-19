using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Arena;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
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
        for (var x = 0; x < Subject.Template.Width; x++)
        {
            for (var y = 0; y < Subject.Template.Height; y++)
            {
                var location = new Point(x, y);
            
                // Check if the tile at the current location is a wall
                if (!Subject.IsWall(location))
                {
                    // Create a reactor tile at the current location if it's not a wall
                    var reactorTile = ReactorTileFactory.Create("ColorClash", Subject, location);
                    Subject.SimpleAdd(reactorTile);
                }
            }
        }

        LayAllColorClashReactors = true;
    }
    
    private void CalculateScores()
    {
        foreach (var reactorTile in Subject.GetEntities<ReactorTile>())
        {
            if (reactorTile.Script.Is<ColorClashScript>(out var colorClashScript) && colorClashScript.CurrentTeam.HasValue)
            {
                TeamScores[colorClashScript.CurrentTeam.Value]++;
            }
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
            player.Trackers.Enums.TryGetValue(out ArenaTeam value);
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");

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
                        tile.RemoveScript<IReactorTileScript, ColorClashScript>();
                }
                Subject.Morph("26014");
                LayAllColorClashReactors = false;
                HandleWin = false;
            }
                break;
        }
    }
}