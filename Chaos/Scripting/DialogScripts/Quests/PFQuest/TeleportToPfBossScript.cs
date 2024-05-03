using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PFQuest;

public class TeleportToPfBossScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToPfBossScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        var point = new Point(source.X, source.Y);
        var point1 = point;
        var group = source.Group?.Where(x => x.WithinRange(point1));

        if (group is null)
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are too nervous to venture onto the peak alone.");
            Subject.Reply(source, "You must have a group to get onto the peak.");
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
                if (member.WithinRange(point, 10))
                    ++groupCount;

            if (groupCount.Equals(enumerable.Count()))
            {
                Subject.Close(source);

                foreach (var member in enumerable)
                {
                    var mapInstance = SimpleCache.Get<MapInstance>("pf_peak");

                    do
                        point = rectangle.GetRandomPoint();
                    while (!mapInstance.IsWalkable(point, member.Type));

                    member.TraverseMap(mapInstance, point);
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