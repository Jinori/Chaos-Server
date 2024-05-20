using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Namotion.Reflection;

namespace Chaos.Scripting.MapScripts.MainStoryLine.DivineTrials
{
    public class LuckChallengeMapScript : MapScriptBase
    {
        private readonly IMerchantFactory MerchantFactory;
        private readonly TimeSpan StartDelay = TimeSpan.FromMilliseconds(200);
        private readonly ISimpleCache SimpleCache;
        private DateTime? StartTime;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        private List<Point> SpawnPoints;
        private readonly TimeSpan LuckTimer2 = TimeSpan.FromMinutes(1);
        private readonly IIntervalTimer LuckTimer;
        public const int LUCK_INTERVAL = 1;
        private int CurrentRound = 1;
        private int DoorsuccessCount = 2; // Number of door success merchants per round
        private int DoorfailCount = 1; // Number of door fail merchants per round
        private readonly Random Random = new Random();

        public const int UPDATE_INTERVAL_MS = 1;

        public LuckChallengeMapScript(MapInstance subject, IMerchantFactory merchantFactory, ISimpleCache simpleCache)
            : base(subject)
        {
            MerchantFactory = merchantFactory;
            SimpleCache = simpleCache;
            UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS), false);
            LuckTimer = new IntervalTimer(TimeSpan.FromMinutes(LUCK_INTERVAL), false);
            SpawnPoints = GenerateSpawnPoints(CurrentRound); // Generate spawn points for merchants
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(MainStoryEnums.StartedSecondTrial) && State == ScriptState.Dormant)
            {
                // Start the luck trial
                StartLuckTrial();
            }
        }

        public void StartLuckTrial()
        {
            StartTime = DateTime.UtcNow; // Set the start time to the current UTC time
            State = ScriptState.Dormant; // Set the state to DelayedStart to begin the trial
            UpdateTimer.Reset(); // Reset the update timer
            LuckTimer.Reset(); // Reset the combat timer
        }

        public override void Update(TimeSpan delta)
        {
            if (!StartTime.HasValue)
                return;

            UpdateTimer.Update(delta);
            LuckTimer.Update(delta);

            if (LuckTimer.IntervalElapsed)
            {
                HandleLuckTimeout();
                return;
            }

            if (UpdateTimer.IntervalElapsed)
            {
                UpdateTimer.Reset();
                var remainingTime = LuckTimer2 - (DateTime.UtcNow - StartTime.Value);
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
                                a => a.Trackers.Enums.HasValue(MainStoryEnums.StartedSecondTrial)))
                        {
                            State = ScriptState.DelayedStart;
                        }

                        break;
                    // Delayed start state
                    case ScriptState.DelayedStart:
                        // Check if the start delay has been exceeded
                        if (DateTime.UtcNow - StartTime > StartDelay)
                        {
                            // Set the state to spawning
                            State = GetNextStateAfterDelayedStart();
                        }

                        break;
                    // Spawning state
                    case ScriptState.Spawning1:
                        CurrentRound = 1;
                        // Spawn merchants for the current round
                        SpawnMerchants();
                        State = ScriptState.Spawned1;
                        break;
                    // Spawned state
                    case ScriptState.Spawned1:
                    {
                        // Check if there are any Aislings in the subject
                        if (!Subject.GetEntities<Aisling>().Any())
                        {
                            RemoveMerchants();
                        }

                        if (!Subject.GetEntities<Merchant>().Any(x => x.Template.TemplateKey != "trialofluckchest"))
                        {
                            CurrentRound++;
                            State = ScriptState.DelayedStart;
                        }
                        
                        break;
                    }
                    
                    case ScriptState.Spawning2:
                            DoorsuccessCount = 2;
                            DoorfailCount = 3;

                        // Spawn merchants for the current round
                        SpawnMerchants();
                        State = ScriptState.Spawned2;
                        break;
                    // Spawned state
                    case ScriptState.Spawned2:
                    {
                        // Check if there are any Aislings in the subject
                        if (!Subject.GetEntities<Aisling>().Any())
                        {
                            RemoveMerchants();
                        }

                        if (!Subject.GetEntities<Merchant>().Any(x => x.Template.TemplateKey != "trialofluckchest"))
                        {
                            CurrentRound++;
                            State = ScriptState.DelayedStart;
                        }
                        
                        break;
                    }
                    
                    case ScriptState.Spawning3:
                            DoorsuccessCount = 3;
                            DoorfailCount = 5;
                        // Spawn merchants for the current round
                        SpawnMerchants();
                        State = ScriptState.Spawned3;
                        break;
                    // Spawned state
                    case ScriptState.Spawned3:
                    {
                        // Check if there are any Aislings in the subject
                        if (!Subject.GetEntities<Aisling>().Any())
                        {
                            RemoveMerchants();
                        }

                        if (!Subject.GetEntities<Merchant>().Any(x => x.Template.TemplateKey != "trialofluckchest"))
                        {
                            CurrentRound++;
                            State = ScriptState.DelayedStart;
                        }
                        
                        break;
                    }

                    case ScriptState.Completed:
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(LuckTrial.CompletedTrial);
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
            Spawning1,
            Spawned1,
            Spawning2,
            Spawned2,
            Spawning3,
            Spawned3,
            Completed
        }

        private void NotifyRemainingTime(TimeSpan remainingTime)
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                aisling.SendPersistentMessage($"Time left: {remainingTime.ToReadableString()}");
            }
        }

        private void HandleLuckTimeout()
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

        // Method to generate spawn points for merchants
        // Method to generate spawn points for merchants based on the current round
        private List<Point> GenerateSpawnPoints(int currentRound)
        {
            List<Point> points = new List<Point>();

            // First round points
            if (currentRound == 1)
            {
                points.Add(new Point(3, 57));
                points.Add(new Point(7, 57));
                points.Add(new Point(11, 57));
            }
            // Second round points
            else if (currentRound == 2)
            {
                points.Add(new Point(2, 37));
                points.Add(new Point(5, 37));
                points.Add(new Point(8, 37));
                points.Add(new Point(11, 37));
                points.Add(new Point(14, 37));
            }
            // Third round points
            else if (currentRound == 3)
            {
                points.Add(new Point(2, 17));
                points.Add(new Point(3, 19));
                points.Add(new Point(5, 17));
                points.Add(new Point(6, 19));
                points.Add(new Point(8, 17));
                points.Add(new Point(9, 19));
                points.Add(new Point(11, 17));
                points.Add(new Point(12, 19));
            }

            return points;
        }


        // Method to spawn merchants
        // Method to spawn merchants
        // Method to spawn merchants
        private void SpawnMerchants()
        {
            // Generate spawn points for the current round
            SpawnPoints = GenerateSpawnPoints(CurrentRound);

            // Shuffle spawn points for random spawning
            Shuffle(SpawnPoints);

            // Ensure that we have enough spawn points for both door success and door fail merchants
            int totalMerchants = DoorsuccessCount + DoorfailCount;
            int maxIndex = Math.Min(totalMerchants, SpawnPoints.Count);

            // Spawn door success merchants
            for (int i = 0; i < maxIndex && i < DoorsuccessCount; i++)
            {
                var point = SpawnPoints[i];
                var doorsuccess = MerchantFactory.Create("doorsuccess", Subject, point);
                Subject.AddEntity(doorsuccess, point);
            }

            // Spawn door fail merchants
            for (int i = DoorsuccessCount; i < maxIndex && i < totalMerchants; i++)
            {
                var point = SpawnPoints[i];
                var doorfail = MerchantFactory.Create("doorfail", Subject, point);
                Subject.AddEntity(doorfail, point);
            }
        }



        // Method to shuffle a list
        private void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        private void RemoveMerchants()
        {
            foreach (var merchant in Subject.GetEntities<Merchant>())
            {
                if (merchant.Template.TemplateKey != "trialofluckchest")
                {
                    Subject.RemoveEntity(merchant);
                }
            }
        }
        
        private ScriptState GetNextStateAfterDelayedStart()
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                if (aisling.Trackers.Enums.HasValue(LuckTrial.StartedTrial) || (aisling.Trackers.Enums.HasValue(LuckTrial.BadDoor)))
                {
                    return ScriptState.Spawning1;
                }
                else if (aisling.Trackers.Enums.HasValue(LuckTrial.SucceededFirst))
                {
                    return ScriptState.Spawning2;
                }
                else if (aisling.Trackers.Enums.HasValue(LuckTrial.SucceededSecond))
                {
                    return ScriptState.Spawning3;
                }
                else if (aisling.Trackers.Enums.HasValue(LuckTrial.SucceededThird))
                {
                    return ScriptState.Completed;
                }
            }

            // If no specific trial has finished, return SpawningFirstWave as default
            return ScriptState.Spawning1;
        }
    }
}
