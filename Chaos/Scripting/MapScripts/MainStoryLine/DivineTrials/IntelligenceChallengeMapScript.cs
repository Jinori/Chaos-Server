using System.Diagnostics;
using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
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
        public const int LUCK_INTERVAL = 8;

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

            if (!Subject.GetEntities<Aisling>().Any(x => x.IsGodModeEnabled()))
            {
                var rect = new Rectangle(
                    1,
                    6,
                    17,
                    17);

                var start = new Point(9, 22);
                var end = new Point(9, 6);

                // Generate the maze walls
                var mazeWalls = rect.GenerateMaze(start, end)
                    .ToList();

                // Now use the walkablePoints to create and place reactors
                foreach (var point in mazeWalls)
                {
                    var reactor = ReactorTileFactory.Create("intelligencechallengereactor", Subject, point);
                    Subject.SimpleAdd(reactor);
                }
            }

            if (Subject.GetEntities<Aisling>().Any())
            {
                aisling.Trackers.Counters.Set("trialofintelligencelives", 5);
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
            }
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
                if (aisling.Trackers.Enums.HasValue(IntelligenceTrial.CompletedTrial))
                    continue;
                
                var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
                var point = new Point(16, 16);
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("Time is up. You failed the Intelligence Trial.");
                aisling.Trackers.TimedEvents.AddEvent("intelligencetrialcd", TimeSpan.FromHours(1), true);
                aisling.Trackers.Enums.Set(IntelligenceTrial.None);
                aisling.Trackers.Enums.Set(MainStoryEnums.StartedThirdTrial);
                aisling.Trackers.Counters.Remove("trialofintelligencelives", out _);
            }

            StartTime = null;
        }
    }
}
