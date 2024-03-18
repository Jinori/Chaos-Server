using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.UndineFields
{
    public class StartEastRoomScript : DialogScriptBase
    {
        private readonly ISimpleCache SimpleCache;

        /// <inheritdoc />
        public StartEastRoomScript(Dialog subject, ISimpleCache simpleCache)
            : base(subject) =>
            SimpleCache = simpleCache;

        public override void OnDisplaying(Aisling source)
        {
            var point = new Point(source.X, source.Y);
            var group = source.Group?.Where(x => x.WithinRange(point));

            if (group is null)
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You cannot venture the fields alone.");
                Subject.Reply(source, "You must have a group to continue venturing the fields.");
                return; // Exit early if there is no group
            }

            var rectangle = new Rectangle(22, 36, 2, 2);

            var allGroupMembersNearby = true;

            foreach (var member in group)
            {
                if (!member.MapInstance.IsWithinMap(member))
                {
                    allGroupMembersNearby = false;
                    break; // If any member is not nearby, exit the loop
                }
            }

            if (allGroupMembersNearby)
            {
                Subject.Close(source);

                foreach (var member in group)
                {
                    var mapInstance = SimpleCache.Get<MapInstance>("undine_field_east");

                    Point newPoint;
                    do
                    {
                        newPoint = rectangle.GetRandomPoint();
                    } while (!mapInstance.IsWalkable(newPoint, member.Type));

                    member.Trackers.Counters.Remove("orckills", out _);
                    member.TraverseMap(mapInstance, newPoint);
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
