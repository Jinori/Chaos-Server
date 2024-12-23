﻿using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.karlopos;

public class KarloposFirstTrial(MapInstance subject, IMonsterFactory monsterFactory) : MapScriptBase(subject)
{
    private readonly TimeSpan StartDelay = TimeSpan.FromSeconds(1);
    private DateTime? StartTime;
    private ScriptState State;
    private readonly IIntervalTimer? UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
    public const int UPDATE_INTERVAL_MS = 1;

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
                                   a => a.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage)
                                        && (stage == QueenOctopusQuest.Pendant2)))
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
                                "Spores begin to surround you");
                    }

                    break;
                // Spawning state
                case ScriptState.Spawning:
                    
                    var monster1 = monsterFactory.Create("karlopos_sporet", Subject, new Point(3, 2));
                    var monster2 = monsterFactory.Create("karlopos_sporet", Subject, new Point(4, 2));
                    var monster3 = monsterFactory.Create("karlopos_sporet", Subject, new Point(5, 2));
                    var monster4 = monsterFactory.Create("karlopos_sporet", Subject, new Point(6, 2));
                    var monster5 = monsterFactory.Create("karlopos_sporet", Subject, new Point(7, 2));
                    
                    Subject.AddEntity(monster1, monster1);
                    Subject.AddEntity(monster2, monster2);
                    Subject.AddEntity(monster3, monster3);
                    Subject.AddEntity(monster4, monster4);
                    Subject.AddEntity(monster5, monster5);
                    
                        // Set the state to spawned
                        State = ScriptState.Spawned;

                    break;
                // Spawned state
                case ScriptState.Spawned:
                    // Check if there are any Aislings in the subject
                    if (!Subject.GetEntities<Aisling>().Any())
                    {
                        // Get all monsters in the subject
                        var monsters = Subject.GetEntities<Monster>().ToList();

                        // Remove all monsters from the subject
                        foreach (var monster in monsters)
                            Subject.RemoveEntity(monster);

                        // Set the state to dormant
                        State = ScriptState.Dormant;
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