using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modes
{
    /// <summary>
    /// Script to manage the pit fight arena mode.
    /// Handles player victories, losses, and missing players.
    /// </summary>
    public class PitFightScript : MapScriptBase
    {
        private readonly IIntervalTimer CheckPlayersTimer;
        private readonly IIntervalTimer CheckOnMissingPlayersTimer = new IntervalTimer(TimeSpan.FromSeconds(8));
        private readonly ISimpleCache SimpleCache;
        private readonly IStorage<PitFightLeaderboardScript.PitFightLeaderboardObj> LeaderboardStorage;
        private readonly ISequentialTimer SequenceTimer;
        private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

        private bool MissingPlayer;
        private Aisling PlayerStillActive = null!;
        
        private bool NormalPlayers => SequenceTimer.CurrentTimer == CheckPlayersTimer;

        private bool AbnormalPlayers => SequenceTimer.CurrentTimer == CheckOnMissingPlayersTimer;

        public PitFightScript(
            MapInstance subject,
            ISimpleCache simpleCache,
            IStorage<PitFightLeaderboardScript.PitFightLeaderboardObj> leaderboardStorage, IClientRegistry<IChaosWorldClient> clientRegistry)
            : base(subject)
        {
            ClientRegistry = clientRegistry;
            LeaderboardStorage = leaderboardStorage;
            SimpleCache = simpleCache;
            CheckPlayersTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
            SequenceTimer = new SequentialEventTimer(CheckPlayersTimer, CheckOnMissingPlayersTimer);
        }

        /// <summary>
        /// Records a victory for the specified player.
        /// </summary>
        private void RecordVictory(Aisling player)
        {
            var leaderboard = LeaderboardStorage.Value;
            leaderboard.AddVictory(player.Name);
        }

        /// <summary>
        /// Records a loss for the specified player.
        /// </summary>
        private void RecordLoss(Aisling player)
        {
            var leaderboard = LeaderboardStorage.Value;
            leaderboard.AddLoss(player.Name);
        }
        
        public override void Update(TimeSpan delta)
        {
            SequenceTimer.Update(delta);

            if (NormalPlayers)
            {
                UpdatePlayerCheck(delta);
            }
            else if (AbnormalPlayers)
            {
                HandleMissingPlayerCheck(delta);
            }
        }

        private void UpdatePlayerCheck(TimeSpan delta)
        {
            CheckPlayersTimer.Update(delta);

            if (!CheckPlayersTimer.IntervalElapsed)
                return;

            var players = Subject.GetEntities<Aisling>().ToList();

            switch (players.Count)
            {
                case 2:
                    HandleTwoPlayers(players);
                    break;
                case 1:
                    HandleSinglePlayer(players.First());
                    break;
            }
        }

        private void HandleMissingPlayerCheck(TimeSpan delta)
        {
            CheckOnMissingPlayersTimer.Update(delta);

            if (!CheckOnMissingPlayersTimer.IntervalElapsed || !MissingPlayer)
                return;

            MovePlayerOut(PlayerStillActive);
            MissingPlayer = false;
            PlayerStillActive = null!;
        }

        private void HandleTwoPlayers(List<Aisling> players)
        {
            MissingPlayer = false;

            if (players.All(x => x.IsAlive))
                return;

            var alivePlayer = players.FirstOrDefault(x => x.IsAlive);
            var deadPlayer = players.FirstOrDefault(x => !x.IsAlive);

            if (alivePlayer != null && deadPlayer != null)
            {
                AwardWin(alivePlayer, deadPlayer);
                AnnounceLoser(deadPlayer);
                return;
            }

            if (players.All(x => !x.IsAlive))
                RevivePlayers(players);
        }

        private void HandleSinglePlayer(Aisling player)
        {
            player.SendMessage("No opponent detected. Waiting a few moments for them to return...");
            PlayerStillActive = player;
            MissingPlayer = true;
        }

        private void AnnounceLoser(Aisling loser)
        {
            RecordLoss(loser);
            loser.SendMessage("You lost the pit fight. Better luck next time!");
            MovePlayerToEntrance(loser, new Point(6, 10));
            RevivePlayer(loser);
        }

        private void AwardWin(Aisling winner, Aisling loser)
        {
            RecordVictory(winner);

            winner.Legend.AddOrAccumulate(
                new LegendMark(
                    "Pit Fight Victory",
                    "pitfightwin",
                    MarkIcon.Victory,
                    MarkColor.LightGreen,
                    1,
                    GameTime.Now));

            winner.SendMessage("Congratulations! You have won the pit fight!");
            
            foreach (var client in ClientRegistry)
                client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    $"{loser.Name} was humiliated by {winner.Name} in a Pit Fight!");
            
            MovePlayerToEntrance(winner, new Point(6, 9));
            RevivePlayer(winner);
        }

        private void RevivePlayers(List<Aisling> players)
        {
            var revivePositions = new[]
            {
                new Point(3, 18),
                new Point(17, 3)
            };

            for (var i = 0; i < players.Count; i++)
            {
                RevivePlayer(players[i]);
                players[i].TraverseMap(Subject, revivePositions[i]);
                players[i].SendMessage("You have been revived and repositioned to continue fighting!");
            }
        }

        private static void RevivePlayer(Aisling player)
        {
            if (player.IsAlive)
                return;

            player.IsDead = false;
            player.StatSheet.SetHealthPct(100);
            player.StatSheet.SetManaPct(100);
            player.Client.SendAttributes(StatUpdateType.Vitality);
            player.Refresh();
            player.Display();
        }

        private void MovePlayerOut(Aisling player)
        {
            player.SendMessage("No opponents remain. You are being moved out.");
            MovePlayerToEntrance(player, new Point(6, 9));
        }

        private void MovePlayerToEntrance(Aisling player, Point position)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("arena_entrance");
            player.TraverseMap(mapInstance, position);
        }
    }
}
