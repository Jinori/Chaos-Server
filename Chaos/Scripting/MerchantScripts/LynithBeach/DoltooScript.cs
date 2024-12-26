using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.LynithBeach;

public sealed class DoltooScript : MerchantScriptBase
{
    public enum CheckpointState
    {
        Idle,
        FollowingPlayer,
        ExitingMap
    }

    private readonly IIntervalTimer AnimationTimer;

    private readonly Dictionary<int, List<string>> CheckpointSayings = new()
    {
        {
            1, new List<string>
            {
                "Are you sure it's this way?",
                "Oh, I've been this way before.",
                "Those pirates are nice to me sometimes.",
                "Should we go the other way?",
                "Can you slow down? My feet hurt.",
                "There's no out this way.",
                "Are we there yet?",
                "Where are you taking me again?",
                "Can we go back? I forgot something...",
                "Please tell me you know the way."
            }
        }
    };

    private readonly Random Random = new();
    private readonly IIntervalTimer SayingsTimer;
    private readonly IIntervalTimer SayingsTimer2;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer WalkTimer;
    private Aisling? AislingToFollow;
    private CheckpointState State;

    public CheckpointState DoltooState { get; set; } = CheckpointState.Idle;

    private Animation Animation { get; } = new()
    {
        AnimationSpeed = 50,
        TargetAnimation = 96
    };

    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public DoltooScript(Merchant subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));

        SayingsTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(30),
            40,
            RandomizationType.Balanced,
            false);

        SayingsTimer2 = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(5),
            10,
            RandomizationType.Balanced,
            false);
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    private void DisplayRandomSaying()
    {
        if (CheckpointSayings.TryGetValue(1, out var sayings))
        {
            var saying = sayings[Random.Next(sayings.Count)];
            Subject.Say(saying);
        }
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        AnimationTimer.Update(delta);
        WalkTimer.Update(delta);
        SayingsTimer.Update(delta);
        SayingsTimer2.Update(delta);

        switch (State)
        {
            case CheckpointState.Idle:
            {
                var aislingOnQuest = Subject.MapInstance
                                            .GetEntities<Aisling>()
                                            .Any(
                                                x => x.Trackers.Enums.HasValue(HelpSable.StartedDoltoo)
                                                     || x.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart));

                if (aislingOnQuest)
                {
                    var players = Subject.MapInstance
                                         .GetEntities<Aisling>()
                                         .Where(
                                             x => x.Trackers.Enums.HasValue(HelpSable.StartedDoltoo)
                                                  || x.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart));

                    foreach (var player in players)
                        player.Trackers.Enums.Set(HelpSable.EscortingDoltooStart);
                    State = CheckpointState.FollowingPlayer;
                    AnimationTimer.Reset();
                    WalkTimer.Reset();
                    SayingsTimer.Reset();
                }

                break;
            }

            case CheckpointState.FollowingPlayer:
            {
                if (AnimationTimer.IntervalElapsed)
                {
                    var options = new AoeShapeOptions
                    {
                        Source = new Point(Subject.X, Subject.Y),
                        Range = 4
                    };

                    var points = AoeShape.AllAround.ResolvePoints(options);
                    var enumerable = points as Point[] ?? points.ToArray();

                    foreach (var tile in enumerable)
                        Subject.MapInstance.ShowAnimation(Animation.GetPointAnimation(tile));
                }

                if (SayingsTimer.IntervalElapsed)
                {
                    DisplayRandomSaying();
                    SayingsTimer.Reset();
                }

                var merchantPoint = new Point(Subject.X, Subject.Y);

                var rectBrig = new Rectangle(
                    1,
                    10,
                    6,
                    5);

                if (rectBrig.Contains(merchantPoint))
                {
                    State = CheckpointState.ExitingMap;

                    return;
                }

                if (WalkTimer.IntervalElapsed)
                {
                    var options = new AoeShapeOptions
                    {
                        Source = new Point(Subject.X, Subject.Y),
                        Range = 4
                    };

                    var points = AoeShape.AllAround.ResolvePoints(options);

                    var playersInAoe = points.SelectMany(point => Subject.MapInstance.GetEntitiesAtPoints<Aisling>(point))
                                             .ToList();

                    var playerNearby = playersInAoe.Count >= 1;

                    if (playerNearby)
                    {
                        var aislings = Subject.MapInstance
                                              .GetEntitiesWithinRange<Aisling>(Subject, 4)
                                              .ToList();

                        if (aislings.Count > 0)
                            AislingToFollow = aislings.Count == 1 ? aislings[0] : aislings.PickRandom();

                        if (AislingToFollow != null)
                        {
                            var point = new Point(AislingToFollow.X, AislingToFollow.Y);
                            var location = new Location(Subject.MapInstance.InstanceId, point);

                            Subject.Pathfind(location);
                        }
                    }
                }

                break;
            }

            case CheckpointState.ExitingMap:
            {
                var merchantPoint = new Point(Subject.X, Subject.Y);
                var doorpoint = new Point(0, 11);

                var rectDoor = new Rectangle(
                    0,
                    10,
                    3,
                    3);

                var rectBrig = new Rectangle(
                    0,
                    9,
                    7,
                    6);

                if (!rectBrig.Contains(merchantPoint))
                {
                    State = CheckpointState.FollowingPlayer;

                    return;
                }

                if (SayingsTimer2.IntervalElapsed)
                {
                    Subject.Say("Oh, there's the door, I know my way out!");
                    SayingsTimer.Reset();
                }

                if (WalkTimer.IntervalElapsed)
                    Subject.Pathfind(doorpoint);

                if (rectDoor.Contains(merchantPoint))
                {
                    Subject.MapInstance.RemoveEntity(Subject);

                    var players = Subject.MapInstance
                                         .GetEntities<Aisling>()
                                         .Where(
                                             x => x.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart)
                                                  || x.Trackers.Flags.HasFlag(ShipAttackFlags.FinishedDoltoo));

                    foreach (var player in players)
                    {
                        if (player.Trackers.Flags.HasFlag(ShipAttackFlags.FinishedDoltoo))
                        {
                            player.SendOrangeBarMessage("Thank you for helping others.");
                            player.TryGiveGamePoints(25);
                            ExperienceDistributionScript.GiveExp(player, 10000000);
                        } else
                            player.Trackers.Enums.Set(HelpSable.CompletedEscort);

                        player.SendOrangeBarMessage("Doltoo quickly escapes the ship and you follow.");

                        var point2 = new Point(44, 16);
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_beach_south");
                        player.TraverseMap(mapinstance1, point2);
                    }
                }

                break;
            }
        }
    }
}