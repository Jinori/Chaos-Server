using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class HomeScript(Spell subject, ISimpleCache simpleCache) : ConfigurableSpellScriptBase(subject),
                                                                   TeleportComponent.ITeleportComponentOptions,
                                                                   ManaCostAbilityComponent.IManaCostComponentOptions
{
    /// <inheritdoc />
    public string DestinationMapKey { get; set; } = null!;
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public Point OriginPoint { get; set; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public ISimpleCache SimpleCache { get; init; } = simpleCache;

    public override void OnUse(SpellContext context)
    {
        if (context.SourceAisling is null)
            return;

        switch (context.SourceAisling.Nation)
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

        new ComponentExecutor(context).WithOptions(this).ExecuteAndCheck<ManaCostAbilityComponent>()?.Execute<TeleportComponent>();
    }
}