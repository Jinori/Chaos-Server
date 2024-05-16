using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.KarloposQuest;

public class TeleportToQueenOctopusScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToQueenOctopusScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        if (source.Group is null || source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }
        
        var rectangle = new Rectangle(
            9,
            16,
            2,
            2);


        foreach (var member in source.Group!)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("karloposqueenroom");

            Point point;
            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));

            member.TraverseMap(mapInstance, point);
            member.Trackers.Enums.Set(QueenOctopusQuest.QueenSpawning);
            member.Inventory.Remove("Red Pearl");
            member.Inventory.Remove("Coral Pendant");
        }
    }
}