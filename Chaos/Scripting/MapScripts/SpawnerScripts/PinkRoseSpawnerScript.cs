using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class PinkRoseSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "pinkrose";
    public override int MaxAmount { get; set; } = 15;
    public override int MaxPerSpawn { get; set; } = 3;

    public override int SpawnChance { get; set; } = 15;
    public override int SpawnIntervalMs { get; set; } = 400000;

    /// <inheritdoc />
    public PinkRoseSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}