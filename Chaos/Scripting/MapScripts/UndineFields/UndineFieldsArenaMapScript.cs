using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.UndineFields;

public class UndineFieldsArenaMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1;
    private readonly Animation Animation;
    private readonly IIntervalTimer AnimationInterval;
    private readonly IRectangle AnimationShape;
    private readonly IMonsterFactory MonsterFactory;
    private readonly List<Point> ReverseOutline;
    private readonly List<Point> ShapeOutline;
    private readonly TimeSpan StartDelay;
    private readonly IIntervalTimer? UpdateTimer;
    private int AnimationIndex;
    private DateTime? StartTime;
    private ScriptState State;

    public UndineFieldsArenaMapScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(1);
        AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(20));
        AnimationShape = new Rectangle(new Point(18, 18), 6, 6);

        ShapeOutline = AnimationShape.GetOutline()
                                     .ToList();

        ReverseOutline = ShapeOutline.AsEnumerable()
                                     .Reverse()
                                     .ToList();
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));

        Animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 293
        };
    }

    private void SpawnWalls()
    {
        var wallPoints = new[]
        {
            new Point(27, 15),
            new Point(27, 16),
            new Point(27, 17),
            new Point(27, 18),
            new Point(27, 19),
            new Point(27, 20),
            new Point(27, 21),
            new Point(26, 15),
            new Point(26, 16),
            new Point(26, 17),
            new Point(26, 18),
            new Point(26, 19),
            new Point(26, 20),
            new Point(26, 21),
            new Point(15, 26),
            new Point(16, 26),
            new Point(17, 26),
            new Point(18, 26),
            new Point(19, 26),
            new Point(20, 26),
            new Point(21, 26),
            new Point(15, 27),
            new Point(16, 27),
            new Point(17, 27),
            new Point(18, 27),
            new Point(19, 27),
            new Point(20, 27),
            new Point(21, 27),
            new Point(10, 15),
            new Point(10, 16),
            new Point(10, 17),
            new Point(10, 18),
            new Point(10, 19),
            new Point(10, 20),
            new Point(10, 21),
            new Point(9, 15),
            new Point(9, 16),
            new Point(9, 17),
            new Point(9, 18),
            new Point(9, 19),
            new Point(9, 20),
            new Point(9, 21),
            new Point(15, 10),
            new Point(16, 10),
            new Point(17, 10),
            new Point(18, 10),
            new Point(19, 10),
            new Point(20, 10),
            new Point(21, 10),
            new Point(15, 9),
            new Point(16, 9),
            new Point(17, 9),
            new Point(18, 9),
            new Point(19, 9),
            new Point(20, 9),
            new Point(21, 9)
        };

        var windwalls = new List<Monster>();

        foreach (var wallPoint in wallPoints)
        {
            var windwall = MonsterFactory.Create("carnun_wall", Subject, wallPoint);
            windwalls.Add(windwall);
        }

        Subject.AddEntities(windwalls);
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
                                   x => x.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage)
                                        && (stage == UndineFieldDungeon.EnteredArena)))
                    {
                        foreach (var member in Subject.GetEntities<Aisling>()
                                                      .ToList())
                        {
                            var hasStage = member.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage);

                            member.Trackers.Enums.Set(UndineFieldDungeon.EnteredArena);
                        }

                        State = ScriptState.DelayedStart;
                    }
                }

                    break;

                // Delayed start state
                case ScriptState.DelayedStart:
                    // Set the start time if it is not already set
                    StartTime ??= DateTime.UtcNow;

                    // Check if the start delay has been exceeded
                    if ((DateTime.UtcNow - StartTime) > StartDelay)
                    {
                        var rectangle = new Rectangle(
                            12,
                            20,
                            4,
                            4);

                        if (Subject.GetEntities<Aisling>()
                                   .Any(
                                       x => x.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage)
                                            && (stage == UndineFieldDungeon.EnteredArena)))
                            foreach (var member in Subject.GetEntities<Aisling>()
                                                          .ToList())
                            {
                                var hasStage = member.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage);

                                Point point;

                                do
                                    point = rectangle.GetRandomPoint();
                                while (!Subject.IsWalkable(point, member.Type));

                                member.TraverseMap(Subject, point);
                                member.Trackers.Enums.Set(UndineFieldDungeon.StartedCarnun);
                                member.SendOrangeBarMessage("Face the Carnun Champion");
                            }

                        SpawnWalls();

                        // Reset the start time
                        StartTime = null;

                        // Set the state to spawning
                        State = ScriptState.Spawning;
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
                        // Create a monster
                        var monster = MonsterFactory.Create("undine_field_carnun", Subject, new Point(22, 14));

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