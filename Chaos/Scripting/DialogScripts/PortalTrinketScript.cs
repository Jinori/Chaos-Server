using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class PortalTrinketScript : DialogScriptBase
{
    private readonly IReactorTileFactory ReactorTileFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public PortalTrinketScript(Dialog subject, IReactorTileFactory reactorTileFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "portaltrinket_summongroup":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("portalTrinket", out var repairTime))
                {
                    Subject.Reply(source, $"You must wait {repairTime.Remaining.ToReadableString()} before you can summon your group again.");
                    return;
                }
                
                if (source.Group is null)
                {
                    Subject.Reply(source, "You are not in a group!");
                    return;
                }

                if (source.MapInstance.IsShard)
                {
                    Subject.Reply(source, "This location is unique and you cannot summon group members here.");
                    return;
                }
                
                foreach (var member in source.Group)
                {
                    if (member.Id != source.Id)
                    {
                        var tile = ReactorTileFactory.Create("summonportal", member.MapInstance, new Point(member.X + 1, member.Y - 1), null, source);
                        member.MapInstance.SimpleAdd(tile);
                        member.SendActiveMessage($"{source.Name} has created a group portal for you to enter.");
                    }
                }
                source.Trackers.TimedEvents.AddEvent("portalTrinket", TimeSpan.FromMinutes(30), true);
                break;
            }

            case "portaltrinket_portalhaven":
            {
                var targetMap = SimpleCache.Get<MapInstance>("undine");
                source.TraverseMap(targetMap, new Point(47, 42));
                break;
            }
        }
    }
}