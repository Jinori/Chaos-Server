using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Christmas;

public class TeleportToMtMerryScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToMtMerryScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source) => TeleportPlayerToMtMerry(source);

    private void TeleportPlayerToMtMerry(Aisling source)
    {
        var rectangle = new Rectangle(
            22,
            12,
            5,
            5);
        var mapInstance = SimpleCache.Get<MapInstance>("mtmerry_northpole");

        Subject.Close(source);

        Point point;

        do
            point = rectangle.GetRandomPoint();
        while (!mapInstance.IsWalkable(point, source.Type));

        source.TraverseMap(mapInstance, point);
    }
}