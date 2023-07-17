using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class BlossomofBetrayalSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "blossomofbetrayal";
    public override int MaxAmount { get; set; } = 3;
    public override int MaxPerSpawn { get; set; } = 1;
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public BlossomofBetrayalSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}