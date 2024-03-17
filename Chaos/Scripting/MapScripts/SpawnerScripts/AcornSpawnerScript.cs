using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class AcornSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "acorn";
    public override int MaxAmount { get; set; } = 10;
    public override int MaxPerSpawn { get; set; } = 3;
    public override int SpawnChance { get; set; } = 60;
    public override int SpawnIntervalMs { get; set; } = 500000;

    /// <inheritdoc />
    public AcornSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}