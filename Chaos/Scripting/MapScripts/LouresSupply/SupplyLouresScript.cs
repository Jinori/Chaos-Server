using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.LouresSupply;

public class SupplyLouresScript(MapInstance subject, ISimpleCache simpleCache)
    : MapScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;
    private readonly TimeSpan StartDelay = TimeSpan.FromSeconds(1);
    private DateTime? StartTime;
    private ScriptState State;
    private readonly IIntervalTimer? UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
    private const int UPDATE_INTERVAL_MS = 1;

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);

        if (UpdateTimer!.IntervalElapsed)
        {
            
            // Switch statement to determine the current state of the script
            switch (State)
            {
                case ScriptState.Dormant:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>().Where(x =>
                                 x.Trackers.Enums.HasValue(SickChildStage.SickChildKilled) && x.Trackers.Enums.HasValue(SupplyLouresStage.StartedQuest) || x.Trackers.Enums.HasValue(SupplyLouresStage.SawAssassin) && x is { IsAlive: true, UserStatSheet.Level: > 40 }))
                    {
                        var mapInstance = SimpleCache.Get<MapInstance>("loures_training_camp1");

                        aisling.TraverseMap(mapInstance, aisling);
                    }

                }

                    break;
                // Delayed start state
                case ScriptState.DelayedStart:
                    // Set the start time if it is not already set
                    StartTime ??= DateTime.UtcNow;

                    // Check if the start delay has been exceeded
                    if (DateTime.UtcNow - StartTime > StartDelay)
                    {
                    }

                    break;
                // Spawning state
                case ScriptState.Spawning:
                {
                }

                    break;
                // Spawned state
                case ScriptState.Spawned:
                {
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
        Spawning,
        Spawned
    }
}