using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class HomeScript : ConfigurableItemScriptBase, TeleportComponent.ITeleportComponentOptions
{
    /// <inheritdoc />
    public string DestinationMapKey { get; set; } = null!;

    /// <inheritdoc />
    public Point OriginPoint { get; set; }

    /// <inheritdoc />
    public ISimpleCache SimpleCache { get; init; }

    /// <inheritdoc />
    public HomeScript(Item subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnUse(Aisling source)
    {
        switch (source.Nation)
        {
            case Nation.Exile:
                OriginPoint = new Point(7, 5);
                DestinationMapKey = "toc";

                break;
            case Nation.Suomi:
                OriginPoint = new Point(5, 8);
                DestinationMapKey = "suomi_inn";

                break;
            case Nation.Ellas:
                OriginPoint = new Point(9, 2);

                break;
            case Nation.Loures:
                OriginPoint = new Point(21, 16);
                DestinationMapKey = "loures_castle";

                break;
            case Nation.Mileth:
                OriginPoint = new Point(5, 8);
                DestinationMapKey = "mileth_inn";

                break;
            case Nation.Tagor:
                OriginPoint = new Point(5, 8);
                DestinationMapKey = "tagor_inn";

                break;
            case Nation.Rucesion:
                OriginPoint = new Point(7, 3);
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
                OriginPoint = new Point(5, 8);
                DestinationMapKey = "abel_inn";

                break;
            case Nation.Undine:
                OriginPoint = new Point(5, 11);
                DestinationMapKey = "undine_village_way";

                break;
            case Nation.Void:
                OriginPoint = new Point(12, 15);
                DestinationMapKey = "arena_entrance";

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        new ComponentExecutor(source, source).WithOptions(this)
                                             .Execute<TeleportComponent>();
        source.Inventory.RemoveQuantity(Subject.Slot, 1);
    }
}