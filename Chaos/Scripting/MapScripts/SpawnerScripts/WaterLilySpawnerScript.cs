using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class WaterLilySpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "waterlily";
    public override int MaxAmount { get; set; } = 15;
    public override int MaxPerSpawn { get; set; } = 3;

    public override int SpawnChance { get; set; } = 12;
    public override int SpawnIntervalMs { get; set; } = 350000;

    /// <inheritdoc />
    public WaterLilySpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}