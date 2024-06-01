using Chaos.Collections;
using Chaos.Common.Utilities;
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
                    Subject.Reply(source, $"The arcane energies need time to replenish. You must wait {repairTime.Remaining.ToReadableString()} before you can call upon your group once more.");
                    return;
                }
                
                if (source.Group is null)
                {
                    Subject.Reply(source, "You stand alone, unbound by the ties of a group.");
                    return;
                }

                if (source.MapInstance.IsShard)
                {
                    Subject.Reply(source, "The mystical nature of this location prevents any external interference. You are unable to summon your group members to this place.");
                    return;
                }
                
                foreach (var member in source.Group)
                    if (!member.Equals(source))
                    {
                        if (source.MapInstance == member.MapInstance)
                        {
                            var tile = ReactorTileFactory.Create("summonportal", member.MapInstance, new Point(member.X + 1, member.Y - 1), null, source);
                            member.MapInstance.SimpleAdd(tile);
                            member.SendActiveMessage($"{source.Name} has created a group portal for you to enter.");   
                        }
                        else
                            Task.Run(
                                async () =>
                                {
                                    await using var sync = await ComplexSynchronizationHelper.WaitAsync(
                                        TimeSpan.FromMilliseconds(500),
                                        TimeSpan.FromMilliseconds(300),
                                        source.MapInstance.Sync,
                                        member.MapInstance.Sync);

                                    var tile = ReactorTileFactory.Create(
                                        "summonportal",
                                        member.MapInstance,
                                        new Point(member.X + 1, member.Y - 1),
                                        null,
                                        source);

                                    member.MapInstance.SimpleAdd(tile);
                                    member.SendActiveMessage($"{source.Name} has created a group portal for you to enter.");
                                });
                    }

                source.Trackers.TimedEvents.AddEvent("portalTrinket", TimeSpan.FromHours(2), true);
                break;
            }

            case "portaltrinket_portalhaven":
            {
                var targetMap = SimpleCache.Get<MapInstance>("undine");
                source.TraverseMap(targetMap, new Point(47, 42));
                source.SendActiveMessage("The orb has taken you back to the Enchanter's Haven.");   
                break;
            }
        }
    }
}