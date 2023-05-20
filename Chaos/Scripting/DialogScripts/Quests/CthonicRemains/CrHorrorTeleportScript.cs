using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.CthonicRemains;

public class CrHorrorTeleportScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public CrHorrorTeleportScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;
    public override void OnDisplaying(Aisling source)
    {
        var point = new Point(source.X, source.Y);
        var group = source.Group?.Where(x => x.WithinRange(point));

        if (group is null)
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you group your companions for this quest!");
            Subject.Reply(source, "What? You don't have any friends with you.. who are you talking to?");
        }

        if (group is not null)
        {
            var groupCount = 0;

            foreach (var member in group)
                if (member.WithinLevelRange(source))
                    ++groupCount;

            if (groupCount.Equals(group.Count()))
            {
                Subject.Close(source);

                foreach (var member in group)
                {
                    var mapInstance = SimpleCache.Get<MapInstance>("cthonic_domain");
                    var pointS = new Point(22, 15);
                    member.TraverseMap(mapInstance, pointS);
                }
            } else
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
                Subject.Reply(source, "Some of your companions are not within your level range.");
            }
        }
    }
}