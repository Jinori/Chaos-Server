using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class NightmareBossMapScript : MapScriptBase
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
    private readonly IIntervalTimer? UpdateTimer;
    public const int UPDATE_INTERVAL_MS = 300;

    public NightmareBossMapScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(2);
        AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(50));
        AnimationShape = new Rectangle(new Point(5, 5), 5, 5);
        ShapeOutline = AnimationShape.GetOutline().ToList();
        ReverseOutline = ShapeOutline.AsEnumerable().Reverse().ToList();
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS));

        Animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 251
        };
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
                                   a => a.Trackers.Enums.TryGetValue(out NightmareQuestStage stage)
                                        && (stage == NightmareQuestStage.EnteredDream)))
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
                                "You feel a chill run down your spine.");
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
                        {
                            aisling.Trackers.Enums.Set(NightmareQuestStage.SpawnedNightmare);
    
                            var baseClass = aisling.UserStatSheet.BaseClass;
                            var isMale = (aisling.Gender & Gender.Male) != 0;

                            var monsterName = baseClass switch
                            {
                                BaseClass.Warrior => isMale ? "nightmare_malewarrior" : "nightmare_femalewarrior",
                                BaseClass.Priest  => isMale ? "nightmare_malepriest" : "nightmare_femalepriest",
                                BaseClass.Rogue   => isMale ? "nightmare_malerogue" : "nightmare_femalerogue",
                                BaseClass.Monk    => isMale ? "nightmare_malemonk" : "nightmare_femalemonk",
                                BaseClass.Wizard  => isMale ? "nightmare_malewizard" : "nightmare_femalewizard",
                                _                 => "default_monster_name"
                            };

                            var monster = MonsterFactory.Create(monsterName, Subject, new Point(5, 5));
                            Subject.AddEntity(monster, monster);
                        }
                        // Set the state to spawned
                        State = ScriptState.Spawned;
                        // Reset the animation index
                        AnimationIndex = 0;
                    }

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