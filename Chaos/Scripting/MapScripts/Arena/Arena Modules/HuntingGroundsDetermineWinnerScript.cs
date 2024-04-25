using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class HuntingGroundsDetermineWinnerScript : MapScriptBase
{
    private readonly IIntervalTimer GameDurationTimer;
    public readonly Dictionary<Aisling, int> IndividualScores = new();
    public readonly List<Aisling> ActivePlayers = new(); 
    private readonly Dictionary<Aisling, Aisling> AssignedTargets = new();
    private readonly List<Aisling> DeathList = [];
    private bool HandleWin;
    private bool MatchStart;
    private readonly ISimpleCache SimpleCache;
    
    /// <inheritdoc />
    public HuntingGroundsDetermineWinnerScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        GameDurationTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(4),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "Match will conclude in {Time}.",
            SendMessage);
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
        
        if (Subject.GetEntities<Aisling>().Count() < 2)
            return;

        if (!MatchStart)
        {
            foreach (var player in Subject.GetEntities<Aisling>())
                ActivePlayers.Add(player);

            AssignRandomTargets();
            MatchStart = true;
        }
        
        UpdateDeathListAndScores();
        
        if (!HandleWin && (GameDurationTimer.IntervalElapsed || (Subject.GetEntities<Aisling>().Count(x => x.IsAlive) == 1)))
        {
            HandleWin = true;
            
            var winner = CalculateScores() ?? HandleLastPlayerStanding();
        
            if (winner != null)
                HandleWinner(winner.Value.Key, winner.Value.Value);
            else
                NotifyNoWinner();
        }
    }

    public void AssignRandomTargets()
    {
        var random = new Random();
        var playerList = ActivePlayers.ToList();

        foreach (var player in ActivePlayers)
        {
            var possibleTargets = playerList.Where(x => !x.Equals(player) && !AssignedTargets.ContainsValue(x)).ToList();

            if (possibleTargets.Count != 0)
            {
                AssignedTargets[player] = possibleTargets[random.Next(possibleTargets.Count)];
                player.SendPersistentMessage($"Target: {AssignedTargets[player].Name}");
            }
        }
    }
    
    private void ReassignTarget(Aisling killer)
    {
        var possibleTargets = ActivePlayers.Where(x => !x.Equals(killer) && !AssignedTargets.ContainsValue(x)).ToList();

        if (possibleTargets.Count != 0)
        {
            AssignedTargets[killer] = possibleTargets[IntegerRandomizer.RollSingle(possibleTargets.Count)];
            killer.SendPersistentMessage($"Target: {AssignedTargets[killer].Name}");
        }
        else
            AssignedTargets.Remove(killer);
    }

    public void UpdateDeathListAndScores()
    {
        var list = Subject.GetEntities<Aisling>().Where(x => !x.IsAlive).ToList();

        foreach (var deadPlayer in list)
            if (!DeathList.Contains(deadPlayer))
            {
                DeathList.Add(deadPlayer);

                if (deadPlayer.Trackers.LastDamagedBy is Aisling killer)
                {
                    if (AssignedTargets.TryGetValue(killer, out var target) && (target.Equals(deadPlayer)))
                    {
                        IndividualScores[killer] = IndividualScores.GetValueOrDefault(killer, 0) + 5;
                        ReassignTarget(killer);
                    }
                    else
                        IndividualScores[killer] = IndividualScores.GetValueOrDefault(killer, 0) + 1;

                    ActivePlayers.Remove(deadPlayer);
                    AssignedTargets.Remove(deadPlayer);

                    foreach (var player in AssignedTargets.Where(kvp => kvp.Value.Equals(deadPlayer)).ToList())
                        ReassignTarget(player.Key);
                }
            }
    }

    
    private KeyValuePair<Aisling, int>? HandleLastPlayerStanding()
    {
        if (ActivePlayers.Count == 1)
        {
            var lastPlayer = ActivePlayers[0];
            var score = IndividualScores.GetValueOrDefault(lastPlayer, 0);
            return new KeyValuePair<Aisling, int>(lastPlayer, score);
        }
        return null;
    }
    
    private void NotifyNoWinner()
    {
        foreach (var player in Subject.GetEntities<Aisling>())
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, "The match concluded with no winner.");
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
        }
    }
    
    private void HandleWinner(Aisling winner, int points)
    {
        var allToPort = Subject.GetEntities<Aisling>().ToList();

        foreach (var player in allToPort)
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, $"{winner.Name} has won the match with {points} points!");
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
        }
    }
    
    private KeyValuePair<Aisling, int>? CalculateScores()
    {
        if (IndividualScores.Count == 0)
            return null;
        
        var highestScorer = IndividualScores.MaxBy(kvp => kvp.Value);
        
        return highestScorer;
    }
}