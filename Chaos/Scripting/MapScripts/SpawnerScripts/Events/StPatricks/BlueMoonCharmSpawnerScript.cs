using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.SpawnerScripts.Events.StPatricks;

public class BlueMoonCharmSpawnerScript : ItemSpawnerScript
{
    public override string ItemTemplateKey { get; set; } = "bluemooncharm";
    public override int MaxAmount { get; set; } = 10;
    public override int MaxPerSpawn { get; set; } = 3;
    public override int SpawnChance { get; set; } = 60;
    public override int SpawnIntervalMs { get; set; } = 500000;

    /// <inheritdoc />
    public BlueMoonCharmSpawnerScript(MapInstance subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject, itemFactory, simpleCache) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.InstanceId);

        if (isEventActive)
            base.Update(delta);
    }
}