using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.PietWerewolf;

public class PietWerewolfEmptyRoomMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1000;
    private readonly IIntervalTimer DelayedStartTimer;
    private readonly IEffectFactory EffectFactory;
    private readonly IMerchantFactory MerchantFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer UpdateTimer;
    private ScriptState State;

    public PietWerewolfEmptyRoomMapScript(
        MapInstance subject,
        IMonsterFactory monsterFactory,
        ISimpleCache simpleCache,
        IMerchantFactory merchantFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        MerchantFactory = merchantFactory;
        DelayedStartTimer = new IntervalTimer(TimeSpan.FromSeconds(20), false);
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
    }

    private bool CheckAllMonstersCleared()
    {
        if (!Subject.GetEntities<Monster>()
                    .Any(m => !m.Script.Is<PetScript>()))
        {
            State = ScriptState.CompletedWerewolf;

            return true;
        }

        return false;
    }

    private void ClearMonsters()
    {
        foreach (var monster in Subject.GetEntities<Monster>())
            Subject.RemoveEntity(monster);
    }

    private void HandleCompletedWerewolf()
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
                var effect = EffectFactory.Create("Werewolf");
                aisling.Effects.Apply(aisling, effect);
                aisling.Trackers.Enums.Set(WerewolfOfPiet.KilledandGotCursed);
                aisling.SendOrangeBarMessage("You defeated the Werewolf but his curse lingers on...");
                var mapinstance = SimpleCache.Get<MapInstance>("piet_empty_room");
                var point = new Point(aisling.X, aisling.Y);
                aisling.TraverseMap(mapinstance, point);
            }

            State = ScriptState.Dormant;
        }
    }

    private void HandleScriptState()
    {
        switch (State)
        {
            case ScriptState.Dormant:
                if (Subject.GetEntities<Aisling>()
                           .Any(a => a.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant)))
                    State = ScriptState.DelayedStart;

                break;

            case ScriptState.DelayedStart:
                if (DelayedStartTimer.IntervalElapsed)
                    State = ScriptState.SpawningWerewolf;

                break;

            case ScriptState.SpawningWerewolf:
                if (Subject.GetEntities<Monster>()
                           .Any(x => x.Name == "Werewolf"))
                    State = ScriptState.SpawnedWerewolf;

                break;

            case ScriptState.SpawnedWerewolf:
                if (CheckAllMonstersCleared()
                    && Subject.GetEntities<Aisling>()
                              .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed)))
                    State = ScriptState.CompletedWerewolf;

                break;

            case ScriptState.CompletedWerewolf:
                HandleCompletedWerewolf();

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        // Check if the player has a specific quest flag and the trial is not already in progress
        if (aisling.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant) && (State == ScriptState.Dormant))
        {
            // Start the combat trial
            var point = new Point(10, 7);
            var werewolftoby = MerchantFactory.Create("werewolfTobyInHouse", Subject, point);
            Subject.AddEntity(werewolftoby, point);
            StartTimers();
            State = ScriptState.DelayedStart;
        }
    }

    public void StartTimers()
    {
        State = ScriptState.DelayedStart; // Set the state to DelayedStart to begin the trial
        UpdateTimer.Reset(); // Reset the update timer
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);
        DelayedStartTimer.Update(delta);

        if (UpdateTimer.IntervalElapsed)
        {
            if (!Subject.GetEntities<Aisling>()
                        .Any())
                return;

            HandleScriptState();
        }
    }
}

public enum ScriptState
{
    Dormant,
    DelayedStart,
    SpawningWerewolf,
    SpawnedWerewolf,
    CompletedWerewolf
}