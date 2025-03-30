using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

// This script is added to maps that are created as dedicated shards
// this shard belongs to another map
// if someone relogs on this map, they will be teleported to the map that this shard belongs to
// these maps still need sharding options and an exit location, incase the shard no longer exists when the person relogs
public class DedicatedShardScript : MapScriptBase
{
    private readonly IIntervalTimer CheckTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
    private readonly ISimpleCache SimpleCache;
    public Location FromLocation { get; set; } = null!;
    public List<string> WhiteList { get; set; } = [];

    /// <inheritdoc />
    public DedicatedShardScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        CheckTimer.Update(delta);

        if (CheckTimer.IntervalElapsed)
        {
            var aislings = Subject.GetEntities<Aisling>()
                                  .ExceptBy(WhiteList, aisling => aisling.Name);

            var owningMapInstance = SimpleCache.Get<MapInstance>(FromLocation.Map);

            foreach (var aisling in aislings)
                aisling.TraverseMap(owningMapInstance, FromLocation, true);
        }
    }
}