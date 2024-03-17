using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class LilyPadSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "LilyPad";
    public override int MaxAmount { get; set; } = 4;
    public override int MaxPerSpawn { get; set; } = 2;

    public override int SpawnChance { get; set; } = 10;
    public override int SpawnIntervalMs { get; set; } = 500000;

    /// <inheritdoc />
    public LilyPadSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}