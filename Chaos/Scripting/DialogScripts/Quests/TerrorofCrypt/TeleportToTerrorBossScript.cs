using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.TerrorofCrypt;

public class TeleportToTerrorBossScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public TeleportToTerrorBossScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        // Check if the group is null or has only one member
        if (source.Group is null || source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source)))
        {
            // Send a message to the Aisling
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");

            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        if (!source.Group.All(member => member.WithinLevelRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
            Subject.Reply(source, "Some of your companions are not within your level range.");

            return;
        }

        Subject.Close(source);

        var mapInstance = SimpleCache.Get<MapInstance>("cryptTerror");
        var pointS = new Point(13, 8);

        foreach (var member in source.Group)
        {
            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            member.TraverseMap(mapInstance, pointS);
        }
    }
}