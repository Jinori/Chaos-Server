using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class DochasBloomSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "dochasbloom";
    public override int MaxAmount { get; set; } = 3;
    public override int MaxPerSpawn { get; set; } = 1;
    public override int SpawnIntervalMs { get; set; } = 1800000;
    
    public override int SpawnChance { get; set; } = 10;

    /// <inheritdoc />
    public DochasBloomSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}