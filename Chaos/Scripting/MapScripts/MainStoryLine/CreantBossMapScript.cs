using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine;

public class CreantBossMapScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly TimeSpan StartDelay;
    private DateTime? StartTime;
    public ScriptState State;
    private readonly IIntervalTimer? UpdateTimer;
    public const int UPDATE_INTERVAL_MS = 1;

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
        {
            // Switch statement to determine the current state of the script
            switch (State)
            {
                case ScriptState.Dormant:
                {
                    if (Subject.Template.TemplateKey == "31010")
                    {
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedSham)))
                            State = ScriptState.DelayedStart;
                    }

                    if (Subject.Template.TemplateKey == "19522")
                    {
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedTauren)))
                            State = ScriptState.DelayedStart;
                    }

                    if (Subject.Template.TemplateKey == "989")
                    {
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedPhoenix)))
                            State = ScriptState.DelayedStart;
                    }

                    if (Subject.Template.TemplateKey == "6599")
                    {
                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       a => a.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                            && stage is MainstoryMasterEnums.StartedCreants
                                            && a.Trackers.Flags.HasFlag(CreantEnums.StartedMedusa)))
                            State = ScriptState.DelayedStart;
                    }

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
                    if (DateTime.UtcNow - StartTime > StartDelay)
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
                        var monster = MonsterFactory.Create("ladyphoenix", Subject, new Point(36, 38));
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
                                .Any())
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
                    }
                }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ScriptState
    {
        Dormant,
        DelayedStart,
        Spawning,
        Spawned,
        CreantKilled
    }
}