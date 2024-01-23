using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Nightmare;

public class MalePriestNightmareChallengeMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1;
    private readonly Animation Animation;
    private readonly IIntervalTimer AnimationInterval;
    private readonly IRectangle AnimationShape1;
    protected readonly IExperienceDistributionScript ExperienceDistributionScript;
    private readonly IItemFactory ItemFactory;
    private readonly IIntervalTimer? MonsterDelay;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer? NightmareComplete;
    private readonly List<Point> ReverseOutline1;
    private readonly List<Point> ShapeOutline1;
    private readonly ISimpleCache SimpleCache;
    private readonly TimeSpan StartDelay;
    private readonly IIntervalTimer? UpdateTimer;
    private int AnimationIndex;
    private DateTime? StartTime;
    private ScriptState State;
    public required Rectangle? SpawnArea { get; set; }

    public MalePriestNightmareChallengeMapScript(
        MapInstance subject,
        IMonsterFactory monsterFactory,
        IItemFactory itemFactory,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        MonsterFactory = monsterFactory;
        StartDelay = TimeSpan.FromSeconds(2);
        AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(100));
        AnimationShape1 = new Rectangle(new Point(12, 11), 5, 5);
        ShapeOutline1 = AnimationShape1.GetOutline().ToList();
        ReverseOutline1 = ShapeOutline1.AsEnumerable().Reverse().ToList();
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        MonsterDelay = new IntervalTimer(TimeSpan.FromSeconds(30));
        NightmareComplete = new IntervalTimer(TimeSpan.FromMinutes(30));

        Animation = new Animation
        {
            AnimationSpeed = 150,
            TargetAnimation = 63
        };
    }

    private IPoint GenerateSpawnPoint() => (SpawnArea ?? Subject.Template.Bounds).GetRandomPoint();
    
    private void SpawnMonsters()
    {
        var monsters = new List<Monster>();

        for (var i = 0; i < 4; i++)
        {
            var point = GenerateSpawnPoint();

            var monster = MonsterFactory.Create(
                "nightmare_cthonic",
                Subject,
                point);

            monsters.Add(monster);
        }

        Subject.AddObjects(monsters);
    }
    
    private void SpawnTeam()
    {
        var teammates = new List<Monster>();

        var target = Subject.GetEntities<Aisling>().Single();
        
        var teammatespawnRectangle = new Rectangle(target, 5, 5);
        
        var point1 = teammatespawnRectangle.GetRandomPoint(point1 => point1 != target);
        var point2 = teammatespawnRectangle.GetRandomPoint(point2 => point2 != target);
        var point3 = teammatespawnRectangle.GetRandomPoint(point3 => point3 != target);
        var point4 = teammatespawnRectangle.GetRandomPoint(point4 => point4 != target);
        
            if (Subject.GetEntities<Aisling>().Any(a => a.Gender == Gender.Male))
            {
                var monster1 = MonsterFactory.Create(
                    "nightmare_malewarrior",
                    Subject,
                    point1);
                
                var monster2 = MonsterFactory.Create(
                    "nightmare_malemonk",
                    Subject,
                    point2);
                
                var monster3 = MonsterFactory.Create(
                    "nightmare_malerogue",
                    Subject,
                    point3);
                
                var monster4 = MonsterFactory.Create(
                    "nightmare_malewizard",
                    Subject,
                    point4);
                
                teammates.Add(monster1);
                teammates.Add(monster2);
                teammates.Add(monster3);
                teammates.Add(monster4);
            }
            else
            {
                var monster1 = MonsterFactory.Create(
                    "nightmare_femalewarrior",
                    Subject,
                    point1);
                
                var monster2 = MonsterFactory.Create(
                    "nightmare_femalemonk",
                    Subject,
                    point2);
                
                var monster3 = MonsterFactory.Create(
                    "nightmare_femalerogue",
                    Subject,
                    point3);
                
                var monster4 = MonsterFactory.Create(
                    "nightmare_femalewizard",
                    Subject,
                    point4);
                
                teammates.Add(monster1);
                monster1.PetOwner = target;
                teammates.Add(monster2);
                monster2.PetOwner = target;
                teammates.Add(monster3);
                monster3.PetOwner = target;
                teammates.Add(monster4);
                monster4.PetOwner = target;
            }

        Subject.AddObjects(teammates);
    }

    private void SpawnWalls()
    {
        var wallPoints = new Point[]
        {
            new Point(0, 9),
            new Point(0, 10),
            new Point(0, 11),
            new Point(0, 12),
            new Point(13, 24),
            new Point(14, 24),
            new Point(15, 24),
            new Point(16, 24),
            new Point(17, 24),
            new Point(29, 12),
            new Point(29, 11),
            new Point(29, 10),
            new Point(29, 9),
            new Point(29, 8),
            new Point(16, 0),
            new Point(15, 0),
            new Point(14, 0),
            new Point(13, 0)
        };

        var windwalls = new List<Monster>();

        foreach (var wallPoint in wallPoints)
        {
            var windwall = MonsterFactory.Create("nightmare_windwall", Subject, wallPoint);
            windwalls.Add(windwall);
        }

        Subject.AddObjects(windwalls);
    }


    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);
        MonsterDelay?.Update(delta);
        NightmareComplete?.Update(delta);

        if (UpdateTimer!.IntervalElapsed)
            // Switch statement to determine the current state of the script
            switch (State)
            {
                case ScriptState.Dormant:
                {
                    if (Subject.GetEntities<Aisling>()
                               .Any(
                                   a => a.Trackers.Enums.TryGetValue(out NightmareQuestStage stage)
                                        && (stage == NightmareQuestStage.EnteredDream)))
                    {
                        SpawnWalls();
                        State = ScriptState.DelayedStart;
                    }
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
                        
                        SpawnTeam();

                        // Get all Aislings in the subject
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                            // Send an orange bar message to the Aisling
                            aisling.Client.SendServerMessage(
                                ServerMessageType.OrangeBar1,
                                "Your group has arrived. Keep them alive for 6 minutes!");
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
                    var pt1 = ShapeOutline1[AnimationIndex];
                    var pt2 = ReverseOutline1[AnimationIndex];
                    // Show the animations for the points
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt1));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt2));
                    // Increment the animation index
                    AnimationIndex++;

                    // Check if the animation index has exceeded the shape outline count
                    if (AnimationIndex >= ShapeOutline1.Count)
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(NightmareQuestStage.SpawnedNightmare);
                            aisling.SendOrangeBarMessage("Enemies incoming! Protect your group.");
                        }

                        SpawnMonsters();

                        State = ScriptState.Spawned;
                        // Reset the animation index
                        AnimationIndex = 0;
                    }

                    break;
                // Spawned state
                case ScriptState.Spawned:
                {
                    if (NightmareComplete!.IntervalElapsed)
                    {
                        State = ScriptState.Complete;

                        return;
                    }

                    if (MonsterDelay!.IntervalElapsed)
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                            aisling.SendOrangeBarMessage("There's more incoming!");

                        SpawnMonsters();
                    }

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
                }

                    break;

                case ScriptState.Complete:
                {
                    var nightmaregearDictionary = new Dictionary<(BaseClass, Gender), string[]>
                    {
                        { (BaseClass.Warrior, Gender.Male), new[] { "malecarnunplate", "carnunhelmet" } },
                        { (BaseClass.Warrior, Gender.Female), new[] { "femalecarnunplate", "carnunhelmet" } },
                        { (BaseClass.Monk, Gender.Male), new[] { "maleaosdicpatternwalker" } },
                        { (BaseClass.Monk, Gender.Female), new[] { "femaleaosdicpatternwalker" } },
                        { (BaseClass.Rogue, Gender.Male), new[] { "malemarauderhide", "maraudermask" } },
                        { (BaseClass.Rogue, Gender.Female), new[] { "femalemarauderhide", "maraudermask" } },
                        { (BaseClass.Priest, Gender.Male), new[] { "malecthonicdisciplerobes", "cthonicdisciplecaputium" } },
                        { (BaseClass.Priest, Gender.Female), new[] { "morrigudisciplepellison", "holyhairband" } },
                        { (BaseClass.Wizard, Gender.Male), new[] { "malecthonicmagusrobes", "cthonicmaguscaputium" } },
                        { (BaseClass.Wizard, Gender.Female), new[] { "morrigumaguspellison", "magushairband" } }
                    };

                    foreach (var aisling in Subject.GetEntities<Aisling>())
                    {
                        ExperienceDistributionScript.GiveExp(aisling, 500000);
                        aisling.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin1);

                        aisling.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Successfully conquered their Nightmares",
                                "Nightmare",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        var pointS = new Point(5, 7);
                        aisling.TraverseMap(mapInstance, pointS);

                        var gearKey = (aisling.UserStatSheet.BaseClass, aisling.Gender);

                        if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
                        {
                            var hasGear = nightmaregear.All(
                                gearItemName =>
                                    aisling.Inventory.ContainsByTemplateKey(gearItemName)
                                    || aisling.Bank.Contains(gearItemName)
                                    || aisling.Equipment.ContainsByTemplateKey(gearItemName));

                            if (!hasGear)
                                foreach (var gearItemName in nightmaregear)
                                {
                                    var gearItem = ItemFactory.Create(gearItemName);
                                    aisling.GiveItemOrSendToBank(gearItem);
                                }
                        }

                        aisling.SendOrangeBarMessage("You wake up from the nightmare feeling refreshed.");
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
    }

    private enum ScriptState
    {
        Dormant,
        DelayedStart,
        Spawning,
        Spawned,
        Complete
    }
}