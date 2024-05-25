using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss.MainStory.TrialOfSacrifice.zoe;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.DivineTrials
{
    public class SacrificeChallengeMapScript(
        MapInstance subject,
        ISimpleCache simpleCache,
        IMonsterFactory monsterFactory)
        : MapScriptBase(subject)
    {
        private readonly IMonsterFactory MonsterFactory = monsterFactory;
        private readonly ISimpleCache SimpleCache = simpleCache;
        private readonly TimeSpan StartDelay = TimeSpan.FromSeconds(5);
        private readonly TimeSpan SacrificeTimer2 = TimeSpan.FromMinutes(20);
        private DateTime? StartTime;
        private ScriptState2 State;
        private readonly IIntervalTimer UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        private readonly IIntervalTimer CombatTimer = new IntervalTimer(TimeSpan.FromMinutes(SACRIFICE_INTERVAL), false);
        public const int UPDATE_INTERVAL_MS = 1000;
        public const int SACRIFICE_INTERVAL = 20;

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(MainStoryEnums.StartedFourthTrial) && State == ScriptState2.Dormant)
            {
                // Start the sacrifice trial
                StartSacrificeTrial();
            }
        }
        public override void Update(TimeSpan delta)
        {
            if (!StartTime.HasValue)
                return;

            UpdateTimer.Update(delta);
            CombatTimer.Update(delta);

            if (CombatTimer.IntervalElapsed)
            {
                HandleSacrificeTimeout();
                return;
            }

            if (UpdateTimer.IntervalElapsed)
            {
                UpdateTimer.Reset();
                var remainingTime = SacrificeTimer2 - (DateTime.UtcNow - StartTime.Value);
                if (remainingTime > TimeSpan.Zero)
                {
                    NotifyRemainingTime(remainingTime);
                }
                HandleScriptState();
            }
        }

        private void HandleSacrificeTimeout()
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
                var point = new Point(16, 16);
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("Time is up. You failed the Sacrifice Trial.");
                aisling.Trackers.TimedEvents.AddEvent("sacrificetrialcd", TimeSpan.FromHours(1), true);
                aisling.Trackers.Enums.Set(SacrificeTrial.None);
                aisling.Trackers.Enums.Set(MainStoryEnums.StartedFourthTrial);
            }
            StartTime = null;
        }

        private void NotifyRemainingTime(TimeSpan remainingTime)
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                aisling.SendPersistentMessage($"Time left: {remainingTime.ToReadableString()}");
            }
        }

        private void HandleScriptState()
        {
            switch (State)
            {
                case ScriptState2.Dormant:
                    if (Subject.GetEntities<Aisling>().Any(a => a.Trackers.Enums.HasValue(SacrificeTrial.StartedTrial)))
                    {
                        State = ScriptState2.DelayedStart;
                    }
                    break;

                case ScriptState2.DelayedStart:
                    if (DateTime.UtcNow - StartTime > StartDelay)
                    {
                        State = GetNextStateAfterDelayedStart();
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState2.SpawningFirstWave:
                    StartWave("Skandara: Protect Zoe from the wave of krakens.");
                    SpawnMonsters("sacrificemob1", 4);
                    State = ScriptState2.SpawnedFirstWave;
                    break;

                case ScriptState2.SpawnedFirstWave:
                    if (CheckAislingDeaths() || CheckAllMonstersCleared(SacrificeTrial.FinishedFirst))
                    {
                        State = ScriptState2.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState2.SpawningSecondWave:
                    StartWave("Skandara: Here's a wave of murauders, don't let Zoe die.");
                    SpawnMonsters("sacrificemob2", 5);
                    State = ScriptState2.SpawnedSecondWave;
                    break;

                case ScriptState2.SpawnedSecondWave:
                    if (CheckAislingDeaths() || CheckAllMonstersCleared(SacrificeTrial.FinishedSecond))
                    {
                        State = ScriptState2.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState2.SpawningThirdWave:
                    StartWave("Skandara: Zoe might not survive these Qualgeist.");
                    SpawnMonsters("sacrificemob3", 3);
                    State = ScriptState2.SpawnedThirdWave;
                    break;

                case ScriptState2.SpawnedThirdWave:
                    if (CheckAislingDeaths() || CheckAllMonstersCleared(SacrificeTrial.FinishedThird))
                    {
                        State = ScriptState2.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState2.SpawningFourthWave:
                    StartWave("Skandara: The wave of Mummies, can you save her?");
                    SpawnMonsters("sacrificemob4", 5);
                    State = ScriptState2.SpawnedFourthWave;
                    break;

                case ScriptState2.SpawnedFourthWave:
                    if (CheckAislingDeaths() || CheckAllMonstersCleared(SacrificeTrial.FinishedFourth))
                    {
                        State = ScriptState2.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState2.SpawningFifthWave:
                    StartWave("Skandara: Whoops! He wasn't suppose to appear... Watch out!");
                    SpawnMonsters(["sacrificemob1", "sacrificemob2", "sacrificemob3", "sacrificemob4"
                    ], 2);
                    var point = new Point(11, 18);
                    var sacrificeboss = MonsterFactory.Create("sacrificemobboss", Subject, point);
                    Subject.AddEntity(sacrificeboss, point);
                    sacrificeboss.Direction.Equals(Direction.Up);
                    State = ScriptState2.SpawnedFifthWave;
                    break;

                case ScriptState2.SpawnedFifthWave:
                    if (CheckAislingDeaths2())
                    {
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartWave(string message)
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                aisling.SendMessage(message);
            }
        }

        private void SpawnMonsters(string monsterType, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                {
                    var monster = MonsterFactory.Create(monsterType, Subject, point);
                    Subject.AddEntity(monster, monster);
                }
            }
        }

        private void SpawnMonsters(string[] monsterTypes, int count)
        {
            foreach (var monsterType in monsterTypes)
            {
                for (var i = 0; i < count; i++)
                {
                    if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                    {
                        var monster = MonsterFactory.Create(monsterType, Subject, point);
                        Subject.AddEntity(monster, monster);
                    }
                }
            }
        }

        private bool CheckAislingDeaths()
        {
            if (Subject.GetEntities<Aisling>().Any(a => a.IsDead))
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
                    var point = new Point(16, 16);
                    aisling.TraverseMap(mapInstance, point);
                    aisling.IsDead = false;
                    aisling.StatSheet.SetHealthPct(25);
                    aisling.StatSheet.SetManaPct(25);
                    aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    aisling.Trackers.TimedEvents.AddEvent("sacrificetrialcd", TimeSpan.FromHours(1), true);
                    aisling.SendActiveMessage("Miraelis revives you.");
                    aisling.Refresh();
                }
                return true;
            }
            return false;
        }
        
        private bool CheckAislingDeaths2()
        {
            if (Subject.GetEntities<Aisling>().Any(a => a.IsDead))
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    HandleCompletedTrial();
                }
                return true;
            }
            return false;
        }

        private bool CheckAllMonstersCleared(SacrificeTrial trial)
        {
            if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>() && !m.Script.Is<SacrificeZoe>()))
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.Set(trial);
                }
                return true;
            }
            return false;
        }

        private void HandleCompletedTrial()
        {
            if (!Subject.GetEntities<Aisling>().Any())
            {
                ClearMonsters();
                State = ScriptState2.Dormant;
            }
            else
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.Set(SacrificeTrial.FinishedTrial);
                    var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
                    var point = new Point(16, 16);
                    aisling.IsDead = false;
                    aisling.StatSheet.SetHealthPct(25);
                    aisling.StatSheet.SetManaPct(25);
                    aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    aisling.TraverseMap(mapInstance, point);
                    aisling.SendOrangeBarMessage("Congratulations! You have completed the Sacrifice Trial!");
                }
                ClearMonsters();
                State = ScriptState2.Dormant;
                StartTime = null;
            }
        }

        private void ClearMonsters()
        {
            foreach (var monster in Subject.GetEntities<Monster>())
            {
                Subject.RemoveEntity(monster);
            }
        }
        public void StartSacrificeTrial()
        {
            StartTime = DateTime.UtcNow; // Set the start time to the current UTC time
            State = ScriptState2.DelayedStart; // Set the state to DelayedStart to begin the trial
            UpdateTimer.Reset(); // Reset the update timer
            CombatTimer.Reset(); // Reset the sacrifice timer
        }
        
        private ScriptState2 GetNextStateAfterDelayedStart()
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                if (aisling.Trackers.Enums.HasValue(SacrificeTrial.FinishedFirst))
                {
                    return ScriptState2.SpawningSecondWave;
                }
                else if (aisling.Trackers.Enums.HasValue(SacrificeTrial.FinishedSecond))
                {
                    return ScriptState2.SpawningThirdWave;
                }
                else if (aisling.Trackers.Enums.HasValue(SacrificeTrial.FinishedThird))
                {
                    return ScriptState2.SpawningFourthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(SacrificeTrial.FinishedFourth))
                {
                    return ScriptState2.SpawningFifthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(SacrificeTrial.FinishedFifth))
                {
                    return ScriptState2.CompletedTrial;
                }
            }

            // If no specific trial has finished, return SpawningFirstWave as default
            return ScriptState2.SpawningFirstWave;
        }
    }

    public enum ScriptState2
    {
        Dormant,
        DelayedStart,
        SpawningFirstWave,
        SpawnedFirstWave,
        SpawningSecondWave,
        SpawnedSecondWave,
        SpawningThirdWave,
        SpawnedThirdWave,
        SpawningFourthWave,
        SpawnedFourthWave,
        SpawningFifthWave,
        SpawnedFifthWave,
        CompletedTrial
    }
}
