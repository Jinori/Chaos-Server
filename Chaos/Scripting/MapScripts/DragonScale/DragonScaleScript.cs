using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Site.Pages;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class DragonScaleScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly TimeSpan StartDelay;
    private DateTime? StartTime;
    private ScriptState State;
    private readonly IIntervalTimer? UpdateTimer;
    public const int UPDATE_INTERVAL_MS = 1;

    public DragonScaleScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(1);
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
    }

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
                    if (Subject.GetEntities<Aisling>()
                               .Any(
                                   a => a.Trackers.Enums.TryGetValue(out DragonScale stage)
                                        && (stage == DragonScale.DroppedScale)))
                        State = ScriptState.DelayedStart;
                }

                    break;
                // Delayed start state
                case ScriptState.DelayedStart:
                    // Set the start time if it is not already set
                    StartTime ??= DateTime.UtcNow;

                    // Check if the start delay has been exceeded
                    if (DateTime.UtcNow - StartTime > StartDelay)
                    {
                        // Reset the start time
                        StartTime = null;
                        // Set the state to spawning
                        State = ScriptState.Spawning;

                        // Get all Aislings in the subject
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                            // Send an orange bar message to the Aisling
                            aisling.Client.SendServerMessage(
                                ServerMessageType.OrangeBar1,
                                "You hear a loud roar in the distance...");
                    }

                    break;
                // Spawning state
                case ScriptState.Spawning:
                {

                    foreach (var aisling in Subject.GetEntities<Aisling>().Where(x =>
                                 x.Trackers.Enums.TryGetValue(out DragonScale stage) &&
                                 stage == DragonScale.DroppedScale))
                    {
                        aisling.Trackers.Enums.Set(DragonScale.SpawnedDragon);
                    }

                    // Create a monster
                    var monster = MonsterFactory.Create("dragonscale_boss", Subject, new Point(18, 25));

                    Subject.AddEntity(monster, monster);
                    // Set the state to spawned
                    State = ScriptState.Spawned;
                    // Reset the animation index
                }

                    break;
                // Spawned state
                case ScriptState.Spawned:
                {
                    if (!Subject.GetEntities<Aisling>().Any(x =>
                            x.Trackers.Enums.TryGetValue(out DragonScale stage) && stage == DragonScale.SpawnedDragon))
                    {
                        var monster = Subject.GetEntities<Monster>().FirstOrDefault(x => x.Name == "Dragon");

                        if (monster != null)
                        {
                            Subject.RemoveEntity(monster);
                        }
                        else
                        {
                            State = ScriptState.Dormant;
                        }
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
        Spawning,
        Spawned
    }
}