﻿using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
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

public class WarriorNightmareChallengeMapScript : MapScriptBase
{
    public const int UPDATE_INTERVAL_MS = 1;
    private readonly Animation Animation;
    private readonly IIntervalTimer AnimationInterval;
    private readonly ICircle AnimationShape1;
    protected readonly IExperienceDistributionScript ExperienceDistributionScript;
    private readonly IItemFactory ItemFactory;
    private readonly IIntervalTimer? PriestDelay;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IMerchantFactory MerchantFactory;
    private readonly List<Point> ReverseOutline1;
    private readonly List<Point> ShapeOutline1;
    private readonly ISimpleCache SimpleCache;
    private readonly TimeSpan StartDelay;
    private readonly IIntervalTimer? UpdateTimer;
    private int AnimationIndex;
    private DateTime? StartTime;
    private ScriptState State;
    public required Rectangle? SpawnArea { get; set; }

    public WarriorNightmareChallengeMapScript(
        MapInstance subject,
        IMonsterFactory monsterFactory,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        IMerchantFactory merchantFactory
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        MonsterFactory = monsterFactory;
        MerchantFactory = merchantFactory;
        StartDelay = TimeSpan.FromSeconds(3);
        AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(100));
        AnimationShape1 = new Circle(new Point(9, 9), 3);
        ShapeOutline1 = AnimationShape1.GetOutline().ToList();
        ReverseOutline1 = ShapeOutline1.AsEnumerable().Reverse().ToList();
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        PriestDelay = new IntervalTimer(TimeSpan.FromSeconds(35));

        Animation = new Animation
        {
            AnimationSpeed = 150,
            TargetAnimation = 63
        };
    }

    private IPoint GenerateSpawnPoint() => (SpawnArea ?? Subject.Template.Bounds).GetRandomPoint();

    private void SpawnMonsters()
    {

        var point = new Point(9, 9);

        var monster = MonsterFactory.Create(
            "nightmare_carnun",
            Subject, point);

        Subject.AddEntity(monster, point);
    }


    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);
        PriestDelay?.Update(delta);

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

                        // Get all Aislings in the subject
                        foreach (var aisling in Subject.GetEntities<Aisling>())
                            // Send an orange bar message to the Aisling
                            aisling.Client.SendServerMessage(
                                ServerMessageType.OrangeBar1,
                                "Fight the Carnun to the death.");
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
                            aisling.SendOrangeBarMessage("You can feel the great Carnun's presence.");
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
                    if (!Subject.GetEntities<Monster>().Any())
                        State = ScriptState.Complete;


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

                    if (PriestDelay!.IntervalElapsed)
                    {
                        var point = GenerateSpawnPoint();
                        var priest = MerchantFactory.Create(
                            "nightmare_priestsupport",
                            Subject, point);

                        Subject.AddEntity(priest, point);
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