using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class KoboldTailSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "KoboldTail";
    public override int MaxAmount { get; set; } = 4;
    public override int MaxPerSpawn { get; set; } = 1;

    public override int SpawnChance { get; set; } = 5;
    public override int SpawnIntervalMs { get; set; } = 800000;

    /// <inheritdoc />
    public KoboldTailSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}