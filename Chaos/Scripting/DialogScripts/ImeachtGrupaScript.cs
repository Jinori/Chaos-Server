using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class ImeachtGrupaScript : DialogScriptBase
{
    private readonly IReactorTileFactory ReactorTileFactory;
    private readonly ISimpleCache SimpleCache;
    public string DestinationMapKey { get; set; } = null!;
    /// <inheritdoc />
    public Point OriginPoint { get; set; }
    /// <inheritdoc />
    public ImeachtGrupaScript(Dialog subject, IReactorTileFactory reactorTileFactory, ISimpleCache simpleCache)
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
            case "imeachtgrupa_portgroup":
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
                            var tile = ReactorTileFactory.Create("summonhomeportal", member.MapInstance, new Point(member.X + 1, member.Y - 1), null, source);
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
                                        "summonhomeportal",
                                        member.MapInstance,
                                        new Point(member.X + 1, member.Y - 1),
                                        null,
                                        source);

                                    member.MapInstance.SimpleAdd(tile);
                                    member.SendActiveMessage($"{source.Name} has created a home portal for you to enter.");
                                });
                    }
                
                switch (source.Nation)
                {
                    case Nation.Exile:
                        OriginPoint = new Point(8, 5);
                        DestinationMapKey = "toc";

                        break;
                    case Nation.Suomi:
                        OriginPoint = new Point(9, 5);
                        DestinationMapKey = "suomi_inn";

                        break;
                    case Nation.Ellas:
                        OriginPoint = new Point(9, 2);

                        break;
                    case Nation.Loures:
                        OriginPoint = new Point(5, 6);
                        DestinationMapKey = "loures_2_floor_empty_room_1";

                        break;
                    case Nation.Mileth:
                        OriginPoint = new Point(4, 8);
                        DestinationMapKey = "mileth_inn";

                        break;
                    case Nation.Tagor:
                        OriginPoint = new Point(4, 8);
                        DestinationMapKey = "tagor_inn";

                        break;
                    case Nation.Rucesion:
                        OriginPoint = new Point(7, 5);
                        DestinationMapKey = "rucesion_inn";

                        break;
                    case Nation.Noes:
                        OriginPoint = new Point(9, 9);

                        break;
                    case Nation.Illuminati:
                        OriginPoint = new Point(9, 10);

                        break;
                    case Nation.Piet:
                        OriginPoint = new Point(5, 8);
                        DestinationMapKey = "piet_inn";

                        break;
                    case Nation.Atlantis:
                        OriginPoint = new Point(9, 12);

                        break;
                    case Nation.Abel:
                        OriginPoint = new Point(4, 7);
                        DestinationMapKey = "abel_inn";

                        break;
                    case Nation.Undine:
                        OriginPoint = new Point(12, 4);
                        DestinationMapKey = "undine_tavern";

                        break;
                    case Nation.Void:
                        OriginPoint = new Point(12, 16);
                        DestinationMapKey = "arena_entrance";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var targetMap = SimpleCache.Get<MapInstance>(DestinationMapKey);
                source.TraverseMap(targetMap, OriginPoint);
                source.Inventory.RemoveQuantity("Imeacht Grupa", 1);
                break;
            }
        }
    }
}