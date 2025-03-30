using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.karlopos;

public class QueenOctopusScript : MapScriptBase
{
    private readonly Animation Animation;
    private readonly IIntervalTimer AnimationInterval;
    private readonly IRectangle AnimationShape;

    private readonly IMonsterFactory MonsterFactory;
    private readonly List<Point> ReverseOutline;
    private readonly List<Point> ShapeOutline;
    private readonly TimeSpan StartDelay;
    private int AnimationIndex;
    private DateTime? StartTime;
    private ScriptState State;

    public QueenOctopusScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(5);
        AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        AnimationShape = new Rectangle(new Point(4, 8), 3, 3);

        ShapeOutline = AnimationShape.GetOutline()
                                     .ToList();

        ReverseOutline = ShapeOutline.AsEnumerable()
                                     .Reverse()
                                     .ToList();

        Animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 235
        };
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling)
            return;

        State = ScriptState.Dormant;
    }

    public override void Update(TimeSpan delta)
    {
        // Switch statement to determine the current state of the script
        switch (State)
        {
            case ScriptState.Dormant:
            {
                if (Subject.GetEntities<Aisling>()
                           .Any(
                               a => a.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage)
                                    && (stage == QueenOctopusQuest.QueenSpawning)))
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
                        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You hear waves crash against the beach...");
                }

                break;

            // Spawning state
            case ScriptState.Spawning:
                // Update the animation interval
                AnimationInterval.Update(delta);

                // Check if the animation interval has elapsed
                if (!AnimationInterval.IntervalElapsed)
                    return;

                // Get the points for the current animation index
                var pt1 = ShapeOutline[AnimationIndex];
                var pt2 = ReverseOutline[AnimationIndex];

                // Show the animations for the points
                Subject.ShowAnimation(Animation.GetPointAnimation(pt1));
                Subject.ShowAnimation(Animation.GetPointAnimation(pt2));

                // Increment the animation index
                AnimationIndex++;

                // Check if the animation index has exceeded the shape outline count
                if (AnimationIndex >= ShapeOutline.Count)
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>())
                        aisling.Trackers.Enums.Set(QueenOctopusQuest.QueenSpawned);

                    // Create a monster
                    var monster = MonsterFactory.Create("karlopos_queen_octopus", Subject, new Point(4, 8));

                    // Get the group level from the Aislings in the subject
                    var groupLevel = Subject.GetEntities<Aisling>()
                                            .Select(aisling => aisling.StatSheet.Level)
                                            .ToList();

                    // Create attributes based on the group level
                    var attrib = new Attributes
                    {
                        MaximumHp = groupLevel.Count * 20000,
                        MaximumMp = groupLevel.Count * 10000,
                        SkillDamagePct = groupLevel.Count * 2,
                        SpellDamagePct = groupLevel.Count * 2
                    };

                    // Add the attributes to the monster
                    monster.StatSheet.AddBonus(attrib);

                    // Add HP and MP to the monster
                    monster.StatSheet.SetHealthPct(100);
                    monster.StatSheet.SetManaPct(100);

                    // Add the monster to the subject
                    Subject.AddEntity(monster, monster);

                    // Set the state to spawned
                    State = ScriptState.Spawned;

                    // Reset the animation index
                    AnimationIndex = 0;
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

                    // Set the state to dormant
                    State = ScriptState.Dormant;
                }

                break;
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