using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class AcornSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "acorn";
    public override int MaxAmount { get; set; } = 20;
    public override int MaxPerSpawn { get; set; } = 4;
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public AcornSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}