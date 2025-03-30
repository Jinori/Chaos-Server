using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.DivineTrials;

public class CombatChallengeMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1000;
    public const int COMBAT_INTERVAL = 10;
    private readonly IIntervalTimer CombatTimer;
    private readonly TimeSpan CombatTimer2 = TimeSpan.FromMinutes(10);
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly TimeSpan StartDelay = TimeSpan.FromSeconds(5);
    private readonly IIntervalTimer UpdateTimer;
    private DateTime? StartTime;
    private ScriptState State;

    public CombatChallengeMapScript(MapInstance subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        CombatTimer = new IntervalTimer(TimeSpan.FromMinutes(COMBAT_INTERVAL), false);
    }

    private bool CheckAislingDeaths()
    {
        if (Subject.GetEntities<Aisling>()
                   .Any(a => a.IsDead))
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
                aisling.Trackers.TimedEvents.AddEvent("combattrialcd", TimeSpan.FromHours(1), true);
                aisling.SendActiveMessage("Miraelis revives you.");
                aisling.Refresh();
            }

            return true;
        }

        return false;
    }

    private bool CheckAllMonstersCleared(CombatTrial trial)
    {
        if (!Subject.GetEntities<Monster>()
                    .Any(m => !m.Script.Is<PetScript>()))
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
                aisling.Trackers.Enums.Set(trial);

            return true;
        }

        return false;
    }

    private void ClearMonsters()
    {
        foreach (var monster in Subject.GetEntities<Monster>())
            Subject.RemoveEntity(monster);
    }

    private ScriptState GetNextStateAfterDelayedStart()
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
        {
            if (aisling.Trackers.Enums.HasValue(CombatTrial.FinishedFirst))
                return ScriptState.SpawningSecondWave;

            if (aisling.Trackers.Enums.HasValue(CombatTrial.FinishedSecond))
                return ScriptState.SpawningThirdWave;

            if (aisling.Trackers.Enums.HasValue(CombatTrial.FinishedThird))
                return ScriptState.SpawningFourthWave;

            if (aisling.Trackers.Enums.HasValue(CombatTrial.FinishedFourth))
                return ScriptState.SpawningFifthWave;

            if (aisling.Trackers.Enums.HasValue(CombatTrial.FinishedFifth))
                return ScriptState.CompletedTrial;
        }

        // If no specific trial has finished, return SpawningFirstWave as default
        return ScriptState.SpawningFirstWave;
    }

    private ScriptState GetNextStateAfterWave()
    {
        if (Subject.GetEntities<Aisling>()
                   .All(a => a.Trackers.Enums.HasValue(CombatTrial.FinishedFirst)))
            return ScriptState.SpawningSecondWave;

        if (Subject.GetEntities<Aisling>()
                   .All(a => a.Trackers.Enums.HasValue(CombatTrial.FinishedSecond)))
            return ScriptState.SpawningThirdWave;

        if (Subject.GetEntities<Aisling>()
                   .All(a => a.Trackers.Enums.HasValue(CombatTrial.FinishedThird)))
            return ScriptState.SpawningFourthWave;

        if (Subject.GetEntities<Aisling>()
                   .All(a => a.Trackers.Enums.HasValue(CombatTrial.FinishedFourth)))
            return ScriptState.SpawningFifthWave;

        if (Subject.GetEntities<Aisling>()
                   .All(a => a.Trackers.Enums.HasValue(CombatTrial.FinishedFifth)))
            return ScriptState.CompletedTrial;

        // If no specific wave finished, remain in the current state
        return State;
    }

    private void HandleCombatTimeout()
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
        {
            var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
            var point = new Point(16, 16);
            aisling.TraverseMap(mapInstance, point);
            aisling.SendOrangeBarMessage("Time is up. You failed the Combat Trial.");
            aisling.Trackers.TimedEvents.AddEvent("combattrialcd", TimeSpan.FromHours(1), true);
            aisling.Trackers.Enums.Set(CombatTrial.None);
            aisling.Trackers.Enums.Set(MainStoryEnums.StartedFirstTrial);
        }

        StartTime = null;
        State = ScriptState.Dormant;
    }

    private void HandleCompletedTrial()
    {
        if (!Subject.GetEntities<Aisling>()
                    .Any())
        {
            ClearMonsters();
            State = ScriptState.Dormant;
        } else if (!Subject.GetEntities<Monster>()
                           .Any(m => !m.Script.Is<PetScript>()))
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                aisling.Trackers.Enums.Set(CombatTrial.FinishedTrial);
                var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
                var point = new Point(16, 16);
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("Congratulations! You have completed the Combat Trial!");
            }

            ClearMonsters();
            State = ScriptState.Dormant;
            StartTime = null;
        }
    }

    private void HandleScriptState()
    {
        switch (State)
        {
            case ScriptState.Dormant:
                if (Subject.GetEntities<Aisling>()
                           .Any(a => a.Trackers.Enums.HasValue(CombatTrial.StartedTrial)))
                    State = ScriptState.DelayedStart;

                break;

            case ScriptState.DelayedStart:
                if ((DateTime.UtcNow - StartTime) > StartDelay)
                    State = GetNextStateAfterDelayedStart();

                break;

            case ScriptState.SpawningFirstWave:
                StartWave("Miraelis: Let's see how you handle some minions.");
                SpawnMonsters("mainstory_firstwave", 6);
                State = ScriptState.SpawnedFirstWave;

                break;

            case ScriptState.SpawnedFirstWave:
                if (CheckAislingDeaths() || CheckAllMonstersCleared(CombatTrial.FinishedFirst))
                    State = GetNextStateAfterWave();

                break;

            case ScriptState.SpawningSecondWave:
                StartWave("Theselene: You won't make it through this.");
                SpawnMonsters("mainstory_secondwave", 4);
                State = ScriptState.SpawnedSecondWave;

                break;

            case ScriptState.SpawnedSecondWave:
                if (CheckAislingDeaths() || CheckAllMonstersCleared(CombatTrial.FinishedSecond))
                    State = GetNextStateAfterWave();

                break;

            case ScriptState.SpawningThirdWave:
                StartWave("Serendael: Good luck, you'll need it.");
                SpawnMonsters("mainstory_thirdwave", 3);
                State = ScriptState.SpawnedThirdWave;

                break;

            case ScriptState.SpawnedThirdWave:
                if (CheckAislingDeaths() || CheckAllMonstersCleared(CombatTrial.FinishedThird))
                    State = GetNextStateAfterWave();

                break;

            case ScriptState.SpawningFourthWave:
                StartWave("Skandara: You really are stubborn.");
                SpawnMonsters("mainstory_fourthwave", 5);
                State = ScriptState.SpawnedFourthWave;

                break;

            case ScriptState.SpawnedFourthWave:
                if (CheckAislingDeaths() || CheckAllMonstersCleared(CombatTrial.FinishedFourth))
                    State = GetNextStateAfterWave();

                break;

            case ScriptState.SpawningFifthWave:
                StartWave("Miraelis: The Gods will not hold back.");

                SpawnMonsters(
                    new[]
                    {
                        "mainstory_fifthwave1",
                        "mainstory_fifthwave2",
                        "mainstory_fifthwave3",
                        "mainstory_fifthwave4"
                    },
                    2);
                State = ScriptState.SpawnedFifthWave;

                break;

            case ScriptState.SpawnedFifthWave:
                if (CheckAislingDeaths() || CheckAllMonstersCleared(CombatTrial.FinishedFifth))
                    State = ScriptState.CompletedTrial;

                break;

            case ScriptState.CompletedTrial:
                HandleCompletedTrial();

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void NotifyRemainingTime(TimeSpan remainingTime)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
            aisling.SendPersistentMessage($"Time left: {remainingTime.ToReadableString()}");
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        if (aisling.IsGodModeEnabled())
            return;

        // Check if the player has a specific quest flag and the trial is not already in progress
        if (aisling.Trackers.Enums.HasValue(MainStoryEnums.StartedFirstTrial) && (State == ScriptState.Dormant))

            // Start the combat trial
            StartCombatTrial();
    }

    private void SpawnMonsters(string monsterType, int count)
    {
        for (var i = 0; i < count; i++)
            if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
            {
                var monster = MonsterFactory.Create(monsterType, Subject, point);
                Subject.AddEntity(monster, monster);
            }
    }

    private void SpawnMonsters(string[] monsterTypes, int count)
    {
        foreach (var monsterType in monsterTypes)
            for (var i = 0; i < count; i++)
                if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                {
                    var monster = MonsterFactory.Create(monsterType, Subject, point);
                    Subject.AddEntity(monster, monster);
                }
    }

    public void StartCombatTrial()
    {
        StartTime = DateTime.UtcNow; // Set the start time to the current UTC time
        State = ScriptState.DelayedStart; // Set the state to DelayedStart to begin the trial
        UpdateTimer.Reset(); // Reset the update timer
        CombatTimer.Reset(); // Reset the combat timer
    }

    private void StartWave(string message)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
            aisling.SendMessage(message);
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
                NotifyRemainingTime(remainingTime);
            HandleScriptState();
        }
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
    CompletedTrial
}