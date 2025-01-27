using Chaos.Collections;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

/// <summary>
/// Map script responsible for spawning merchants when an event is active.
/// </summary>
internal class EventMerchantScript : MapScriptBase
{
    private readonly IIntervalTimer EventCheckTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
    private readonly IMerchantFactory MerchantFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventMerchantScript"/> class.
    /// </summary>
    public EventMerchantScript(MapInstance subject, IMerchantFactory merchantFactory) : base(subject)
    {
        MerchantFactory = merchantFactory;
    }

    /// <summary>
    /// Dictionary mapping map IDs to the merchants that should spawn during events.
    /// </summary>
    private static readonly Dictionary<string, List<MerchantSpawn>> MerchantSpawns = new()
    {
        { "loures_castle_way", [ new MerchantSpawn("aidan", 8, 2, Direction.Down), new MerchantSpawn("nadia", 7, 2, Direction.Down) ] },
        { "elf_room", [ new MerchantSpawn("elf2", 5, 4, Direction.Down) ] },
        { "lift_room", [ new MerchantSpawn("elf4", 3, 2, Direction.Down) ] },
        { "mtmerry_frostychallenge", [ new MerchantSpawn("elf5", 4, 16, Direction.Right) ] },
        { "mtmerry_northpole", [ new MerchantSpawn("christmastree", 17, 17, Direction.Down), new MerchantSpawn("mothererbie", 14, 8, Direction.Down) ] },
        { "reindeer_pen", [ new MerchantSpawn("elf3", 5, 7, Direction.Right) ] },
        { "santas_room", [ new MerchantSpawn("santa", 3, 6, Direction.Down) ] },
        { "toy_shop", [ new MerchantSpawn("elf1", 6, 6, Direction.Down) ] }
    };

    public override void Update(TimeSpan delta)
    {
        EventCheckTimer.Update(delta);
        if (!EventCheckTimer.IntervalElapsed) return;

        TrySpawnMerchantsForActiveEvent();
    }

    /// <summary>
    /// Checks for an active event on the current map and spawns merchants if needed.
    /// </summary>
    private void TrySpawnMerchantsForActiveEvent()
    {
        var mapId = Subject.InstanceId;
        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, mapId);
        
        if (!isEventActive || !MerchantSpawns.TryGetValue(mapId, out var merchants))
            return;

        foreach (var merchant in merchants)
        {
            if (Subject.GetEntities<Merchant>().Any(x => x.Template.TemplateKey == merchant.MerchantId))
                continue;

            SpawnMerchant(merchant);
        }
    }

    /// <summary>
    /// Spawns a merchant at a specific location and direction based on the dictionary.
    /// </summary>
    /// <param name="merchant">The merchant to spawn.</param>
    private void SpawnMerchant(MerchantSpawn merchant)
    {
        var merchantPoint = new Point(merchant.X, merchant.Y);
        var merchantToSpawn = MerchantFactory.Create(merchant.MerchantId, Subject, merchantPoint);
        
        merchantToSpawn.Direction = merchant.Direction;
        Subject.AddEntity(merchantToSpawn, merchantPoint);
    }
}

/// <summary>
/// Represents a merchant's spawn details.
/// </summary>
public record MerchantSpawn(string MerchantId, int X, int Y, Direction Direction);
