using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class ShrineOfSerendaelSpawnerScript : MerchantSpawnerScript
{
    public override int MaxAmount { get; set; } = 10;
    public override int MaxPerSpawn { get; set; } = 2;
    public override int MinDistanceFromWall { get; set; } = 3;
    public override string MerchantTemplateKey { get; set; } = "shrineOfSerendael";
    public override int SpawnIntervalMs { get; set; } = 600000;

    /// <inheritdoc />
    public ShrineOfSerendaelSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory)
        : base(subject, merchantFactory) { }
}