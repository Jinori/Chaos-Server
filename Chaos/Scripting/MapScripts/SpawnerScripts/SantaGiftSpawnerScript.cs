using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class SantaGiftSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "santagift";
    public override int MaxAmount { get; set; } = 2;
    public override int MaxPerSpawn { get; set; } = 1;
    public override int SpawnChance { get; set; } = 25;
    public override int SpawnIntervalMs { get; set; } = 500000;

    /// <inheritdoc />
    public SantaGiftSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}