using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena
{
    public abstract class ArenaScriptBase : MapScriptBase
    {
        private readonly Point GreenPoint = new(16, 6);
        private readonly Point RedPoint = new(6, 6);
        private readonly Point BluePoint = new(6, 15);
        private readonly Point GoldPoint = new(15, 15);
        private int MorphCount;
        private bool WinnerDeclared;
        private IIntervalTimer MorphTimer { get; }
        
        public abstract List<string> MorphTemplateKeys { get; set; }
        public abstract string MorphOriginalTemplateKey { get; set; }
        public abstract bool TeamGame { get; set; }
        public abstract bool IsHostPlaying { get; set; }
        public abstract bool ShouldMapShrink { get; set; }


        private readonly ISimpleCache SimpleCache;

        protected ArenaScriptBase(MapInstance subject, ISimpleCache simpleCache)
            : base(subject)
        {
            SimpleCache = simpleCache;
            MorphTimer = new IntervalTimer(TimeSpan.FromSeconds(30), false);
        }

        public override void Update(TimeSpan delta)
        {
            var allAislings = Subject.GetEntities<Aisling>().ToList();

            if (!allAislings.Any(x => x.IsAlive))
            {
                MorphCount = 0;
                WinnerDeclared = false;
                return;   
            }

            if (ShouldMapShrink)
            {
                MorphTimer.Update(delta);

                if (MorphTimer.IntervalElapsed && (MorphCount < MorphTemplateKeys.Count))
                {
                    MorphMap();
                }
            }

            if (TeamGame)
            {
                if (!WinnerDeclared)
                {
                    var activeTeams = GetActiveTeams(allAislings);
                
                    // If there is only one active team left, and the host is not playing, declare them as the winner.
                    if ((activeTeams.Count == 1) && (!IsHostPlaying || (activeTeams[0] != ArenaTeam.None)))
                    {
                        DeclareTeamWinner(activeTeams.First());
                    }   
                }
            }
            else
            {
                if (!WinnerDeclared)
                {
                    var aliveAislings = GetAliveAislings(allAislings);

                    // If there is only one alive player left, and the host is not playing, declare them as the winner.
                    if ((aliveAislings.Count == 1) && (!IsHostPlaying || IsHost(aliveAislings.First())))
                    {
                        DeclareIndividualWinner(aliveAislings.First());
                    }                    
                }
            }
        }

        private void MorphMap()
        {
            var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
            Subject.Morph(templateKey);
            MorphCount++;
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

        private List<Aisling> GetAliveAislings(IEnumerable<Aisling> allAislings)
        {
            List<Aisling> aliveAislings;
            
            if (IsHostPlaying)
            {
                aliveAislings = allAislings
                                    .Where(x => x.IsAlive)
                                    .ToList();

                return aliveAislings;
            }
            
            aliveAislings = allAislings
                            .Where(x => x.IsAlive && (!x.Trackers.Enums.TryGetValue(out ArenaHost value) || value == ArenaHost.None))
                            .ToList();


            return aliveAislings;
        }

        private bool IsHost(Aisling aisling) =>
            !aisling.Trackers.Enums.TryGetValue(out ArenaHost value) ||
            ((value != ArenaHost.Host) && (value != ArenaHost.MasterHost));

        private void ResetMaps()
        {
            if (MorphOriginalTemplateKey == "26006")
            {
                var arenalava = SimpleCache.Get<MapInstance>("arena_lava");
                arenalava.Morph("26006");
                var arenalava2 = SimpleCache.Get<MapInstance>("arena_lava_2");
                arenalava2.Morph("26007");
                var arenalava3 = SimpleCache.Get<MapInstance>("arena_lava_3");
                arenalava3.Morph("26008");
                var arenalava4 = SimpleCache.Get<MapInstance>("arena_lava_4");
                arenalava4.Morph("26009");   
            }
        }

        private void DeclareTeamWinner(ArenaTeam winningTeam)
        {
            var playersThatWon = Subject.GetEntities<Aisling>()
                                        .Where(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value == winningTeam));

            foreach (var winner in playersThatWon)
            {
                winner.SendActiveMessage($"Your team has won the round! Go {winningTeam}!");
            }

            var playersThatLost = Subject.GetEntities<Aisling>()
                                         .Where(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value != winningTeam));

            foreach (var loser in playersThatLost)
            {
                loser.SendActiveMessage($"{winningTeam} has won the round. Better luck next time.");
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

            foreach (var enabledScript in Subject.ScriptKeys)
            {
                Subject.ScriptKeys.Remove(enabledScript);
            }

            WinnerDeclared = true;
            ResetMaps();
        }

        private void DeclareIndividualWinner(Aisling aisling)
        {
            aisling.SendActiveMessage("You won the round!");

            var allToPort = Subject.GetEntities<Aisling>().ToList();
            var allHosts = Subject.GetEntities<Aisling>().Where(IsHost);

            foreach (var player in allToPort)
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                player.TraverseMap(mapInstance, new Point(12, 10));
            }

            foreach (var host in allHosts)
            {
                host.SendServerMessage(ServerMessageType.OrangeBar2, $"{aisling.Name} has won the round!");
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                host.TraverseMap(mapInstance, new Point(12, 10));
            }

            foreach (var enabledScript in Subject.ScriptKeys)
            {
                Subject.ScriptKeys.Remove(enabledScript);
            }

            WinnerDeclared = true;
            ResetMaps();
        }
    }
}
