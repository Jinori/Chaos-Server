using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusHomeOptionsScript : DialogScriptBase
{
    private readonly ISimpleCache _simpleCache;
    
    /// <inheritdoc />
    public TerminusHomeOptionsScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) => _simpleCache = simpleCache;

    private static readonly Dictionary<Nation, (IPoint Origin, string? DestinationMapKey)> NATION_MAPPINGS =
        new()
        {
            { Nation.Exile, (new Point(8, 5), "toc") },
            { Nation.Suomi, (new Point(9, 5), "suomi_inn") },
            { Nation.Ellas, (new Point(9, 2), null) },
            { Nation.Loures, (new Point(5, 6), "loures_2_floor_empty_room_1") },
            { Nation.Mileth, (new Point(4, 8), "mileth_inn") },
            { Nation.Tagor, (new Point(4, 8), "tagor_inn") },
            { Nation.Rucesion, (new Point(7, 5), "rucesion_inn") },
            { Nation.Noes, (new Point(9, 9), null) },
            { Nation.Illuminati, (new Point(9, 10), null) },
            { Nation.Piet, (new Point(5, 8), "piet_inn") },
            { Nation.Atlantis, (new Point(9, 12), null) },
            { Nation.Abel, (new Point(4, 7), "abel_inn") },
            { Nation.Undine, (new Point(12, 4), "undine_tavern") },
            { Nation.Labyrinth, (new Point(6, 7), "arena_entrance") }
        };

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (source.Guild == null) return;

        var option = new DialogOption
        {
            DialogKey = "terminus_homeoptions",
            OptionText = "Guild Hall",
        };

        if (!Subject.HasOption(option.OptionText))
            Subject.Options.Add(option);
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!optionIndex.HasValue)
            return;
        

        switch (optionIndex)
        {
            case 3:
            {
                var map = _simpleCache.Get<MapInstance>("guildhallmain");
                source.TraverseMap(map, new Point(98, 46));
                return;
            }
            case 2:
            {
                NATION_MAPPINGS.TryGetValue(source.Nation, out var location);
                if (location.DestinationMapKey is not null)
                {
                    var map = _simpleCache.Get<MapInstance>(location.DestinationMapKey);
                    source.TraverseMap(map, location.Origin);
                }

                break;
            }
        }
    }
}
