using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class TerrorOfTheCryptScript : MapScriptBase
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

    private Element[] Elements { get; } =
        [
            Element.Fire,
            Element.Water,
            Element.Wind,
            Element.Earth
        ];

    public TerrorOfTheCryptScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(5);
        AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        AnimationShape = new Rectangle(new Point(8, 8), 5, 5);

        ShapeOutline = AnimationShape.GetOutline()
                                     .ToList();

        ReverseOutline = ShapeOutline.AsEnumerable()
                                     .Reverse()
                                     .ToList();

        Animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 13
        };
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        if (aisling.Trackers.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt) && (State == ScriptState.Dormant))
            State = ScriptState.DelayedStart;
    }

    public override void Update(TimeSpan delta)
    {
        // Switch statement to determine the current state of the script
        switch (State)
        {
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
                        aisling.Client.SendServerMessage(
                            ServerMessageType.OrangeBar1,
                            "You hear a creak, as the coffin begins to slide open...");
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
                    var groupLevel = Subject.GetEntities<Aisling>()
                                            .Select(aisling => aisling.StatSheet.Level)
                                            .ToList();
                    Monster monster;

                    switch (groupLevel.Average())
                    {
                        case <= 10:
                            monster = MonsterFactory.Create("terror10Minus", Subject, new Point(8, 8));

                            break;

                        case <= 19:
                            monster = MonsterFactory.Create("terror10To19", Subject, new Point(8, 8));
                            monster.StatSheet.SetDefenseElement(Elements.PickRandom());

                            break;

                        case <= 25:
                            monster = MonsterFactory.Create("terror20To25", Subject, new Point(8, 8));
                            monster.StatSheet.SetOffenseElement(Element.Darkness);
                            monster.StatSheet.SetDefenseElement(Elements.PickRandom());

                            break;

                        case <= 40:
                            monster = MonsterFactory.Create("terror26To40", Subject, new Point(8, 8));
                            monster.StatSheet.SetOffenseElement(Element.Darkness);
                            monster.StatSheet.SetDefenseElement(Element.Darkness);

                            break;

                        default:
                            monster = MonsterFactory.Create("terror10Minus", Subject, new Point(8, 8));

                            break;
                    }

                    var attrib = new Attributes
                    {
                        AtkSpeedPct = (int)groupLevel.Average() * 4 / 2,
                        MaximumHp = (int)groupLevel.Average() * groupLevel.Count * 500,
                        MaximumMp = (int)groupLevel.Average() * groupLevel.Count * 500,
                        SkillDamagePct = groupLevel.Count,
                        SpellDamagePct = groupLevel.Count
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