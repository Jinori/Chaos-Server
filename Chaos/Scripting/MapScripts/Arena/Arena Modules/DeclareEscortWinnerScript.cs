using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class DeclareEscortWinnerScript : MapScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private IIntervalTimer DeclareWinnerTimer { get; }
    private readonly IIntervalTimer GameDurationTimer;
    private readonly Point GoldPoint = new(15, 15);
    private readonly Point GreenPoint = new(16, 6);
    private readonly Point RedPoint = new(6, 6);
    private readonly Point BluePoint = new(6, 15);
    private bool WinnerDeclared;

    /// <inheritdoc />
    public DeclareEscortWinnerScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        DeclareWinnerTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
        GameDurationTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(10),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "Match will conclude in {Time}.",
            SendMessage);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        GameDurationTimer.Update(delta);
        DeclareWinnerTimer.Update(delta);
        
        if (!Subject.GetEntities<Aisling>().Any() && WinnerDeclared)
        {
            Subject.Destroy();
            return;
        }
        
        if (!DeclareWinnerTimer.IntervalElapsed)
            return;

        var nathra = Subject.GetEntities<Merchant>().FirstOrDefault(obj => obj.Template.TemplateKey.EqualsI("nathra"));

        if (nathra != null)
        {
            var currentPoint = new Point(nathra.X, nathra.Y);

            //Successful offense
            if (currentPoint == new Point(39, 6))
                HandleWinningTeam(ArenaSide.Offensive);
        }
        
        if (!GameDurationTimer.IntervalElapsed)
            return;
        
        //Successful defense
        HandleWinningTeam(ArenaSide.Defender);
    }
    
    private void SendMessage(string message)
    {
        foreach (var player in Subject.GetEntities<Aisling>())
            player.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }
    
    private void HandleWinningTeam(ArenaSide winningTeam)
    {
        var playersThatWon = Subject.GetEntities<Aisling>()
                                    .Where(x => x.Trackers.Enums.TryGetValue(out ArenaSide value) && (value == winningTeam));

        foreach (var winner in playersThatWon)
        {
            winner.SendActiveMessage($"Your team has won the round! Go {winningTeam}!");
            winner.Trackers.Enums.Remove<ArenaSide>();
        }

        var playersThatLost = Subject.GetEntities<Aisling>()
                                     .Where(x => x.Trackers.Enums.TryGetValue(out ArenaSide value) && (value != winningTeam));

        foreach (var loser in playersThatLost)
        {
            loser.SendActiveMessage($"{winningTeam} has won the round. Better luck next time.");
            loser.Trackers.Enums.Remove<ArenaSide>();
        }
        
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

        WinnerDeclared = true;
    }
}