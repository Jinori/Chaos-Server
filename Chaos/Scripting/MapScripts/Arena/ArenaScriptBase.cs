using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

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
        private IIntervalTimer ArenaUpdateTimer { get; }
        private readonly List<IPoint> CurrentMapPoints = new();
        private readonly List<IPoint> NextMapPoints = new();
        private double TimePassedSinceMainAnimationStart;
        private double TimePassedSinceTileAnimation;
        private Animation PreAnimation { get; } = new()
        {
            AnimationSpeed = 100,
            TargetAnimation = 214 
        };
        
        
        private DateTime? AnnouncedStart;
        private bool AnnouncedTwentySecondStart;
        private bool CountdownStarted;
        private int CountdownStep;
        private DateTime LastCountdownTime;
        
        private DateTime? AnnouncedMorphStart;
        private bool AnnounceMorph;
        private bool CountdownMorphStarted;
        private int CountdownMorphStep;
        private DateTime LastCountdownMorphTime;
        
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
            ArenaUpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(2), false);
        }

        public override void Update(TimeSpan delta)
        {
            ArenaUpdateTimer.Update(delta);
            MorphTimer.Update(delta);

            if (!AnnouncedTwentySecondStart)
            {
                AnnouncedStart = DateTime.UtcNow;
                AnnouncedTwentySecondStart = true;
                
                var allPlayers = Subject.GetEntities<Aisling>().ToList();
                foreach (var player in allPlayers)
                {
                    player.SendActiveMessage("Match begins in twenty seconds!");
                }
            }

            else if (AnnouncedStart.HasValue && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalSeconds >= 10) && !CountdownStarted)
            {
                var allPlayers = Subject.GetEntities<Aisling>().ToList();
                foreach (var player in allPlayers)
                {
                    player.SendActiveMessage("Match begins in ten seconds!");
                }
                CountdownStarted = true;
                CountdownStep = 9;
                LastCountdownTime = DateTime.UtcNow;
            }
            else if (CountdownStarted && (CountdownStep >= 0))
                if (DateTime.UtcNow.Subtract(LastCountdownTime).TotalSeconds >= 1)
                {
                    var allPlayers = Subject.GetEntities<Aisling>().ToList();

                    foreach (var player in allPlayers)
                    {
                        var message = CountdownStep > 0 ? "Match begins in " + CountdownStep.ToWords() + " seconds!" : "Go! Match Start!";
                        player.SendActiveMessage(message);
                    }

                    LastCountdownTime = DateTime.UtcNow;
                    CountdownStep--;
                }

            if (ShouldMapShrink)
            {
                if (!AnnounceMorph && (MorphCount < MorphTemplateKeys.Count))
                {
                    AnnouncedMorphStart = DateTime.UtcNow;
                    AnnounceMorph = true;
                }
                else if (AnnouncedMorphStart.HasValue
                         && (DateTime.UtcNow.Subtract(AnnouncedMorphStart.Value).TotalSeconds >= 30)
                         && !CountdownMorphStarted && (MorphCount < MorphTemplateKeys.Count))
                {
                    var allPlayers = Subject.GetEntities<Aisling>().ToList();

                    foreach (var player in allPlayers)
                    {
                        player.SendActiveMessage("Lava will claim more of the map in ten seconds!");
                    }

                    CountdownMorphStarted = true;
                    CountdownMorphStep = 9;
                    LastCountdownMorphTime = DateTime.UtcNow;
                }
                else if (CountdownMorphStarted && (CountdownMorphStep > 0) && (MorphCount < MorphTemplateKeys.Count))
                {
                    if (DateTime.UtcNow.Subtract(LastCountdownMorphTime).TotalSeconds >= 1)
                    {
                        var allPlayers = Subject.GetEntities<Aisling>().ToList();

                        foreach (var player in allPlayers)
                        {
                            var message = CountdownMorphStep > 1 ? "Lava creeps in " + CountdownMorphStep.ToWords() + " seconds!" : "Lava has flowed inwards!";
                            player.SendActiveMessage(message);
                        }

                        var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
                        var currentMapTemp = SimpleCache.Get<MapTemplate>(Subject.Template.TemplateKey);
                        var nextMapTemp = SimpleCache.Get<MapTemplate>(templateKey);
                        
                        var currentSecond = (int)Math.Floor(TimePassedSinceMainAnimationStart);
                        
                        
                        for (var x = 0; x < currentMapTemp.Width; x++)
                        {
                            for (var y = 0; y < currentMapTemp.Height; y++)
                            {
                                var point = new Point(x, y);

                                if (currentMapTemp.IsWithinMap(point) && !currentMapTemp.IsWall(point) && !CurrentMapPoints.Contains(point))
                                {
                                    CurrentMapPoints.Add(point);
                                }
                            }
                        }
                        for (var x = 0; x < nextMapTemp.Width; x++)
                        {
                            for (var y = 0; y < nextMapTemp.Height; y++)
                            {
                                var point = new Point(x, y);

                                if (nextMapTemp.IsWithinMap(point) && nextMapTemp.IsWall(point) && !NextMapPoints.Contains(point))
                                {
                                    NextMapPoints.Add(point);
                                }
                            }
                        }

                        foreach (var nowall in CurrentMapPoints)
                        {
                            if (NextMapPoints.Contains(nowall))
                            {
                                if ((int)Math.Floor(TimePassedSinceMainAnimationStart) == currentSecond)
                                {
                                    Subject.ShowAnimation(PreAnimation.GetPointAnimation(nowall));
                                }
                            }
                        }

                        TimePassedSinceMainAnimationStart += delta.TotalSeconds; // Increment the main animation timer

                        TimePassedSinceTileAnimation += delta.TotalSeconds;

                        if (TimePassedSinceTileAnimation >= 1)
                        {
                            TimePassedSinceTileAnimation = 0;
                            CurrentMapPoints.Clear();
                        }

                        LastCountdownMorphTime = DateTime.UtcNow;
                        CountdownMorphStep--;
                    }
                }
                else if (CountdownMorphStarted && (CountdownMorphStep == 0) && (MorphCount < MorphTemplateKeys.Count))
                {
                    MorphMap();
                    CountdownMorphStep = 9;
                    CountdownMorphStarted = false;
                    AnnounceMorph = false;
                    CurrentMapPoints.Clear();
                    NextMapPoints.Clear();
                }
            }

            if (!ArenaUpdateTimer.IntervalElapsed)
                return;
            
            var allAislings = Subject.GetEntities<Aisling>().ToList();

            if (!allAislings.Any(x => x.IsAlive))
            {
                MorphCount = 0;
                WinnerDeclared = false;
                return;   
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

                    if (IsHostPlaying && (aliveAislings.Count == 1))
                        DeclareIndividualWinner(aliveAislings.First());
                    
                    else if ((aliveAislings.Count == 1) && (!IsHostPlaying || IsHost(aliveAislings.First())))
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


            var allHosts = Subject.GetEntities<Aisling>().Where(IsHost);
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

            foreach (var host in allHosts)
            {
                host.SendServerMessage(ServerMessageType.OrangeBar2, $"{winningTeam} has won the round!");
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                host.TraverseMap(mapInstance, new Point(12, 10));
            }
            
            Subject.RemoveScript<IMapScript, ArenaScriptBase>();
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
                player.SendServerMessage(ServerMessageType.OrangeBar2, $"{aisling.Name} has won the round!");
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                player.TraverseMap(mapInstance, new Point(12, 10));
            }

            foreach (var host in allHosts)
            {
                host.SendServerMessage(ServerMessageType.OrangeBar2, $"{aisling.Name} has won the round!");
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                host.TraverseMap(mapInstance, new Point(12, 10));
            }

            Subject.RemoveScript<IMapScript, ArenaScriptBase>();

            WinnerDeclared = true;
            ResetMaps();
        }
    }
}
