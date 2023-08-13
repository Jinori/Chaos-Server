using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts;

public class LockpickChestSpawnerScript : MerchantSpawnerScript
{
    public override int MaxAmount { get; set; } = 2;
    public override int MaxPerSpawn { get; set; } = 1;
    public override string MerchantTemplateKey { get; set; } = "lockpickChest";
    public override int MinDistanceFromWall { get; set; } = 2;
    public override int SpawnIntervalMs { get; set; } = 600;

    /// <inheritdoc />
    public LockpickChestSpawnerScript(MapInstance subject, IMerchantFactory merchantFactory)
        : base(subject, merchantFactory) { }
}