using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine;

public class GuardianBossMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1;
    private readonly IMonsterFactory MonsterFactory;
    private readonly TimeSpan StartDelay;
    private readonly IIntervalTimer? UpdateTimer;
    private DateTime? StartTime;
    private ScriptState State;

    public GuardianBossMapScript(MapInstance subject, IMonsterFactory monsterFactory)
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
                               .Any(
                                   a => a.Trackers.Enums.TryGetValue(out MainStoryEnums stage)
                                        && stage is MainStoryEnums.StartedArtifact1
                                                    or MainStoryEnums.StartedArtifact2
                                                    or MainStoryEnums.StartedArtifact3
                                                    or MainStoryEnums.StartedArtifact4))
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
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can sense a Guardian nearby.");
                    }

                    break;

                // Spawning state
                case ScriptState.Spawning:
                {
                    if (Subject.Template.TemplateKey == "31002")
                    {
                        var monster = MonsterFactory.Create("earth_guardian", Subject, new Point(8, 6));
                        Subject.AddEntity(monster, monster);
                    }

                    if (Subject.Template.TemplateKey == "31001")
                    {
                        var monster = MonsterFactory.Create("flame_guardian", Subject, new Point(6, 8));
                        Subject.AddEntity(monster, monster);
                    }

                    if (Subject.Template.TemplateKey == "31003")
                    {
                        var monster = MonsterFactory.Create("sea_guardian", Subject, new Point(15, 8));
                        Subject.AddEntity(monster, monster);
                    }

                    if (Subject.Template.TemplateKey == "31004")
                    {
                        var monster = MonsterFactory.Create("wind_guardian", Subject, new Point(8, 6));
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
                default:
                    throw new ArgumentOutOfRangeException();
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