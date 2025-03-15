using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts.Events.StPatricks;

public class PotofGoldCharmSpawnerScript : ItemSpawnerScript
{
    public override void Update(TimeSpan delta)
    {
        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.InstanceId);

        if (isEventActive)
            base.Update(delta);
    }

    
    public override string ItemTemplateKey { get; set; } = "potofgoldcharm";
    public override int MaxAmount { get; set; } = 10;
    public override int MaxPerSpawn { get; set; } = 3;
    public override int SpawnChance { get; set; } = 60;
    public override int SpawnIntervalMs { get; set; } = 500000;

    /// <inheritdoc />
    public PotofGoldCharmSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }
}