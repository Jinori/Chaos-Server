using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class BocanBoughSpawnerScript : ItemSpawnerScript
{

    public override string ItemTemplateKey { get; set; } = "bocanbough";
    public override int MaxAmount { get; set; } = 30;
    public override int MaxPerSpawn { get; set; } = 1;
    public override int SpawnIntervalMs { get; set; } = 600000;
    public override List<string> MapsToSpawnItemsOn { get; set; } = new()
    {
        "mileth", "mileth_village_way", "abel", "astrid_center", "astrid_entrance", "astrid_north", "astrid_north_east",
        "astrid_north_west", "astrid_south", "astrid_south_west", "astrid_south_east", "crossroads", "east_woodlands1",
        "east_woodlands2", "east_woodlands3", "east_woodlands4", "east_woodlands5", "east_woodlands6",
        "east_woodlands7", "east_woodlands8", "east_woodlands9", "east_woodlands10", "enchanted_garden",
        "loures_castle", "loures_castle_way", "mythic", "piet", "rucesion", "suomi", "tagor", "pf_entrance", "pf_1_l",
        "pf_1_r", "pf_2_c", "pf_2_l", "pf_2_r", "pf_3_c", "pf_3_r", "pf_3_l", "pf_4_c", "pf_4_r", "pf_4_l", "undine","wilderness","west_woodlands1","west_woodlands2","west_woodlands3",
        "west_woodlands4","west_woodlands5","west_woodlands6","west_woodlands7","undine_field_arena","undine_field_east","undine_field_entrance","undine_field_north","undine_field_south",
        "undine_field_west","sapphire_stream_pass",
    };

        /// <inheritdoc />
    public BocanBoughSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}