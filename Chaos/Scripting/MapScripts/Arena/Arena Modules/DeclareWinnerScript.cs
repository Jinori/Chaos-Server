using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class DeclareWinnerScript : MapScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private IIntervalTimer DeclareWinnerTimer { get; }
    private bool DetermineIfTeamGameOrFfa;
    private bool TeamGame;
    private bool WinnerDeclared;
    private bool IsHostPlaying;
    
    //Arena Underground Team Warps
    private readonly Point GoldPoint = new(15, 15);
    private readonly Point GreenPoint = new(16, 6);
    private readonly Point RedPoint = new(6, 6);
    private readonly Point BluePoint = new(6, 15);
    
    /// <inheritdoc />
    public DeclareWinnerScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        DeclareWinnerTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
    }
        

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        DeclareWinnerTimer.Update(delta);
        
        if (!DeclareWinnerTimer.IntervalElapsed)
            return;

        if (!Subject.GetEntities<Aisling>().Any() && WinnerDeclared)
        {
            Subject.Destroy();
            return;
        }
        
        if (!DetermineIfTeamGameOrFfa)
            DetermineMatchMode();

        CheckForWinners();
    }

    private void CheckForWinners()
    {
        if (WinnerDeclared)
            return;
        
        var allAislings = Subject.GetEntities<Aisling>().ToList();
        
        if (TeamGame)
            CheckForTeamWinners(allAislings);
        else
            CheckForFreeForAllWinners(allAislings);
    }
    
    private void CheckForFreeForAllWinners(IReadOnlyList<Aisling> allAislings)
    {
        if (WinnerDeclared)
            return;
    
        // Filtering out the host if they are not playing
        var aliveAislings = GetAliveAislings(allAislings)
                            .Where(aisling => IsHostPlaying || !IsHost(aisling)).ToList();
    
        switch (aliveAislings.Count)
        {
            case 0:
                DeclareNoWinners();

                break;
            case 1:
                DeclareWinners(aliveAislings);

                break;
            default:
            {
                if (aliveAislings.Any(x => x.Effects.Contains("HiddenHavocHide")) && (aliveAislings.Count <= 2))
                    DeclareWinners(aliveAislings);

                break;
            }
        }
    }
    
    private void CheckForTeamWinners(IEnumerable<Aisling> allAislings)
    {
        if (WinnerDeclared)
            return;

        var aislingList = allAislings.ToList();
        
        if (aislingList.All(x => !x.IsAlive))
        {
            DeclareNoWinners();
            return;
        }
        
        var activeTeams = GetActiveTeams(aislingList);

        if ((activeTeams.Count == 1) && (!IsHostPlaying || (activeTeams[0] != ArenaTeam.None)))
            DeclareTeamWinner(activeTeams.First());
    }
    
    private void DetermineMatchMode()
    {
        if (!DetermineIfTeamGameOrFfa)
        {
            DetermineIfTeamGameOrFfa = true;
            
            var allAislings = Subject.GetEntities<Aisling>().ToList();
            IsHostPlaying = Subject.GetEntities<Aisling>()
                                       .Any(x => x.Trackers.Enums.TryGetValue(out ArenaHostPlaying value) 
                                                 && (value == ArenaHostPlaying.Yes));
            
            var activeTeams = GetActiveTeams(allAislings);

            if (activeTeams.Count > 0)
                TeamGame = true;
        }
    }
    
    private List<ArenaTeam> GetActiveTeams(IEnumerable<Aisling> allAislings)
    {
        var activeTeams = allAislings
                          .Where(x => x.IsAlive && x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value != ArenaTeam.None))
                          .Select(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) ? value : ArenaTeam.None)
                          .Distinct()
                          .ToList();

        return activeTeams;
    }
   
    
    private IEnumerable<Aisling> GetAliveAislings(IEnumerable<Aisling> allAislings) => allAislings.Where(x => x.IsAlive && (IsHostPlaying || !IsHost(x))).ToList();

    private void DeclareWinners(IReadOnlyList<Aisling> winners)
    {
        var allToPort = Subject.GetEntities<Aisling>().ToList();

        var winningMessage = winners.Count switch
        {
            1 => $"{winners[0].Name} has won the round!",
            2 => $"{winners[0].Name} and {winners[1].Name} have won the round!",
            _ => "The round is over!"
        };

        foreach (var player in allToPort)
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, winningMessage);
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
            
        }

        WinnerDeclared = true;
    }
    
    private void DeclareNoWinners()
    {
        var allToPort = Subject.GetEntities<Aisling>().ToList();

        foreach (var player in allToPort)
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, "The round ended in a draw. Everyone died!");
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
        }
        
        WinnerDeclared = true;
    }
    
    private void DeclareTeamWinner(ArenaTeam winningTeam)
    {
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
                case ArenaTeam.None:
                    player.TraverseMap(mapInstance, new Point(12, 13));
                    break;
                
                default:
                    player.TraverseMap(mapInstance, new Point(12, 13));
                    break;
            }
        }
        
        WinnerDeclared = true;
    }
    
    private bool IsHost(Aisling aisling) => aisling.Trackers.Enums.TryGetValue(out ArenaHost value)
                                            && value is ArenaHost.Host or ArenaHost.MasterHost;
}