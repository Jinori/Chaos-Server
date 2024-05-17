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

public class RogueNightmareChallengeMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1;
    private readonly Animation Animation;
    private readonly IIntervalTimer AnimationInterval;
    private readonly IRectangle AnimationShape1;
    private readonly IRectangle AnimationShape2;
    private readonly IRectangle AnimationShape3;
    private readonly IRectangle AnimationShape4;
    private readonly IRectangle AnimationShape5;
    private readonly IRectangle AnimationShape6;
    private readonly IRectangle AnimationShape7;
    private readonly IRectangle AnimationShape8;
    private readonly IRectangle AnimationShape9;
    protected readonly IExperienceDistributionScript ExperienceDistributionScript;
    private readonly IItemFactory ItemFactory;
    private readonly IIntervalTimer? MonsterDelay;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer? NightmareComplete;
    private readonly List<Point> ReverseOutline1;
    private readonly List<Point> ReverseOutline2;
    private readonly List<Point> ReverseOutline3;
    private readonly List<Point> ReverseOutline4;
    private readonly List<Point> ReverseOutline5;
    private readonly List<Point> ReverseOutline6;
    private readonly List<Point> ReverseOutline7;
    private readonly List<Point> ReverseOutline8;
    private readonly List<Point> ReverseOutline9;
    private readonly List<Point> ShapeOutline1;
    private readonly List<Point> ShapeOutline2;
    private readonly List<Point> ShapeOutline3;
    private readonly List<Point> ShapeOutline4;
    private readonly List<Point> ShapeOutline5;
    private readonly List<Point> ShapeOutline6;
    private readonly List<Point> ShapeOutline7;
    private readonly List<Point> ShapeOutline8;
    private readonly List<Point> ShapeOutline9;
    private readonly ISimpleCache SimpleCache;
    private readonly TimeSpan StartDelay;
    private readonly IIntervalTimer? UpdateTimer;
    public required Rectangle? SpawnArea { get; set; }
    private int AnimationIndex;
    private DateTime? StartTime;
    private ScriptState State;

    public RogueNightmareChallengeMapScript(
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
        AnimationShape2 = new Rectangle(new Point(12, 11), 6, 6);
        AnimationShape3 = new Rectangle(new Point(12, 11), 7, 7);
        AnimationShape4 = new Rectangle(new Point(12, 11), 8, 8);
        AnimationShape5 = new Rectangle(new Point(12, 11), 9, 9);
        AnimationShape6 = new Rectangle(new Point(12, 11), 10, 10);
        AnimationShape7 = new Rectangle(new Point(12, 11), 11, 11);
        AnimationShape8 = new Rectangle(new Point(12, 11), 12, 12);
        AnimationShape9 = new Rectangle(new Point(12, 11), 13, 13);
        ShapeOutline1 = AnimationShape1.GetOutline().ToList();
        ShapeOutline2 = AnimationShape2.GetOutline().ToList();
        ShapeOutline3 = AnimationShape3.GetOutline().ToList();
        ShapeOutline4 = AnimationShape4.GetOutline().ToList();
        ShapeOutline5 = AnimationShape5.GetOutline().ToList();
        ShapeOutline6 = AnimationShape6.GetOutline().ToList();
        ShapeOutline7 = AnimationShape7.GetOutline().ToList();
        ShapeOutline8 = AnimationShape8.GetOutline().ToList();
        ShapeOutline9 = AnimationShape9.GetOutline().ToList();
        ReverseOutline1 = ShapeOutline1.AsEnumerable().Reverse().ToList();
        ReverseOutline2 = ShapeOutline2.AsEnumerable().Reverse().ToList();
        ReverseOutline3 = ShapeOutline3.AsEnumerable().Reverse().ToList();
        ReverseOutline4 = ShapeOutline4.AsEnumerable().Reverse().ToList();
        ReverseOutline5 = ShapeOutline5.AsEnumerable().Reverse().ToList();
        ReverseOutline6 = ShapeOutline6.AsEnumerable().Reverse().ToList();
        ReverseOutline7 = ShapeOutline7.AsEnumerable().Reverse().ToList();
        ReverseOutline8 = ShapeOutline8.AsEnumerable().Reverse().ToList();
        ReverseOutline9 = ShapeOutline9.AsEnumerable().Reverse().ToList();
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        MonsterDelay = new IntervalTimer(TimeSpan.FromSeconds(30));
        NightmareComplete = new IntervalTimer(TimeSpan.FromMinutes(6));

        Animation = new Animation
        {
            AnimationSpeed = 150,
            TargetAnimation = 63
        };
    }

    private void SpawnMonsters()
    {
        var monsters = new List<Monster>();

        for (var i = 0; i < 4; i++)
        {
            if (!Subject.TryGetRandomWalkablePoint(out var point))
                continue;
            
            var monster = MonsterFactory.Create(
                "nightmare_murauder",
                Subject,
                point);

            monsters.Add(monster);
        }

        Subject.AddObjects(monsters);
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
                                "Clouds of smoke begin to surround you from all directions...");
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
                    var pt3 = ShapeOutline2[AnimationIndex];
                    var pt4 = ReverseOutline2[AnimationIndex];
                    var pt5 = ShapeOutline3[AnimationIndex];
                    var pt6 = ReverseOutline3[AnimationIndex];
                    var pt7 = ShapeOutline4[AnimationIndex];
                    var pt8 = ReverseOutline4[AnimationIndex];
                    var pt9 = ShapeOutline5[AnimationIndex];
                    var pt10 = ReverseOutline5[AnimationIndex];
                    var pt11 = ShapeOutline6[AnimationIndex];
                    var pt12 = ReverseOutline6[AnimationIndex];
                    var pt13 = ShapeOutline7[AnimationIndex];
                    var pt14 = ReverseOutline7[AnimationIndex];
                    var pt15 = ShapeOutline8[AnimationIndex];
                    var pt16 = ReverseOutline8[AnimationIndex];
                    var pt17 = ShapeOutline9[AnimationIndex];
                    var pt18 = ReverseOutline9[AnimationIndex];
                    // Show the animations for the points
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt1));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt2));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt4));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt3));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt5));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt6));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt8));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt7));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt9));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt18));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt16));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt17));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt14));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt15));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt11));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt12));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt10));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt13));
                    // Increment the animation index
                    AnimationIndex++;

                    // Check if the animation index has exceeded the shape outline count
                    if (AnimationIndex >= ShapeOutline1.Count)
                    {
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            aisling.Trackers.Enums.Set(NightmareQuestStage.SpawnedNightmare);
                            aisling.SendOrangeBarMessage("The smoke clears... you are not alone. Defend the Totem!");
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
                        { (BaseClass.Warrior, Gender.Male), ["malecarnunplate", "carnunhelmet"] },
                        { (BaseClass.Warrior, Gender.Female), ["femalecarnunplate", "carnunhelmet"] },
                        { (BaseClass.Monk, Gender.Male), ["maleaosdicpatternwalker"] },
                        { (BaseClass.Monk, Gender.Female), ["femaleaosdicpatternwalker"] },
                        { (BaseClass.Rogue, Gender.Male), ["malemarauderhide", "maraudermask"] },
                        { (BaseClass.Rogue, Gender.Female), ["femalemarauderhide", "maraudermask"] },
                        { (BaseClass.Priest, Gender.Male), ["malecthonicdisciplerobes", "cthonicdisciplecaputium"] },
                        { (BaseClass.Priest, Gender.Female), ["morrigudisciplepellison", "holyhairband"] },
                        { (BaseClass.Wizard, Gender.Male), ["cthonicmagusrobes", "cthonicmaguscaputium"] },
                        { (BaseClass.Wizard, Gender.Female), ["morrigumaguspellison", "magushairband"] }
                    };

                    var player = Subject.GetEntities<Aisling>().FirstOrDefault(x =>
                        x.Trackers.Enums.TryGetValue(out NightmareQuestStage hasNightmare) &&
                        (hasNightmare == NightmareQuestStage.SpawnedNightmare));

                    if (player != null)
                    {
                        player.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin1);
                        player.Trackers.Counters.Remove("nightmarekills", out _);
                        ExperienceDistributionScript.GiveExp(player, 500000);

                        player.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Successfully conquered their Nightmares",
                                "Nightmare",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));

                        var gearKey = (player.UserStatSheet.BaseClass, player.Gender);

                        if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
                        {
                            var hasGear = nightmaregear.All(
                                gearItemName =>
                                    player.Inventory.ContainsByTemplateKey(gearItemName)
                                    || player.Bank.Contains(gearItemName)
                                    || player.Equipment.ContainsByTemplateKey(gearItemName));

                            if (!hasGear)
                                foreach (var gearItemName in nightmaregear)
                                {
                                    var gearItem = ItemFactory.Create(gearItemName);
                                    player.GiveItemOrSendToBank(gearItem);
                                }
                        }

                        var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                        var pointS = new Point(5, 7);
                        player.TraverseMap(mapInstance, pointS);
                        player.SendOrangeBarMessage("You wake up from the nightmare feeling refreshed.");
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