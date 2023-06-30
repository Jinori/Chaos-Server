using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class PassionFlowerSpawnerScript : ItemSpawnerScript
{

    public override string ItemTemplateKey { get; set; } = "passionflower";
    public override int MaxAmount { get; set; } = 3;
    public override int MaxPerSpawn { get; set; } = 21;
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public PassionFlowerSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}