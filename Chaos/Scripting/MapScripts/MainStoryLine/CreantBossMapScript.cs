﻿using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine;

public class CreantBossMapScript : MapScriptBase
{
    public enum ScriptState
    {
        Dormant,
        DelayedStart,
        Spawning,
        Spawned,
        CreantKilled
    }

    public const int UPDATE_INTERVAL_MS = 1;
    private readonly IMonsterFactory MonsterFactory;
    private readonly TimeSpan StartDelay;
    private readonly IIntervalTimer? UpdateTimer;
    private DateTime? StartTime;
    public ScriptState State;

    public CreantBossMapScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(3);
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);

        if (UpdateTimer!.IntervalElapsed)

            // Switch statement to determine the current state of the script
            switch (State)
            {
                case ScriptState.Dormant:
                {
                    if (Subject.GetEntities<Aisling>()
                               .Any(x => x.Trackers.Flags.HasFlag(CreantEnums.KilledMedusa))
                        && (Subject.Template.TemplateKey == "6599"))
                    {
                        State = ScriptState.CreantKilled;

                        return;
                    }
                    
                    if (Subject.GetEntities<Aisling>()
                               .Any(x => x.Trackers.Flags.HasFlag(CreantEnums.KilledSham))
                        && (Subject.Template.TemplateKey == "31010"))
                    {
                        State = ScriptState.CreantKilled;

                        return;
                    }
                    
                    if (Subject.GetEntities<Aisling>()
                               .Any(x => x.Trackers.Flags.HasFlag(CreantEnums.KilledPhoenix))
                        && (Subject.Template.TemplateKey == "989"))
                    {
                        State = ScriptState.CreantKilled;

                        return;
                    }
                    
                    if (Subject.GetEntities<Aisling>()
                               .Any(x => x.Trackers.Flags.HasFlag(CreantEnums.KilledTauren))
                        && (Subject.Template.TemplateKey == "19522"))
                    {
                        State = ScriptState.CreantKilled;

                        return;
                    }
                    
                    if (Subject.Template.TemplateKey == "31010")
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedSham)))
                            State = ScriptState.DelayedStart;

                    if (Subject.Template.TemplateKey == "19522")
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedTauren)))
                            State = ScriptState.DelayedStart;

                    if (Subject.Template.TemplateKey == "989")
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedPhoenix)))
                            State = ScriptState.DelayedStart;

                    if (Subject.Template.TemplateKey == "6599")
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedMedusa)))
                            State = ScriptState.DelayedStart;

                    if (Subject.GetEntities<Aisling>()
                               .Any(
                                   a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                        && stage is MainstoryMasterEnums.StartedCreants))
                        State = ScriptState.DelayedStart;
                }

                    break;

                // Delayed start state
                case ScriptState.DelayedStart:
                    // Set the start time if it is not already set
                    StartTime ??= DateTime.UtcNow;

                    // Check if the start delay has been exceeded
                    if ((DateTime.UtcNow - StartTime) > StartDelay)
                    {
                        // Reset the start time
                        StartTime = null;

                        // Set the state to spawning
                        State = ScriptState.Spawning;

                        // Get all Aislings in the subject
                        foreach (var aisling in Subject.GetEntities<Aisling>())

                            // Send an orange bar message to the Aisling
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The Creant appears...");
                    }

                    break;

                // Spawning state
                case ScriptState.Spawning:
                {
                    if (Subject.Template.TemplateKey == "31010")
                    {
                        var monster = MonsterFactory.Create("shamensyth", Subject, new Point(14, 14));
                        Subject.AddEntity(monster, monster);
                    }

                    if (Subject.Template.TemplateKey == "19522")
                    {
                        var monster = MonsterFactory.Create("tauren", Subject, new Point(9, 10));
                        Subject.AddEntity(monster, monster);
                    }

                    if (Subject.Template.TemplateKey == "989")
                    {
                        var monster = MonsterFactory.Create("phoenix", Subject, new Point(5, 5));
                        Subject.AddEntity(monster, monster);
                    }

                    if (Subject.Template.TemplateKey == "6599")
                    {
                        var monster = MonsterFactory.Create("medusa", Subject, new Point(10, 11));
                        Subject.AddEntity(monster, monster);
                    }

                    State = ScriptState.Spawned;
                }

                    break;

                // Spawned state
                case ScriptState.Spawned:
                    // Check if there are any Aislings in the subject
                    if (!Subject.GetEntities<Aisling>()
                                .Any(aisling => aisling.IsAlive))
                    {
                        // Get all monsters in the subject
                        var monsters = Subject.GetEntities<Monster>()
                                              .ToList();

                        // Remove all monsters from the subject
                        foreach (var monster in monsters)
                            Subject.RemoveEntity(monster);
                    }

                    break;

                case ScriptState.CreantKilled:
                {
                    if (Subject.GetEntities<Monster>()
                               .Any())
                    {
                        var monsters = Subject.GetEntities<Monster>()
                                              .ToList();

                        // Remove all monsters from the subject
                        foreach (var monster in monsters)
                            Subject.RemoveEntity(monster);

                        var reactors = Subject.GetEntities<ReactorTile>()
                                              .Where(x => !x.ScriptKeys.Contains("finishcreantreactor")).ToList();

                        foreach (var reactor in reactors)
                            Subject.RemoveEntity(reactor);
                    }
                }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}