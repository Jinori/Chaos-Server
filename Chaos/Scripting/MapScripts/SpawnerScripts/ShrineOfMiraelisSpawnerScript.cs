using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class ShrineOfMiraelisSpawnerScript : MerchantSpawnerScript
{
    public override int MaxAmount { get; set; } = 1;
    public override int MaxPerSpawn { get; set; } = 1;
    public override string MerchantTemplateKey { get; set; } = "shrineOfMiraelis";
    public override int MinDistanceFromWall { get; set; } = 3;
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public ShrineOfMiraelisSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory)
        : base(subject, merchantFactory) { }
}