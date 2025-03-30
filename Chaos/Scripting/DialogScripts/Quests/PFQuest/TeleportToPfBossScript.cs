using Chaos.Collections;
using Chaos.DarkAges.Definitions;
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
        var group = source.Group?.Where(x => x.WithinRange(point)).ToList();
        
        
        if (source.Group is null || source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");
            var point1 = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point1);

            return;
        }
        
        if (group != null && !group.All(member => member.WithinLevelRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
            Subject.Reply(source, "Some of your companions are not within your level range.");

            return;
        }
        
        var rectangle = new Rectangle(
            9,
            16,
            2,
            2);
        
        foreach (var member in source.Group!)
        {
            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
                var mapInstance = SimpleCache.Get<MapInstance>("pf_peak");

                do
                    point = rectangle.GetRandomPoint();
                while (!mapInstance.IsWalkable(point, collisionType: member.Type));

                member.TraverseMap(mapInstance, point);
        }
    }
}