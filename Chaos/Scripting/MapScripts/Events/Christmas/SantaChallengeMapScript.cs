using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events.Christmas;

public class SantaChallengeMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1000;
    public const int COMBAT_INTERVAL = 30;

    private readonly List<string> BaseMonsters = new()
    {
        "mtmerry_nakedsnowman50k",
        "mtmerry_radiopenguin50k",
        "mtmerry_reinwolf50k",
        "mtmerry_santawolf50k",
        "mtmerry_snowman50k",
        "mtmerry_snowwolf50k",
        "mtmerry_treegoblin50k",
        "mtmerry_dirtyerbievit"
    };

    private readonly IIntervalTimer CombatTimer;
    private readonly TimeSpan CombatTimer2 = TimeSpan.FromMinutes(30);
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly TimeSpan StartDelay = TimeSpan.FromSeconds(3);
    private readonly IIntervalTimer UpdateTimer;
    private int CurrentWave;
    private DateTime? StartTime;
    private ScriptState State;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public SantaChallengeMapScript(MapInstance subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        CurrentWave = 1;
        CombatTimer = new IntervalTimer(TimeSpan.FromMinutes(COMBAT_INTERVAL), false);
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    private bool CheckAislingDeaths()
    {
        if (Subject.GetEntities<Aisling>()
                   .All(a => a.IsDead))
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                var mapInstance = SimpleCache.Get<MapInstance>("lift_room");
                var point = new Point(4, 7);
                aisling.TraverseMap(mapInstance, point);
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(25);
                aisling.StatSheet.SetManaPct(25);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.SendActiveMessage("Elf revives you.");
                aisling.Trackers.Counters.Remove("scwave", out _);
                aisling.Refresh();
            }

            return true;
        }

        return false;
    }

    private bool CheckAllMonstersCleared()
    {
        if (Subject.GetEntities<Monster>()
                   .All(m => m.Script.Is<PetScript>()))
        {
            var aislings = Subject.GetEntities<Aisling>();

            foreach (var aisling in aislings)
                aisling.Trackers.Counters.AddOrIncrement("scwave");

            return true;
        }

        return false;
    }

    private void ClearMonsters()
    {
        foreach (var monster in Subject.GetEntities<Monster>())
            Subject.RemoveEntity(monster);
    }

    private ScriptState GetNextStateAfterWave()
    {
        HandleWaveCompleteReward();

        CurrentWave++;

        if (CurrentWave > 15) // Assuming 20 is the final wave
            return ScriptState.CompletedTrial;

        return ScriptState.SpawningWave;
    }

    private void HandleCombatTimeout()
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
        {
            var mapInstance = SimpleCache.Get<MapInstance>("lift_room");
            var point = new Point(4, 7);
            aisling.TraverseMap(mapInstance, point);
            aisling.SendOrangeBarMessage("Time is up. You failed the Santa's Challenge!");
            aisling.Trackers.Counters.Remove("scwave", out _);
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
        } else if (Subject.GetEntities<Monster>()
                          .All(m => m.Script.Is<PetScript>()))
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                var mapInstance = SimpleCache.Get<MapInstance>("lift_room");
                var point = new Point(4, 7);
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("You can feel Santa's proud! Congratulations!");
                aisling.Trackers.Counters.Remove("scwave", out _);
                ExperienceDistributionScript.GiveExp(aisling, 50000000);
                aisling.TryGiveGamePoints(5);

                aisling.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Completed Santa's Challenge",
                        "santachallenge",
                        MarkIcon.Victory,
                        MarkColor.White,
                        1,
                        GameTime.Now));
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
                           .Any())
                    State = ScriptState.DelayedStart;

                break;

            case ScriptState.DelayedStart:
                if ((DateTime.UtcNow - StartTime) > StartDelay)
                    State = ScriptState.SpawningWave;

                break;

            case ScriptState.SpawningWave:
                StartNextWave();
                State = ScriptState.SpawnedWave;

                break;

            case ScriptState.SpawnedWave:
                if (CheckAislingDeaths() || CheckAllMonstersCleared())
                    State = GetNextStateAfterWave();

                break;

            case ScriptState.CompletedTrial:
                HandleCompletedTrial();

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleWaveCompleteReward()
    {
        var playersInTrial = Subject.GetEntities<Aisling>()
                                    .Where(x => x.IsAlive)
                                    .ToList();
        var experienceToGroup = CurrentWave * 500000;
        var groupCount = playersInTrial.Count;

        if (groupCount == 0)
            groupCount = 1;

        var experience = experienceToGroup / groupCount;

        foreach (var player in playersInTrial)
        {
            ExperienceDistributionScript.GiveExp(player, experience);
            player.SendOrangeBarMessage($"Your group completed wave {CurrentWave}! {experience} gained!");
            player.Trackers.Counters.Set("scwave", CurrentWave);
        }
    }

    private void NotifyPlayers(string message)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
            aisling.SendMessage(message);
    }

    private void NotifyRemainingTime(TimeSpan remainingTime)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
            aisling.SendPersistentMessage($"Wave {CurrentWave}. Time Left: {remainingTime.ToReadableStringSimplified()}");
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        if (aisling.IsGodModeEnabled())
            return;

        if (State == ScriptState.Dormant)
            StartCombatTrial();
    }

    private void SpawnMonstersForCurrentWave()
    {
        var monsterCount = 2 + CurrentWave; // Increase count with each wave for difficulty

        for (var i = 0; i < monsterCount; i++)
            if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
            {
                // Pick a random monster type from the base list
                var monsterType = BaseMonsters.PickRandom();
                var monster = MonsterFactory.Create(monsterType, Subject, point);

                Subject.AddEntity(monster, monster);
            }
    }

    public void StartCombatTrial()
    {
        StartTime = DateTime.UtcNow;
        State = ScriptState.DelayedStart;
        UpdateTimer.Reset();
        CombatTimer.Reset();
    }

    private void StartNextWave()
    {
        var message = $"Wave {CurrentWave}: Prepare for the next challenge!";
        NotifyPlayers(message);
        SpawnMonstersForCurrentWave();
    }

    public override void Update(TimeSpan delta)
    {
        if (!StartTime.HasValue)
            return;

        UpdateTimer.Update(delta);
        CombatTimer.Update(delta);

        var highestWave = Subject.GetEntities<Aisling>()
                                 .Select(x => x.Trackers.Counters.TryGetValue("scwave", out var wave) ? wave : 0)
                                 .Where(wave => wave > CurrentWave)
                                 .DefaultIfEmpty(CurrentWave)
                                 .Max();

        // Only update CurrentWave if a higher wave was found
        if (highestWave > CurrentWave)
            CurrentWave = highestWave;

        if (CombatTimer.IntervalElapsed)
        {
            HandleCombatTimeout();

            return;
        }

        if (UpdateTimer.IntervalElapsed)
        {
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
    SpawningWave,
    SpawnedWave,
    CompletedTrial
}