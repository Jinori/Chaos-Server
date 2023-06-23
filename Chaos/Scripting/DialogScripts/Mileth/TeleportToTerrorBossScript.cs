using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Collections;
using Chaos.Extensions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class TeleportToTerrorBossScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public TeleportToTerrorBossScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        var group = source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();

        if ((group == null) || !group.Any())
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you group your companions for this quest!");
            Subject.Reply(source, "What? You don't have any friends with you.. who are you talking to?");
            return;
        }

        if (!group.All(member => member.WithinLevelRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
            Subject.Reply(source, "Some of your companions are not within your level range.");
            return;
        }

        Subject.Close(source);
        
        var mapInstance = SimpleCache.Get<MapInstance>("cryptTerror");
        var pointS = new Point(13, 8);
        
        foreach (var member in group)
        {
            member.TraverseMap(mapInstance, pointS);
        }
    }
}