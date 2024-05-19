using System.Reactive.Subjects;
using System.Runtime.InteropServices.JavaScript;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.DivineTrials;

public class CombatChallengeMapScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly TimeSpan StartDelay;
    private DateTime? StartTime;
    private ScriptState State;
    private readonly IIntervalTimer? UpdateTimer;
    public const int UPDATE_INTERVAL_MS = 200;

    public CombatChallengeMapScript(MapInstance subject, IMonsterFactory monsterFactory, IIntervalTimer animationInterval, IRectangle animationShape, List<Point> reverseOutline, List<Point> shapeOutline)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(5);
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS));
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
                            a => a.Trackers.Enums.HasValue(CombatTrial.StartedTrial)))
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

                        if (Subject.GetEntities<Aisling>()
                            .Any(x => x.Trackers.Enums.HasValue(CombatTrial.StartedTrial)))
                        {
                            State = ScriptState.SpawningFirstWave;
                            // Get all Aislings in the subject
                            foreach (var aisling in Subject.GetEntities<Aisling>())
                                // Send an orange bar message to the Aisling
                                aisling.SendMessage("Miraelis: Let's see how you handle some minions.");
                        }

                        if (Subject.GetEntities<Aisling>()
                            .Any(x => x.Trackers.Enums.HasValue(CombatTrial.FinishedFirst)))
                        {
                            State = ScriptState.SpawningSecondWave;
                            // Get all Aislings in the subject
                            foreach (var aisling in Subject.GetEntities<Aisling>())
                                // Send an orange bar message to the Aisling
                                aisling.SendMessage("Miraelis: Theselene give this Aisling a challenge.");
                        }

                        if (Subject.GetEntities<Aisling>()
                            .Any(x => x.Trackers.Enums.HasValue(CombatTrial.FinishedSecond)))
                        {
                            State = ScriptState.SpawningThirdWave;
                            foreach (var aisling in Subject.GetEntities<Aisling>())
                                // Send an orange bar message to the Aisling
                                aisling.SendMessage("Miraelis: You handled that well. Serendael you're up.");
                        }

                        if (Subject.GetEntities<Aisling>()
                            .Any(x => x.Trackers.Enums.HasValue(CombatTrial.FinishedThird)))
                        {
                            State = ScriptState.SpawningFourthWave;

                            foreach (var aisling in Subject.GetEntities<Aisling>())
                                // Send an orange bar message to the Aisling
                                aisling.SendMessage("Miraelis: That was good. Skandara can you finish this?");
                        }

                        if (Subject.GetEntities<Aisling>()
                            .Any(x => x.Trackers.Enums.HasValue(CombatTrial.FinishedFourth)))
                        {
                            State = ScriptState.SpawningFifthWave;

                            foreach (var aisling in Subject.GetEntities<Aisling>())
                                // Send an orange bar message to the Aisling
                                aisling.SendMessage("Miraelis: Alright, enough games. Everyone test this Aisling.");

                        }

                        if (Subject.GetEntities<Aisling>()
                            .Any(x => x.Trackers.Enums.HasValue(CombatTrial.FinishedFifth)))
                        {
                            State = ScriptState.CompletedTrial;
                            foreach (var aisling in Subject.GetEntities<Aisling>())
                                // Send an orange bar message to the Aisling
                                aisling.SendMessage(
                                    "Miraelis: You are definitely the Aisling for this task. Come see me.");
                        }
                    }

                    break;
                // Spawning state
                case ScriptState.SpawningFirstWave:
                {
                    for (var i = 0; i <= 5; i++)
                    {
                        if (!Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                            continue;
                        
                        var monster = MonsterFactory.Create("mainstory_firstwave", Subject, point);
                        Subject.AddEntity(monster, monster);
                    }

                    // Set the state to spawned
                    State = ScriptState.SpawnedFirstWave;
                    break;
                }

                    break;
                // Spawned state
                case ScriptState.SpawnedFirstWave:
                {
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

                    if (!Subject.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>()))
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(CombatTrial.FinishedFirst);
                        }

                        State = ScriptState.DelayedStart;
                    }

                    break;
                }

            case ScriptState.SpawningSecondWave:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>())
                        // Send an orange bar message to the Aisling
                        aisling.SendMessage("Theselene: You won't make it through this.");
                    
                    for (var i = 0; i <= 5; i++)
                    {
                        if (!Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                            continue;
                        
                        var monster = MonsterFactory.Create("mainstory_secondwave", Subject, point);
                        Subject.AddEntity(monster, monster);
                    }

                    // Set the state to spawned
                    State = ScriptState.SpawnedSecondWave;
                    break;
                }

                    break;
                // Spawned state
                case ScriptState.SpawnedSecondWave:
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

                    if (!Subject.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>()))
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(CombatTrial.FinishedSecond);
                        }
                        State = ScriptState.DelayedStart;
                    }
                    break;
                
                case ScriptState.SpawningThirdWave:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>())
                        // Send an orange bar message to the Aisling
                        aisling.SendMessage("Serendael: Good luck, you'll need it.");
                    
                    for (var i = 0; i <= 5; i++)
                    {
                        if (!Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                            continue;
                        
                        var monster = MonsterFactory.Create("mainstory_thirdwave", Subject, point);
                        Subject.AddEntity(monster, monster);
                    }

                    // Set the state to spawned
                    State = ScriptState.SpawnedSecondWave;
                    break;
                }

                    break;
                // Spawned state
                case ScriptState.SpawnedThirdWave:
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

                    if (!Subject.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>()))
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(CombatTrial.FinishedThird);
                        }
                        State = ScriptState.DelayedStart;
                    }
                    break;
                
                case ScriptState.SpawningFourthWave:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>())
                        // Send an orange bar message to the Aisling
                        aisling.SendMessage("Skandara: You really are stubborn.");
                    
                    for (var i = 0; i <= 5; i++)
                    {
                        if (!Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                            continue;
                        
                        var monster = MonsterFactory.Create("mainstory_fourthwave", Subject, point);
                        Subject.AddEntity(monster, monster);
                    }

                    // Set the state to spawned
                    State = ScriptState.SpawnedFourthWave;
                    break;
                }
                // Spawned state
                case ScriptState.SpawnedFourthWave:
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

                    if (!Subject.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>()))
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(CombatTrial.FinishedFourth);
                        }
                        State = ScriptState.DelayedStart;
                    }
                    break;
                
                case ScriptState.SpawningFifthWave:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>())
                        // Send an orange bar message to the Aisling
                        aisling.SendMessage("Miraelis: The Gods will not hold back.");
                    
                    for (var i = 0; i <= 5; i++)
                    {
                        if (!Subject.Template.Bounds.TryGetRandomPoint(pt => !Subject.IsWall(pt), out var point))
                            continue;
                        
                        var monster = MonsterFactory.Create("mainstory_fifthwave", Subject, point);
                        Subject.AddEntity(monster, monster);
                    }

                    // Set the state to spawned
                    State = ScriptState.SpawnedFifthWave;
                    break;
                }
                // Spawned state
                case ScriptState.SpawnedFifthWave:
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

                    if (!Subject.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>()))
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(CombatTrial.FinishedFifth);
                        }
                        State = ScriptState.DelayedStart;
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
}