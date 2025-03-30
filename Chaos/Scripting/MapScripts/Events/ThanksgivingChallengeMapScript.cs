using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
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

namespace Chaos.Scripting.MapScripts.Events;

public class ThanksgivingChallengeMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 2000;
    public const int COMBAT_INTERVAL = 30;

    private readonly List<string> BaseMonsters = new()
    {
        "tgbee_boss",
        "tgbone_collector",
        "tgbunny_boss",
        "tgcd_darkmonk",
        "tgcd_darkpriest",
        "tgcd_darkrogue",
        "tgcd_darkwarrior",
        "tgcd_darkwizard",
        "tgdark_cleric",
        "tgdraco_skeleton_monk",
        "tgdraco_skeleton_priest",
        "tgdraco_skeleton_rogue",
        "tgdraco_skeleton_warrior",
        "tgdraco_skeleton_wizard",
        "tgflesh_golem",
        "tgfomorian_horror",
        "tgfrog_boss",
        "tggargoyle_boss",
        "tggrimlock_boss",
        "tghorse_boss",
        "tgkobold_boss",
        "tgkopfloser_reiter",
        "tgmantis_boss",
        "tgmummy",
        "tgnagetier_dieter",
        "tgnm_blackshocker",
        "tgnm_blueshocker",
        "tgnm_redshocker",
        "tgnm_goldshocker",
        "tgqualgeist",
        "tgscarlet_beetle",
        "tgshambler",
        "tgshinewood_brownmantis",
        "tgshinewood_goldbeetalic",
        "tgunseelie_satyr",
        "tgwolf_boss",
        "tgzombie_boss",
        "tgor_ruidhtear",
        "tgor_ancientbeetalic",
        "tgor_ancientskeleton",
        "tgor_losgann"
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

    public ThanksgivingChallengeMapScript(MapInstance subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
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
                var mapInstance = SimpleCache.Get<MapInstance>("grassy_fields_entrance");
                var point = new Point(9, 11);
                aisling.TraverseMap(mapInstance, point);
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(25);
                aisling.StatSheet.SetManaPct(25);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                aisling.SendActiveMessage("Dealos revives you.");
                aisling.Trackers.Counters.Remove("tgwave", out _);
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
                aisling.Trackers.Counters.AddOrIncrement("tgwave");

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

        if (CurrentWave > 20) // Assuming 20 is the final wave
            return ScriptState.CompletedTrial;

        return ScriptState.SpawningWave;
    }

    private void HandleCombatTimeout()
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
        {
            var mapInstance = SimpleCache.Get<MapInstance>("grassy_fields_entrance");
            var point = new Point(9, 11);
            aisling.TraverseMap(mapInstance, point);
            aisling.SendOrangeBarMessage("Time is up. You failed the Thanksgiving challenge!");
            aisling.Trackers.Counters.Remove("tgwave", out _);
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
                var mapInstance = SimpleCache.Get<MapInstance>("grassy_fields_entrance");
                var point = new Point(9, 11);
                aisling.TraverseMap(mapInstance, point);
                aisling.SendOrangeBarMessage("Congratulations! You have completed the Thanksgiving Challenge!");
                aisling.Trackers.Counters.Remove("tgwave", out _);
                ExperienceDistributionScript.GiveExp(aisling, 50000000);
                aisling.TryGiveGamePoints(5);

                aisling.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Completed the Thanksgiving Challenge",
                        "thanksgivingchallenge",
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
            player.Trackers.Counters.Set("tgwave", CurrentWave);
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

    private Attributes ScaleMonsterStats(int wave)
        => new()
        {
            Con = 50 + wave * 5,
            Dex = 50 + wave * 5,
            Int = 50 + wave * 5,
            Str = 20 + wave * 4,
            Wis = 50 + wave * 5,
            MaximumHp = 50000 + 20000 * wave,
            MaximumMp = 20000,
            Ac = -10
        };

    private void SpawnMonstersForCurrentWave()
    {
        var monsterCount = 2 + CurrentWave; // Increase count with each wave for difficulty

        for (var i = 0; i < monsterCount; i++)
            if (Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
            {
                // Pick a random monster type from the base list
                var monsterType = BaseMonsters.PickRandom();
                var monster = MonsterFactory.Create(monsterType, Subject, point);

                // Apply scaling based on wave number
                var bonus = ScaleMonsterStats(CurrentWave);

                monster.StatSheet.AddBonus(bonus);
                monster.StatSheet.SetHealthPct(100);
                monster.StatSheet.SetManaPct(100);

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
                                 .Select(x => x.Trackers.Counters.TryGetValue("tgwave", out var wave) ? wave : 0)
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