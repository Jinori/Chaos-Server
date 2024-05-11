using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
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
        var group = source.Group?.Where(x => x.MapInstance.IsWithinMap(source));

        if (group is null)
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are too nervous to venture alone.");
            Subject.Reply(source, "You must have a group to continue.");
        }

        if (group is not null)
        {
            var groupCount = 0;

            var rectangle = new Rectangle(
                9,
                16,
                2,
                2);

            var enumerable = group as Aisling[] ?? group.ToArray();

            foreach (var member in enumerable)
                if (member.MapInstance.IsWithinMap(source))
                    ++groupCount;
            

            if (groupCount.Equals(enumerable.Length))
            {
                Subject.Close(source);

                foreach (var member in enumerable)
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
            else
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your group must be nearby.");
                Subject.Reply(source, "Your group is not near.");
            }
        }
    }
}