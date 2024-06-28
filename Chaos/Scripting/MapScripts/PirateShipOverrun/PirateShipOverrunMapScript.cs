using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
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
        private readonly TimeSpan CombatTimer2 = TimeSpan.FromMinutes(30);
        private DateTime? StartTime;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        private readonly IIntervalTimer CombatTimer;
        public const int UPDATE_INTERVAL_MS = 1000;
        public const int COMBAT_INTERVAL = 30;

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
            if (aisling.Trackers.Enums.HasValue(PirateShip.StartedShipAttack) && State == ScriptState.Dormant)
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
                } while (!mapInstance.IsWalkable(point, aisling.Type));
                
                aisling.SendOrangeBarMessage("The Sea Monsters took over the ship and you swam to shore.");
                aisling.Trackers.TimedEvents.AddEvent("lynithpirateshipcd", TimeSpan.FromHours(8), true);
                aisling.Trackers.Enums.Set(PirateShip.None);
                aisling.StatSheet.SetHealthPct(1);
                aisling.StatSheet.SetManaPct(1);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.Refresh();
                aisling.TraverseMap(mapInstance, point);
            }

            StartTime = null;
            State = ScriptState.Dormant;
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
                    if (Subject.GetEntities<Aisling>()
                        .Any(a => a.Trackers.Enums.HasValue(PirateShip.StartedShipAttack)))
                    {
                        State = ScriptState.DelayedStart;
                    }

                    break;

                case ScriptState.DelayedStart:
                    if (DateTime.UtcNow - StartTime > StartDelay)
                    {
                        State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningFirstWave:
                    StartWave("Captain Wolfgang: They're crawling up the sides of the ship!");
                    SpawnMonsters("overrun_eel", 2);
                    SpawnMonsters("overrun_starfish", 3);
                    State = ScriptState.SpawnedFirstWave;
                    break;

                case ScriptState.SpawnedFirstWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave1) || EnoughMonstersOnShip())
                    {
                        State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningSecondWave:
                    StartWave("Captain Wolfgang: Good work crew! There's more coming though!");
                    SpawnMonsters("overrun_eel", 2);
                    SpawnMonsters("overrun_starfish", 3);
                    SpawnMonsters("overrun_greenoctopus", 2);
                    State = ScriptState.SpawnedSecondWave;
                    break;

                case ScriptState.SpawnedSecondWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave2) || EnoughMonstersOnShip())
                    {
                        State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningThirdWave:
                    StartWave("Captain Wolfgang: Keep up the good work! More incoming!");
                    SpawnMonsters("overrun_starfish", 3);
                    SpawnMonsters("overrun_greenoctopus", 4);
                    SpawnMonsters("overrun_siren", 1);
                    State = ScriptState.SpawnedThirdWave;
                    break;

                case ScriptState.SpawnedThirdWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave3) || EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningFourthWave:
                    StartWave("Captain Wolfgang: I can't believe how many you are handling.");
                    SpawnMonsters("overrun_squid", 3);
                    SpawnMonsters("overrun_siren", 2);
                    SpawnMonsters("overrun_greenoctopus", 5);
                    State = ScriptState.SpawnedFourthWave;
                    break;

                case ScriptState.SpawnedFourthWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave4) || EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningFifthWave:
                    StartWave("Captain Wolfgang: Half way through the waves! More are coming!");
                    SpawnMonsters("overrun_siren", 3);
                    SpawnMonsters("overrun_eyeboss", 1);
                    SpawnMonsters("overrun_greenoctopus", 3);
                    State = ScriptState.SpawnedFifthWave;
                    break;

                case ScriptState.SpawnedFifthWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave5) || EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningSixthWave:
                    StartWave("Captain Wolfgang: More are coming out of the water!");
                    SpawnMonsters("overrun_seamonster", 3);
                    SpawnMonsters("overrun_squid", 4);
                    SpawnMonsters("overrun_gremlin", 1);
                    State = ScriptState.SpawnedSixthWave;
                    break;

                case ScriptState.SpawnedSixthWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave6) || EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningSeventhWave:
                    StartWave("Captain Wolfgang: The Skeleton Pirates! Oh, my old crew...");
                    SpawnMonsters("overrun_gremlin", 1);
                    SpawnMonsters("overrun_blueoctopus", 5);
                    SpawnMonsters("overrun_SkeletonPirate1", 3);
                    State = ScriptState.SpawnedSeventhWave;
                    break;

                case ScriptState.SpawnedSeventhWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave7) || EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningEighthWave:
                    StartWave("Captain Wolfgang: There's more incoming!");
                    SpawnMonsters("overrun_skeletonpirate1", 5);
                    SpawnMonsters("overrun_gremlin", 2);
                    SpawnMonsters("overrun_blueoctopus", 3);
                    State = ScriptState.SpawnedEighthWave;
                    break;

                case ScriptState.SpawnedEighthWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave8) || EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningNinthWave:
                    StartWave("Captain Wolfgang: Keep them off the ship!");
                    SpawnMonsters("overrun_skeletonpirate2", 5);
                    SpawnMonsters("overrun_siren", 3);
                    SpawnMonsters("overrun_squid", 4);
                    State = ScriptState.SpawnedNinthWave;
                    break;

                case ScriptState.SpawnedNinthWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave9)|| EnoughMonstersOnShip())
                    {
                       State = GetNextStateAfterDelayedStart();
                    }

                    break;

                case ScriptState.SpawningTenthWave:
                    StartWave("Captain Wolfgang: I think this is the last of them...");
                    SpawnMonsters("overrun_eyeboss", 1);
                    SpawnMonsters("overrun_skeletonpirate2", 6);
                    SpawnMonsters("overrun_siren", 3);
                    SpawnMonsters("overrun_eel", 3);
                    State = ScriptState.SpawnedTenthWave;
                    break;

                case ScriptState.SpawnedTenthWave:
                    if (CheckAllMonstersCleared(PirateShip.FinishedWave10) || EnoughMonstersOnShip())
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
                if (Subject.Template.Bounds.TryGetRandomPoint(
                        pt => !Subject.IsWall(pt) && !IsPointInRectangle(pt, exclusionRectangle), out var point))
                {
                    var groupLevel = Subject.GetEntities<Aisling>().Where(x => !x.IsGodModeEnabled()).Select(aisling => aisling.StatSheet.Level).ToList();
                    var monster = MonsterFactory.Create(monsterType, Subject, point);

                    // Calculate the excess vitality over 20,000 for any player
                    var excessVitality = Subject.GetEntities<Aisling>()
                        .Where(aisling => !aisling.IsGodModeEnabled())
                        .Where(aisling => aisling.StatSheet.Level > 98)
                        .Select(aisling => (aisling.StatSheet.MaximumHp + aisling.StatSheet.MaximumMp) - 20000)
                        .Where(excess => excess > 0)
                        .Sum(excess => excess / 1000);

                    var attrib = new Attributes
                    {
                        AtkSpeedPct = groupLevel.Count * 5 + 2,
                        MaximumHp = (int)(monster.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 50),
                        MaximumMp = (int)(monster.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 50),
                        Int = (int)(monster.StatSheet.Int + groupLevel.Average() * groupLevel.Count / 9),
                        Str = (int)(monster.StatSheet.Str + groupLevel.Average() * groupLevel.Count / 7),
                        SkillDamagePct = (int)(monster.StatSheet.SkillDamagePct + groupLevel.Average() / 3 + groupLevel.Count + 2),
                        SpellDamagePct = (int)(monster.StatSheet.SpellDamagePct + groupLevel.Average() / 3 + groupLevel.Count + 2)
                    };

                    // Adjust the attributes based on the excess vitality
                    if (excessVitality > 0)
                    {
                        // Example adjustments: increase stats by a percentage of the excess vitality
                        attrib = attrib with
                        {
                            MaximumHp = (int)(excessVitality * 0.03)
                        }; // 5% of excess vitality added to HP
                        attrib = attrib with
                        {
                            MaximumMp = (int)(excessVitality * 0.03)
                        }; // 5% of excess vitality added to MP
                        attrib.Int += (int)(excessVitality * 0.01); // 1% of excess vitality added to Int
                        attrib.Str += (int)(excessVitality * 0.01); // 1% of excess vitality added to Str
                        attrib.SkillDamagePct +=
                            (int)(excessVitality * 0.01); // 1% of excess vitality added to Skill Damage
                        attrib.SpellDamagePct +=
                            (int)(excessVitality * 0.01); // 1% of excess vitality added to Spell Damage
                    }

                    // Add the attributes to the monster
                    monster.StatSheet.AddBonus(attrib);
                    // Add HP and MP to the monster
                    monster.StatSheet.SetHealthPct(100);
                    monster.StatSheet.SetManaPct(100);
                    // Add the monster to the subject
                    Subject.AddEntity(monster, monster);
                }
            }
        }

        private static bool IsPointInRectangle(Point point, IRectangle rect)
        {
            return point.X >= rect.Left && point.X <= rect.Right && point.Y >= rect.Top && point.Y <= rect.Bottom;
        }

        private bool EnoughMonstersOnShip()
        {
            IRectangle shipRectangle1 = new Rectangle(5, 8, 5, 8);
            IRectangle shipRectangle2 = new Rectangle(10, 9, 7, 9);
            IRectangle shipRectangle3 = new Rectangle(17, 11, 15, 7);

            var monstersOnShip = Subject.GetEntities<Monster>().Where(x => shipRectangle1.Contains(x) && shipRectangle2.Contains(x) && shipRectangle3.Contains(x)).ToList();

            if (monstersOnShip.Count > 8)
            {
                HandleCombatTimeout();
                return true;
            }

            return false;
        }
        
        private bool CheckAllMonstersCleared(PirateShip trial)
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
                    aisling.Trackers.Enums.Set(PirateShip.FinishedShipAttack);
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
                if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave1))
                {
                    return ScriptState.SpawningSecondWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave2))
                {
                    return ScriptState.SpawningThirdWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave3))
                {
                    return ScriptState.SpawningFourthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave4))
                {
                    return ScriptState.SpawningFifthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave5))
                {
                    return ScriptState.SpawningSixthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave6))
                {
                    return ScriptState.SpawningSeventhWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave7))
                {
                    return ScriptState.SpawningEighthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave8))
                {
                    return ScriptState.SpawningNinthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave9))
                {
                    return ScriptState.SpawningTenthWave;
                }
                else if (aisling.Trackers.Enums.HasValue(PirateShip.FinishedWave10))
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
