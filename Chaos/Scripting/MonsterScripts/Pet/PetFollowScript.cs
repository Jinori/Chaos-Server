using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public sealed class PetFollowScript : MonsterScriptBase
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

    /// <inheritdoc />
    public PetFollowScript(Monster subject, IClientRegistry<IChaosWorldClient> clientRegistry)
        : base(subject)
        => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public override void OnItemDroppedOn(Aisling source, Item item)
    {
        if (Subject.PetOwner == null)
            return;

        source.SendMessage("You dropped an item on a pet but they returned it.");
        source.GiveItemOrSendToBank(item);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Subject.PetOwner == null) || (ClientRegistry.GetClient(Subject.PetOwner.Client.Id) == null))
        {
            Subject.MapInstance.RemoveEntity(Subject);

            return;
        }

        if (Subject.MapInstance != Subject.PetOwner.MapInstance)
        {
            Subject.TraverseMap(Subject.PetOwner.MapInstance, Subject.PetOwner);

            return;
        }

        if (Subject.PetOwner.Group is not null)
            if (Subject.PetOwner.Group?.Count > 2)
            {
                Subject.MapInstance.RemoveEntity(Subject);
                Subject.PetOwner.SendOrangeBarMessage("Your pet has gone home due to too many group members.");
                Subject.PetOwner.Trackers.TimedEvents.AddEvent("PetReturn", TimeSpan.FromMinutes(5), true);

                return;
            }

        var playerDistance = Subject.PetOwner.ManhattanDistanceFrom(Subject);
        Subject.PetOwner.Trackers.Enums.TryGetValue(out PetFollowMode value);

        if (playerDistance > 13)
        {
            Subject.TraverseMap(Subject.PetOwner.MapInstance, Subject.PetOwner);

            return;
        }

        if (!ShouldMove || (Target != null))
            return;

        switch (value)
        {
            case PetFollowMode.AtFeet:
                if (playerDistance > 2)
                    Subject.Pathfind(new Point(Subject.PetOwner.X, Subject.PetOwner.Y));

                break;

            case PetFollowMode.Wander:
                if (playerDistance <= 4)
                    Subject.Wander();
                else
                    Subject.Pathfind(new Point(Subject.PetOwner.X, Subject.PetOwner.Y));

                break;

            case PetFollowMode.DontMove:
                break;

            case PetFollowMode.FollowAtDistance:
                if (playerDistance > 6)
                    Subject.Pathfind(new Point(Subject.PetOwner.X, Subject.PetOwner.Y), 6);

                break;
        }

        Subject.MoveTimer.Reset();
    }
}