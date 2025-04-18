using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

/// <summary>
///     Map script responsible for spawning and removing merchants based on active events.
/// </summary>
internal class EventMerchantScript : MapScriptBase
{
    /// <summary>
    ///     Dictionary mapping map IDs to the merchants that should spawn during events.
    /// </summary>
    private static readonly Dictionary<string, List<MerchantSpawn>> MerchantSpawns = new()
    {
        {
            "loures_3_floor_magic_room", [
                                             new MerchantSpawn(
                                                 "zappy",
                                                 2,
                                                 3,
                                                 Direction.Down)
                                         ]
        },
        {
            "loures_3_floor_office", [
                                         new MerchantSpawn(
                                             "tricksylovefool",
                                             4,
                                             4,
                                             Direction.Up)
                                     ]
        },
        {
            "loures_2_floor_restaurant", [
                                             new MerchantSpawn(
                                                 "flourentine",
                                                 13,
                                                 2,
                                                 Direction.Right)
                                         ]
        },
        {
            "shinewood_forest_entrance", [
                                             new MerchantSpawn(
                                                 "cueti",
                                                 16,
                                                 8,
                                                 Direction.Down)
                                         ]
        },
        {
            "loures_castle_way", [
                                     new MerchantSpawn(
                                         "aidan",
                                         8,
                                         2,
                                         Direction.Down),
                                     new MerchantSpawn(
                                         "nadia",
                                         7,
                                         2,
                                         Direction.Down)
                                 ]
        },
        {
            "elf_room", [
                            new MerchantSpawn(
                                "elf2",
                                5,
                                4,
                                Direction.Down)
                        ]
        },
        {
            "lift_room", [
                             new MerchantSpawn(
                                 "elf4",
                                 3,
                                 2,
                                 Direction.Down)
                         ]
        },
        {
            "mtmerry_frostychallenge", [
                                           new MerchantSpawn(
                                               "elf5",
                                               4,
                                               16,
                                               Direction.Right)
                                       ]
        },
        {
            "mtmerry_northpole", [
                                     new MerchantSpawn(
                                         "christmastree",
                                         17,
                                         17,
                                         Direction.Down),
                                     new MerchantSpawn(
                                         "mothererbie",
                                         14,
                                         8,
                                         Direction.Down)
                                 ]
        },
        {
            "reindeer_pen", [
                                new MerchantSpawn(
                                    "elf3",
                                    5,
                                    7,
                                    Direction.Right)
                            ]
        },
        {
            "santas_room", [
                               new MerchantSpawn(
                                   "santa",
                                   3,
                                   6,
                                   Direction.Down)
                           ]
        },
        {
            "toy_shop", [
                            new MerchantSpawn(
                                "elf1",
                                6,
                                6,
                                Direction.Down)
                        ]
        },
        {
            "rucesion", [
                            new MerchantSpawn(
                                "slytherin",
                                32,
                                35,
                                Direction.Down)
                        ]
        },
        {
            "mileth_village_way", [
                                      new MerchantSpawn(
                                          "lucky",
                                          5,
                                          9,
                                          Direction.Down)
                                  ]
        },
        {
            "mileth", [
                          new MerchantSpawn(
                              "silasbunnium",
                              56,
                              19,
                              Direction.Up)
                      ]
        },
        {
            "undine", [
                          new MerchantSpawn(
                              "cadburry",
                              20,
                              21,
                              Direction.Down),
                          new MerchantSpawn(
                              "shopscotch",
                              17,
                              28,
                              Direction.Right),
                          new MerchantSpawn(
                              "nugget",
                              22,
                              29,
                              Direction.Right)
                      ]
        }
    };

    private readonly IIntervalTimer EventCheckTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
    private readonly IMerchantFactory MerchantFactory;

    /// <summary>
    ///     Initializes a new instance of the class.
    /// </summary>
    public EventMerchantScript(MapInstance subject, IMerchantFactory merchantFactory)
        : base(subject)
        => MerchantFactory = merchantFactory;

    /// <summary>
    ///     Manages merchant spawning and removal based on event status.
    /// </summary>
    private void ManageEventMerchants()
    {
        var mapId = Subject.InstanceId;
        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, mapId);

        if (!MerchantSpawns.TryGetValue(mapId, out var merchants))
            return;

        if (isEventActive)
            SpawnMissingMerchants(merchants);
        else
            RemoveEventMerchants(merchants);
    }

    /// <summary>
    ///     Removes merchants from the map if the event is no longer active.
    /// </summary>
    private void RemoveEventMerchants(List<MerchantSpawn> merchants)
    {
        var merchantsOnMap = Subject.GetEntities<Merchant>()
                                    .ToList();

        foreach (var merchant in merchantsOnMap.Where(merchant => merchants.Any(m => m.MerchantId.EqualsI(merchant.Template.TemplateKey))))
            Subject.RemoveEntity(merchant);
    }

    /// <summary>
    ///     Spawns a merchant at a specific location and direction based on the dictionary.
    /// </summary>
    private void SpawnMerchant(MerchantSpawn merchant)
    {
        var merchantPoint = new Point(merchant.X, merchant.Y);
        var merchantToSpawn = MerchantFactory.Create(merchant.MerchantId, Subject, merchantPoint);

        merchantToSpawn.Direction = merchant.Direction;
        Subject.AddEntity(merchantToSpawn, merchantPoint);
    }

    /// <summary>
    ///     Spawns merchants if they are missing from the map.
    /// </summary>
    private void SpawnMissingMerchants(IEnumerable<MerchantSpawn> merchants)
    {
        foreach (var merchant in merchants)
            if (Subject.GetEntities<Merchant>()
                       .All(x => !x.Template.TemplateKey.EqualsI(merchant.MerchantId)))
                SpawnMerchant(merchant);
    }

    public override void Update(TimeSpan delta)
    {
        EventCheckTimer.Update(delta);

        if (!EventCheckTimer.IntervalElapsed)
            return;

        ManageEventMerchants();
    }
}

/// <summary>
///     Represents a merchant's spawn details.
/// </summary>
public record MerchantSpawn(
    string MerchantId,
    int X,
    int Y,
    Direction Direction);