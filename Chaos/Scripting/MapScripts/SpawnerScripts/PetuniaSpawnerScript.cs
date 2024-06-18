using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class PetuniaSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "petunia";
    public override int MaxAmount { get; set; } = 15;
    public override int MaxPerSpawn { get; set; } = 4;

    public override int SpawnChance { get; set; } = 15;
    public override int SpawnIntervalMs { get; set; } = 350000;

    /// <inheritdoc />
    public PetuniaSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}