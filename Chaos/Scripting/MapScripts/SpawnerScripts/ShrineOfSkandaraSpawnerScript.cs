using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class ShrineOfSkandaraSpawnerScript : MerchantSpawnerScript
{
    public override int MaxAmount { get; set; } = 10;
    public override int MaxPerSpawn { get; set; } = 2;

    public override string MerchantTemplateKey { get; set; } = "shrineOfSkandara";
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public ShrineOfSkandaraSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory, ISimpleCache simpleCache)
        : base(subject, merchantFactory, simpleCache) { }
}