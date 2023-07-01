using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class KabineBlossomSpawnerScript : ItemSpawnerScript
{

    public override string ItemTemplateKey { get; set; } = "KabineBlossom";
    public override int MaxAmount { get; set; } = 2;
    public override int MaxPerSpawn { get; set; } = 1;
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public KabineBlossomSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}