using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.Components;

public class TeleportComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ITeleportComponentOptions>();
        
        var mapInstance = options.SimpleCache.Get<MapInstance>(options.DestinationMapKey);
        var origin = options.OriginPoint;
        var possiblePoints = mapInstance.Template.Bounds
                                        .GetPoints()
                                        .Where(pt => mapInstance.IsWalkable(pt, CreatureType.Aisling) && !mapInstance.IsBlockingReactor(pt))
                                        .FloodFill(origin)
                                        .ToList();

        var targetPoint = possiblePoints.PickRandom();
        context.SourceAisling?.TraverseMap(mapInstance, targetPoint);
    }
    
    public interface ITeleportComponentOptions
    {
        ISimpleCache SimpleCache { get; init; }
        string DestinationMapKey { get; set; }
        Point OriginPoint { get; set; }
    }
}