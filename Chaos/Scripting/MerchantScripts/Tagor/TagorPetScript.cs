using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Tagor;

public class TagorPetScript : MerchantScriptBase
{
    public enum TagorPetState
    {
        Idle,
        Wandering,
        FollowingPlayer,
        WalkToSpawnPoint,
        Eating
    }

    private static readonly List<KeyValuePair<TagorPetState, decimal>> StateData =
    [
        new(TagorPetState.Wandering, 60),
        new(TagorPetState.FollowingPlayer, 40),
        new(TagorPetState.Eating, 20),
    ];

    #region Messages
    private static readonly List<string> EatingActions =
    [
        "snarfs",
        "snaps it up",
        "licks",
        "devours hungrily",
        "crunches",
        "chews happily",
        "sniffs",
        "gnaws",
        "munches quickly",
        "nibbles playfully",
        "whines",
        "buries their nose in it",
    ];

    #endregion Messages

    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer EatingTimer;

    private readonly TimeSpan MaxFollowDuration = TimeSpan.FromSeconds(50);

    private readonly Location Spawnpoint;
    private readonly IIntervalTimer WalkTimer;
    private DateTime FollowUntil;
    public bool HasPickedAnAisling;
    private DateTime LastChant;
    private Aisling? RandomAisling;

    private IPathOptions Options
        => PathOptions.Default with
        {
            LimitRadius = null,
            IgnoreBlockingReactors = true
        };

    /// <inheritdoc />
    public TagorPetScript(Merchant subject)
        : base(subject)
    {
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(500), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(5));
        Subject.TagorPetState = TagorPetState.Idle;

        EatingTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);

        Spawnpoint = new Location(Subject.MapInstance.InstanceId, Subject.X, Subject.Y);
    }

    private void DisplayEatingMessage()
    {
        var chant = EatingActions.PickRandom();
        Subject.Chant($"*{chant}*");
    }

    public static DisplayColor GetRandomDisplayColor()
    {
        var value = Enum.GetValues<DisplayColor>()
                        .ToList();
        var random = value.PickRandom();

        return random;
    }

    private void HandleEatingState()
    {
        if ((DateTime.Now - LastChant).TotalSeconds >= 2)
        {
            DisplayEatingMessage();
            LastChant = DateTime.Now;
        }

        if (EatingTimer.IntervalElapsed)
            EatingTimer.Reset();
    }

    private void HandleFollowingPlayer(TimeSpan delta)
    {
        if (!HasPickedAnAisling)
        {
            var aislings = Subject.MapInstance
                                  .GetEntitiesWithinRange<Aisling>(Subject)
                                  .ThatAreObservedBy(Subject)
                                  .ThatAreVisibleTo(Subject)
                                  .ToList();

            if (aislings.Count > 0)
            {
                RandomAisling = aislings.Count == 1 ? aislings[0] : aislings.PickRandom();
                FollowUntil = DateTime.Now.AddSeconds(IntegerRandomizer.RollSingle((int)MaxFollowDuration.TotalSeconds));
                HasPickedAnAisling = true;
            }
        }

        if (HasPickedAnAisling && (DateTime.Now < FollowUntil))
        {
            if (RandomAisling != null)
            {
                var point = new Point(RandomAisling.X, RandomAisling.Y);
                var location = new Location(Subject.MapInstance.InstanceId, point);

                if (ShouldWalkTo(location))
                    WalkTowardsPlayer(location, delta);
                else
                {
                    if (ShouldWalkToSpawnPoint())
                        WalkTowards(Spawnpoint, delta);
                    else
                        ResetConversationState();
                }
            }
        } else
        {
            if (ShouldWalkToSpawnPoint())
                WalkTowards(Spawnpoint, delta);
            else
                ResetConversationState();
        }
    }

    private void HandleIdleState()
    {
        var state = StateData.PickRandomWeighted();
        Subject.TagorPetState = state;

        ActionTimer.Reset();
    }

    private void HandleWanderingState()
    {
        Subject.Wander();

        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            Subject.TagorPetState = TagorPetState.Idle;
            ActionTimer.Reset();
        }

        Subject.WanderTimer.Reset();
    }

    private void ResetConversationState()
    {
        HasPickedAnAisling = false;
        RandomAisling = null;
        FollowUntil = DateTime.MinValue;
        Subject.TagorPetState = TagorPetState.Wandering;
    }

    private bool ShouldWalkTo(Location destination) => (Subject.ManhattanDistanceFrom(destination) > 0) && Subject.OnSameMapAs(destination);

    private bool ShouldWalkToSpawnPoint() => Subject.ManhattanDistanceFrom(Spawnpoint) > 0;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        EatingTimer.Update(delta);

        switch (Subject.TagorPetState)
        {
            case TagorPetState.Idle:
                if (ActionTimer.IntervalElapsed)
                    HandleIdleState();

                break;

            case TagorPetState.Wandering:
                if (Subject.WanderTimer.IntervalElapsed)
                    HandleWanderingState();

                break;

            case TagorPetState.FollowingPlayer:
                HandleFollowingPlayer(delta);

                break;

            case TagorPetState.Eating:
                HandleEatingState();

                break;
        }
    }

    private void UpdateWalkTimer(TimeSpan delta) => WalkTimer.Update(delta);

    private void WalkTowards(Location destination, TimeSpan delta)
    {
        UpdateWalkTimer(delta);

        if (WalkTimer.IntervalElapsed)
            Subject.Pathfind(destination, 0, Options);
    }

    private void WalkTowardsPlayer(Location destination, TimeSpan delta)
    {
        UpdateWalkTimer(delta);

        if (WalkTimer.IntervalElapsed)
            Subject.Pathfind(destination, 0);
    }
}