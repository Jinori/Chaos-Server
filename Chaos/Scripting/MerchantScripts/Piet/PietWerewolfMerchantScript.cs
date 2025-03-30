using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Piet;

public class PietWerewolfMerchantScript : MerchantScriptBase
{
    public enum PietWerewolfState
    {
        Idle,
        Wandering,
        SeenByAislingWithEnum
    }

    private IPathOptions Options => PathOptions.Default.ForCreatureType(Subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true
    };



    private readonly IIntervalTimer ActionTimer;
    public readonly Location PietEmptyRoomDoorPoint = new("piet", 30, 12);

    private readonly IIntervalTimer WalkTimer;
    private readonly IIntervalTimer FasterWalkTimer;

    /// <inheritdoc />
    public PietWerewolfMerchantScript(Merchant subject)
        : base(subject)
    {
        FasterWalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.PietWerewolfState = PietWerewolfState.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;
    public bool HasSaidDialog4;
    
    private void HandleApproachToEmptyRoom(TimeSpan delta)
    {
        if (ShouldWalkTo(PietEmptyRoomDoorPoint))
            WalkTowards(PietEmptyRoomDoorPoint, delta);
    }
    
    private bool ShouldWalkTo(Location destination) =>
        (Subject.ManhattanDistanceFrom(destination) > 0) && Subject.OnSameMapAs(destination);
    
    private void AttemptToOpenDoor(IPoint doorPoint)
    {
        var door = Subject.MapInstance.GetEntitiesAtPoints<Door>(doorPoint).FirstOrDefault();

        if (door is { Closed: true })
            door.OnClicked(Subject);
    }

    private void HandleIdleState()
    {
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
            Subject.PietWerewolfState = PietWerewolfState.SeenByAislingWithEnum;

        Subject.PietVillagerState = PietVillagerScript.PietVillagerState.Wandering;

        ActionTimer.Reset();
    }

    private void HandleWanderingState()
    {
        Subject.Wander();

        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
            Subject.PietWerewolfState = PietWerewolfState.SeenByAislingWithEnum;

        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            Subject.PietWerewolfState = PietWerewolfState.Idle;
            ActionTimer.Reset();
        }

        Subject.WanderTimer.Reset();
    }

    private void HandleSeenByAislingWithEnumState(TimeSpan delta)
    {
        if (Subject.WithinRange(PietEmptyRoomDoorPoint, 1))
        {
            var aislingsOnQuest = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
                .Where(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)).ToList();

            foreach (var aisling in aislingsOnQuest)
            {
                // Assign the FollowedMerchant enum to the aisling
                aisling.Trackers.Enums.Set(WerewolfOfPiet.FollowedMerchant);
                aisling.SendOrangeBarMessage("That must be the werewolf's hut!");

                // Check if aisling is in a group
                if (aisling.Group != null)
                {
                    // Get group members who are on the map and have started the quest
                    var groupMembers = aisling.Group
                        .Where(member => member.MapInstance == Subject.MapInstance &&
                                         member.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest))
                        .ToList();

                    // Assign the FollowedMerchant enum to group members
                    foreach (var member in groupMembers)
                    {
                        member.Trackers.Enums.Set(WerewolfOfPiet.FollowedMerchant);
                        member.SendOrangeBarMessage("That must be the werewolf's hut!");
                    }
                }
            }

            Subject.MapInstance.RemoveEntity(Subject);
        }
        HandleApproachToEmptyRoom(delta);
    }

    private void WalkTowards(Location destination, TimeSpan delta)
    {
        WalkTimer.Update(delta);
        FasterWalkTimer.Update(delta);
        
        if (FasterWalkTimer.IntervalElapsed && Subject.MapInstance.Name == "Piet")
        {
            if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 4)
                .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
            {
                Subject.Pathfind(destination, 0, Options);
            }
        }
        
        if (WalkTimer.IntervalElapsed)
        {
            if (destination == PietEmptyRoomDoorPoint)
                AttemptToOpenDoor(PietEmptyRoomDoorPoint);

            Subject.Pathfind(destination, 0, Options);
        }
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        
        switch (Subject.PietWerewolfState)
        {
            case PietWerewolfState.Idle:
            {
                HandleIdleState();
                break;
            }

            case PietWerewolfState.Wandering:
            {
                HandleWanderingState();
                break;
            }

            case PietWerewolfState.SeenByAislingWithEnum:
            {
                HandleSeenByAislingWithEnumState(delta);
                break;
            }
        }
    }
}