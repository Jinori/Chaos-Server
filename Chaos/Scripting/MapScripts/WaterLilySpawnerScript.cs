using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class WaterLilySpawnerScript : ItemSpawnerScript
{

    public override string ItemTemplateKey { get; set; } = "waterlily";
    public override int MaxAmount { get; set; } = 10;
    public override int MaxPerSpawn { get; set; } = 1;
    public override int SpawnIntervalMs { get; set; } = 500000;
    public override List<string> MapsToSpawnItemsOn { get; set; } = new()
    {
        "mehadi_briom", 
        "mehadi_ellama", 
        "mehadi_entrance", 
        "mehadi_heart_east", 
        "mehadi_heart_west", 
        "mehadi_hema", 
        "mehadi_shashi", 
        "mehadi_shoal", 
        "mehadi_tila", 
        "piet_sewer_entrance", 
        "piet_sewer_floor_1", 
        "piet_sewer_floor_2",
        "piet_sewer_floor_3",
        "piet_sewer_floor_4",
        "piet_sewer_floor_5",
        "piet_sewer_floor_6",
        "piet_sewer_floor_7",
        "piet_sewer_floor_8",
        "piet_sewer_floor_9",
        "piet_sewer_floor_10",
        "loures_sewer_entrance", 
        "loures_sewer_floor_1",
        "loures_sewer_floor_2",
        "loures_sewer_floor_3",
        "loures_sewer_floor_4",
        "loures_sewer_floor_5",
        "loures_sewer_floor_6",
        "loures_sewer_floor_7",
        "loures_sewer_floor_8",
        "loures_sewer_floor_9"
    };

        /// <inheritdoc />
    public WaterLilySpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}