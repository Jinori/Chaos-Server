using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
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
    public readonly List<Aisling> ActivePlayers = [];
    private readonly Dictionary<Aisling, Aisling> AssignedTargets = new();
    private readonly List<Aisling> DeathList = [];
    private bool HandleWin;
    private bool MatchStart;
    private readonly ISimpleCache SimpleCache;

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
            InitializeMatch();
            MatchStart = true;
        }

        UpdateDeathListAndScores();

        if (!HandleWin && (GameDurationTimer.IntervalElapsed || IndividualScores.Values.Any(score => score >= 40)))
        {
            HandleWin = true;
            DeclareWinner();
        }
    }

    private void InitializeMatch()
    {
        ActivePlayers.AddRange(Subject.GetEntities<Aisling>());
        AssignRandomTargets();
    }

    private void SendMessage(string message)
    {
        foreach (var player in Subject.GetEntities<Aisling>())
            player.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }

    private void AssignRandomTargets()
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

    private void ReassignTarget(Aisling player)
    {
        var possibleTargets = ActivePlayers.Where(x => !x.Equals(player) && !AssignedTargets.ContainsValue(x)).ToList();
        if (possibleTargets.Count != 0)
        {
            AssignedTargets[player] = possibleTargets.PickRandom();
            player.SendPersistentMessage($"Target: {AssignedTargets[player].Name}");
        }
        else
            AssignedTargets.Remove(player);
    }

    private void UpdateDeathListAndScores()
    {
        var deadPlayers = Subject.GetEntities<Aisling>().Where(x => !x.IsAlive).ToList();

        foreach (var deadPlayer in deadPlayers)
        {
            if (DeathList.Contains(deadPlayer)) continue;

            DeathList.Add(deadPlayer);
            if (deadPlayer.Trackers.LastDamagedBy is Aisling killer)
            {
                if (AssignedTargets.TryGetValue(killer, out var target) && target.Equals(deadPlayer))
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

        RespawnDeadPlayers();
    }

    private void RespawnDeadPlayers()
    {
        var respawnedPlayers = DeathList.Where(player => player.IsAlive).ToList();
        foreach (var player in respawnedPlayers)
        {
            ActivePlayers.Add(player);
            ReassignTarget(player);
        }
        DeathList.RemoveAll(player => player.IsAlive);
    }

    private void DeclareWinner()
    {
        var winner = CalculateWinner();
        if (winner != null)
            HandleWinner(winner.Value.Key, winner.Value.Value);
        else
            NotifyNoWinner();
    }

    private KeyValuePair<Aisling, int>? CalculateWinner()
    {
        if (IndividualScores.Count == 0)
            return null;

        var highestScorer = IndividualScores.MaxBy(kvp => kvp.Value);
        return highestScorer;
    }

    private void HandleWinner(Aisling winner, int points)
    {
        var allPlayers = Subject.GetEntities<Aisling>().ToList();

        foreach (var player in allPlayers)
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, $"{winner.Name} has won the match with {points} points!");
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
        }
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
}