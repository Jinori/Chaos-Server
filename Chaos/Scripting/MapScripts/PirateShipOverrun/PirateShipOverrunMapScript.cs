using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.PirateShipOverrun
{
    public class PirateShipOverrunMapScript : MapScriptBase
    {
        private readonly IMonsterFactory MonsterFactory;
        private readonly ISimpleCache SimpleCache;
        private readonly TimeSpan StartDelay = TimeSpan.FromSeconds(5);
        private readonly TimeSpan CombatTimer2 = TimeSpan.FromMinutes(20);
        private DateTime? StartTime;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        private readonly IIntervalTimer CombatTimer;
        public const int UPDATE_INTERVAL_MS = 1000;
        public const int COMBAT_INTERVAL = 20;

        public PirateShipOverrunMapScript(MapInstance subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
            : base(subject)
        {
            MonsterFactory = monsterFactory;
            SimpleCache = simpleCache;
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
            CombatTimer = new IntervalTimer(TimeSpan.FromMinutes(COMBAT_INTERVAL), false);
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(ShipAttack.StartedShipAttack) && State == ScriptState.Dormant)
            {
                // Start the combat trial
                StartCombatTrial();
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
                HandleCombatTimeout();
                return;
            }

            if (UpdateTimer.IntervalElapsed)
            {
                UpdateTimer.Reset();
                var remainingTime = CombatTimer2 - (DateTime.UtcNow - StartTime.Value);
                if (remainingTime > TimeSpan.Zero)
                {
                    NotifyRemainingTime(remainingTime);
                }
                HandleScriptState();
            }
        }

        private void HandleCombatTimeout()
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                var mapInstance = SimpleCache.Get<MapInstance>("lynith_beach_south");
                var rect = new Rectangle(30, 11, 5, 5);
                Point point;
                do
                {
                    point = rect.GetRandomPoint();
                }
                while (!mapInstance.IsWalkable(point, aisling.Type));
                
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("The Sea Monsters took over the ship and you swam to shore.");
                aisling.Trackers.TimedEvents.AddEvent("lynithpirateshipcd", TimeSpan.FromHours(8), true);
                aisling.Trackers.Enums.Set(ShipAttack.None);
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
                case ScriptState.Dormant:
                    if (Subject.GetEntities<Aisling>().Any(a => a.Trackers.Enums.HasValue(ShipAttack.StartedShipAttack)))
                    {
                        State = ScriptState.DelayedStart;
                    }
                    break;

                case ScriptState.DelayedStart:
                    if (DateTime.UtcNow - StartTime > StartDelay)
                    {
                        State = GetNextStateAfterDelayedStart();
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState.SpawningFirstWave:
                    StartWave("Captain Wolfgang: They're crawling up the sides of the ship!");
                    SpawnMonsters("lynith_sea_monster1", 2);
                    State = ScriptState.SpawnedFirstWave;
                    break;

                case ScriptState.SpawnedFirstWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave1))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState.SpawningSecondWave:
                    StartWave("Captain Wolfgang: Good work crew! There's more coming though!");
                    SpawnMonsters("lynith_sea_monster2", 3);
                    State = ScriptState.SpawnedSecondWave;
                    break;

                case ScriptState.SpawnedSecondWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave2))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState.SpawningThirdWave:
                    StartWave("Captain Wolfgang: Keep up the good work! More incoming!");
                    SpawnMonsters("lynith_sea_monster3", 4);
                    State = ScriptState.SpawnedThirdWave;
                    break;

                case ScriptState.SpawnedThirdWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave3))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState.SpawningFourthWave:
                    StartWave("Captain Wolfgang: I can't believe how many you are handling. Watch out!");
                    SpawnMonsters("lynith_sea_boss1", 2 );
                    State = ScriptState.SpawnedFourthWave;
                    break;

                case ScriptState.SpawnedFourthWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave4))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;

                case ScriptState.SpawningFifthWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters(["lynith_sea_monster5"
                    ], 7);
                    State = ScriptState.SpawnedFifthWave;
                    break;

                case ScriptState.SpawnedFifthWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave5))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;
                
                case ScriptState.SpawningSixthWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters(["lynith_sea_monster5"
                    ], 7);
                    State = ScriptState.SpawnedSixthWave;
                    break;

                case ScriptState.SpawnedSixthWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave6))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;
                
                case ScriptState.SpawningSeventhWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters(["lynith_sea_monster5"
                    ], 7);
                    State = ScriptState.SpawnedSeventhWave;
                    break;

                case ScriptState.SpawnedSeventhWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave7))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;
                
                case ScriptState.SpawningEighthWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters(["lynith_sea_monster5"
                    ], 7);
                    State = ScriptState.SpawnedEighthWave;
                    break;

                case ScriptState.SpawnedEighthWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave8))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;
                
                case ScriptState.SpawningNinthWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters(["lynith_sea_monster5"
                    ], 7);
                    State = ScriptState.SpawnedNinthWave;
                    break;

                case ScriptState.SpawnedNinthWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave9))
                    {
                        State = ScriptState.DelayedStart;
                        StartTime = DateTime.UtcNow;
                    }
                    break;
                
                case ScriptState.SpawningTenthWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters(["lynith_sea_monster5"
                    ], 7);
                    State = ScriptState.SpawnedTenthWave;
                    break;

                case ScriptState.SpawnedTenthWave:
                    if (CheckAllMonstersCleared(ShipAttack.FinishedWave10))
                    {
                        State = ScriptState.CompletedTrial;
                    }
                    break;

                case ScriptState.CompletedTrial:
                    HandleCompletedTrial();
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
            IRectangle exclusionRectangle = new Rectangle(4, 6, 29, 13);
            
            for (var i = 0; i < count; i++)
            {
                if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt) && !IsPointInRectangle(pt, exclusionRectangle), out var point))
                {
                    var monster = MonsterFactory.Create(monsterType, Subject, point);
                    Subject.AddEntity(monster, monster);
                }
            }
        }

        private static bool IsPointInRectangle(Point point, IRectangle rect1)
        {
            return point.X >= rect1.Left && point.X <= rect1.Right && point.Y >= rect1.Top && point.Y <= rect1.Bottom;
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

        private bool CheckAllMonstersCleared(ShipAttack trial)
        {
            if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>()))
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
                State = ScriptState.Dormant;
            }
            else if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>()))
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    aisling.Trackers.Enums.Set(ShipAttack.FinishedShipAttack);
                    var mapInstance = SimpleCache.Get<MapInstance>("lynith_pirate_ship_deck");
                    aisling.TraverseMap(mapInstance, aisling);
                    aisling.SendOrangeBarMessage("You defended the ship from the Sea Monsters! Speak to the Captain.");
                }
                ClearMonsters();
                State = ScriptState.Dormant;
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
        public void StartCombatTrial()
        {
            StartTime = DateTime.UtcNow; // Set the start time to the current UTC time
            State = ScriptState.DelayedStart; // Set the state to DelayedStart to begin the trial
            UpdateTimer.Reset(); // Reset the update timer
            CombatTimer.Reset(); // Reset the combat timer
        }
        
        private ScriptState GetNextStateAfterDelayedStart()
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave1))
                {
                    return ScriptState.SpawningSecondWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave2))
                {
                    return ScriptState.SpawningThirdWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave3))
                {
                    return ScriptState.SpawningFourthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave4))
                {
                    return ScriptState.SpawningFifthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave5))
                {
                    return ScriptState.SpawningSixthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave6))
                {
                    return ScriptState.SpawningSeventhWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave7))
                {
                    return ScriptState.SpawningEighthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave8))
                {
                    return ScriptState.SpawningNinthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave9))
                {
                    return ScriptState.SpawningTenthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(ShipAttack.FinishedWave10))
                {
                    return ScriptState.CompletedTrial;
                }
                
            }

            // If no specific trial has finished, return SpawningFirstWave as default
            return ScriptState.SpawningFirstWave;
        }
    }

    public enum ScriptState
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
        SpawningSixthWave,
        SpawnedSixthWave,
        SpawningSeventhWave,
        SpawnedSeventhWave,
        SpawningEighthWave,
        SpawnedEighthWave,
        SpawningNinthWave,
        SpawnedNinthWave,
        SpawningTenthWave,
        SpawnedTenthWave,
        CompletedTrial
    }
}
