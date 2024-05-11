using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.KarloposQuest;

public class TeleportToTrialScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportToTrialScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        var group = source.Group?.Where(x => x.MapInstance.IsWithinMap(source));

        if (group is null)
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are missing your group.");
            Subject.Reply(source, "You must have a group to enter the next trial.");
        }

        if (group is not null)
        {
            var groupCount = 0;
            var mapInstance = source.MapInstance.InstanceId;
            var nextInstance = SimpleCache.Get<MapInstance>("karlopostrap");
            Rectangle currentTrial;

            // Check the current map and set the corresponding trial rectangle
            switch (mapInstance.ToLower())
            {
                case "karlopostrap":
                    currentTrial = new Rectangle(3, 7, 3, 2);
                    nextInstance = SimpleCache.Get<MapInstance>("karloposfirsttrial");
                    break;
                case "karloposfirsttrial":
                    currentTrial = new Rectangle(9, 11, 3, 2);
                    nextInstance = SimpleCache.Get<MapInstance>("karlopossecondtrial");
                    break;
                case "karlopossecondtrial":
                    currentTrial = new Rectangle(9, 9, 3, 2);
                    nextInstance = SimpleCache.Get<MapInstance>("karloposthirdtrial");
                    break;
                case "karloposthirdtrial":
                    currentTrial = new Rectangle(7, 11, 3, 2);
                    nextInstance = SimpleCache.Get<MapInstance>("karloposfinaltrial");
                    break;
                case "karloposfinaltrial":
                    currentTrial = new Rectangle(6, 9, 3, 2);
                    nextInstance = SimpleCache.Get<MapInstance>("karlopospendant");
                    break;
                default:
                    // Set a default trial rectangle or handle the case when the map is not recognized
                    currentTrial = new Rectangle(0, 0, 1, 1);
                    break;
            }

            var enumerable = group as Aisling[] ?? group.ToArray();

            foreach (var member in enumerable)
                if (member.MapInstance == source.MapInstance)
                    ++groupCount;

            if (groupCount.Equals(enumerable.Length))
            {
                Subject.Close(source);

                foreach (var member in enumerable)
                {
                    Point point;
                    do
                        point = currentTrial.GetRandomPoint();
                    while (!nextInstance.IsWalkable(point, member.Type));

                    member.Trackers.Counters.Remove("karlopostrialkills", out _);
                    member.TraverseMap(nextInstance, point);
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