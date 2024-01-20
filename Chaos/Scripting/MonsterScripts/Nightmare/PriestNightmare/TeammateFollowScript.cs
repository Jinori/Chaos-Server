using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

public sealed class TeammateFollowScript : MonsterScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    /// <inheritdoc />
    public TeammateFollowScript(Monster subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject) =>
        ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public override void OnItemDroppedOn(Aisling source, Item item)
    {
        if ((Subject.PetOwner != null) && (Subject.PetOwner.Equals(source)))
        {
            source.SendMessage("You dropped an item on your pet but they returned it.");
            source.Inventory.TryAddToNextSlot(item);
        }
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Subject.PetOwner is null)
            return;

        
        if (ClientRegistry.GetClient(Subject.PetOwner.Client.Id) is null)
        {
            Subject.MapInstance.RemoveEntity(Subject);

            return;
        }

        if (Subject.MapInstance != Subject.PetOwner.MapInstance)
        {
            Subject.TraverseMap(Subject.PetOwner.MapInstance, Subject.PetOwner);

            return;
        }

        if (!ShouldMove || (Target != null))
            return;

        var playerDistance = Subject.PetOwner.DistanceFrom(Subject);

        switch (playerDistance)
        {
            case <= 4:
                Subject.Wander();

                break;
            case > 4 and < 13:
                Subject.Pathfind(new Point(Subject.PetOwner.X, Subject.PetOwner.Y));

                break;
            case >= 13:
                Subject.WarpTo(Subject.PetOwner);

                break;
        }

        Subject.MoveTimer.Reset();
    }

    /*
        DateTime now;
        private const bool PET_LOOT = false;
        const double TIME_SPAN_SECONDS = 3;
        if (PET_LOOT)
        {
            var item = Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(Subject.PetOwner)
                              .FirstOrDefault(x => now - x.Creation > TimeSpan.FromSeconds(TIME_SPAN_SECONDS));

            if (item is not null)
                return;
        }
*/
}