using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.DivineTrials
{
    public class IntelligenceChallengeMapScript : MapScriptBase
    {
        private readonly TimeSpan StartDelay = TimeSpan.FromMilliseconds(200);
        private readonly ISimpleCache SimpleCache;
        private readonly IReactorTileFactory ReactorTileFactory;
        private DateTime? StartTime;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        private readonly TimeSpan IntelligenceTimer2 = TimeSpan.FromMinutes(8);
        private readonly IIntervalTimer IntelligenceTimer;
        public const int LUCK_INTERVAL = 1;

        public const int UPDATE_INTERVAL_MS = 1;

        public IntelligenceChallengeMapScript(MapInstance subject, ISimpleCache simpleCache, IReactorTileFactory reactorTileFactory)
            : base(subject)
        {
            SimpleCache = simpleCache;
            ReactorTileFactory = reactorTileFactory;
            UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS), false);
            IntelligenceTimer = new IntervalTimer(TimeSpan.FromMinutes(LUCK_INTERVAL), false);
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;
            
            var rect = new Rectangle(
                1,
                7,
                27,
                27);

            var start = new Point(12, 33);
            var end = new Point(12, 7);

            // Generate the maze walls
            var mazeWalls = rect.GenerateMaze(start, end)
                .ToList();

            // Now use the walkablePoints to create and place reactors
            foreach (var point in mazeWalls)
            {
                var reactor = ReactorTileFactory.Create("intelligencechallengereactor", Subject, point);
                Subject.SimpleAdd(reactor);
            }

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(MainStoryEnums.StartedThirdTrial) && State == ScriptState.Dormant)
            {
                // Start the luck trial
                StartIntelligenceTrial();
            }
        }

        public void StartIntelligenceTrial()
        {
            StartTime = DateTime.UtcNow; // Set the start time to the current UTC time
            State = ScriptState.Dormant; // Set the state to DelayedStart to begin the trial
            UpdateTimer.Reset(); // Reset the update timer
            IntelligenceTimer.Reset(); // Reset the combat timer
        }

        public override void Update(TimeSpan delta)
        {
            if (!StartTime.HasValue)
                return;

            UpdateTimer.Update(delta);
            IntelligenceTimer.Update(delta);

            if (IntelligenceTimer.IntervalElapsed)
            {
                HandleIntelligenceTimeout();
                return;
            }

            if (UpdateTimer.IntervalElapsed)
            {
                UpdateTimer.Reset();
                var remainingTime = IntelligenceTimer2 - (DateTime.UtcNow - StartTime.Value);
                if (remainingTime > TimeSpan.Zero)
                {
                    NotifyRemainingTime(remainingTime);
                }

                // Switch statement to determine the current state of the script
                switch (State)
                {
                    case ScriptState.Dormant:
                        if (Subject.GetEntities<Aisling>()
                            .Any(
                                a => a.Trackers.Enums.HasValue(MainStoryEnums.StartedThirdTrial)))
                        {
                            State = ScriptState.DelayedStart;
                        }

                        break;
                    // Delayed start state
                    case ScriptState.DelayedStart:
                        // Check if the start delay has been exceeded
                        if (DateTime.UtcNow - StartTime > StartDelay)
                        {
                        }

                        break;
                    // Spawning state
                    case ScriptState.SpawningReactors:
                        // Spawn merchants for the current round
                        State = ScriptState.SpawnedReactors;
                        break;
                    // Spawned state
                    case ScriptState.SpawnedReactors:
                    {

                        if (!Subject.GetEntities<Aisling>().Any())
                        {
                            State = ScriptState.DelayedStart;
                        }
                        
                        break;
                    }

                    case ScriptState.Completed:
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(IntelligenceTrial.CompletedTrial);
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private enum ScriptState
        {
            Dormant,
            DelayedStart,
            SpawningReactors,
            SpawnedReactors,
            Completed
        }

        private void NotifyRemainingTime(TimeSpan remainingTime)
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                aisling.SendPersistentMessage($"Time left: {remainingTime.ToReadableString()}");
            }
        }

        private void HandleIntelligenceTimeout()
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                if (aisling.Trackers.Enums.HasValue(LuckTrial.CompletedTrial))
                    continue;
                
                var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
                var point = new Point(16, 16);
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("Time is up. You failed the Luck Trial.");
                aisling.Trackers.TimedEvents.AddEvent("lucktrialcd", TimeSpan.FromHours(1), true);
                aisling.Trackers.Enums.Set(LuckTrial.None);
                aisling.Trackers.Enums.Set(MainStoryEnums.StartedSecondTrial);
            }

            StartTime = null;
        }
    }
}
