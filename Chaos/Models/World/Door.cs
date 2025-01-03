#region
using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Models.World;

public sealed class Door(
    bool openRight,
    ushort sprite,
    MapInstance mapInstance,
    IPoint point) : VisibleEntity(sprite, mapInstance, point)
{
    public bool Closed { get; set; } = true;
    public bool OpenRight { get; } = openRight;

    public Door(DoorTemplate doorTemplate, MapInstance mapInstance)
        : this(
            doorTemplate.OpenRight,
            doorTemplate.Sprite,
            mapInstance,
            doorTemplate.Point) { }

    public IEnumerable<Door> GetCluster()
        => MapInstance.GetEntitiesWithinRange<Door>(this)
                      .FloodFill(this);

    public override void HideFrom(Aisling aisling) { }

    public void OnClicked(Merchant source)
    {
        if (!ShouldRegisterClick(source.Id))
            return;

        var doorCluster = GetCluster()
            .ToList();

        foreach (var door in doorCluster)
        {
            door.Closed = !door.Closed;
            door.LastClicked[source.Id] = DateTime.UtcNow;
        }

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this))
            aisling.Client.SendDoors(doorCluster);
    }
    
    public override void OnClicked(Aisling source)
    {
        if (!ShouldRegisterClick(source.Id))
            return;

        var doorCluster = GetCluster()
            .ToList();

        foreach (var door in doorCluster)
        {
            door.Closed = !door.Closed;
            door.LastClicked[source.Id] = DateTime.UtcNow;
        }

        if (doorCluster.All(x => x.Closed))
        {
            MapInstance.PlaySound(166, doorCluster);
        }
        if (doorCluster.All(x => !x.Closed))
        {
            MapInstance.PlaySound(165, doorCluster);
        }
        
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this))
        {
            aisling.Client.SendDoors(doorCluster);
        }
    }
    
    public override bool ShouldRegisterClick(uint fromId)
        => LastClicked.IsEmpty
           || (DateTime.UtcNow.Subtract(LastClicked.Values.Max())
                       .TotalMilliseconds
               > 1500);

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDoors(GetCluster());
}