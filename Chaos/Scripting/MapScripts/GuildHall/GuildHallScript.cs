using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.GuildHall;

public class GuildHallScript : MapScriptBase
{
    /// <summary>
    ///     Dictionary mapping Morph Codes to NPCs that should spawn.
    /// </summary>
    private static readonly Dictionary<string, List<NPCSpawn>> NpcSpawns = new()
    {
        {
            "27000", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left)
                     ]
        },
        {
            "27001", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right)
                     ]
        },
        {
            "27002", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left)
                     ]
        },
        {
            "27003", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left)
                     ]
        },
        {
            "27004", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right)
                     ]
        },
        {
            "27005", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right)
                     ]
        },
        {
            "27006", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right)
                     ]
        },
        {
            "27007", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right)
                     ]
        },
        {
            "27008", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        },
        {
            "27009", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        },
        {
            "27010", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left)
                     ]
        },
        {
            "27011", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        },
        {
            "27012", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        },
        {
            "27013", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "stash",
                             92,
                             22,
                             Direction.Right),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        },
        {
            "27014", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        },
        {
            "27015", [
                         new NPCSpawn(
                             "quill",
                             84,
                             39,
                             Direction.Right),
                         new NPCSpawn(
                             "tibbs",
                             99,
                             39,
                             Direction.Left),
                         new NPCSpawn(
                             "fixx",
                             81,
                             24,
                             Direction.Left),
                         new NPCSpawn(
                             "loom",
                             76,
                             52,
                             Direction.Right),
                         new NPCSpawn(
                             "simm",
                             63,
                             22,
                             Direction.Left)
                     ]
        }
    };

    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;
    private readonly IMerchantFactory MerchantFactory;

    private readonly IIntervalTimer UpdateTimer;
    private string? GuildName;
    private bool MorphHappened;

    public GuildHallScript(MapInstance subject, IStorage<GuildHouseState> guildHouseStateStorage, IMerchantFactory merchantFactory)
        : base(subject)
    {
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        GuildHouseStateStorage = guildHouseStateStorage;
        MerchantFactory = merchantFactory;
    }

    public void CheckIfNeedMorph()
    {
        if (GuildName is null)
        {
            var aisling = Subject.GetEntities<Aisling>()
                                 .FirstOrDefault(x => (x.Guild?.Name != null) && !x.IsAdmin);

            GuildName = aisling?.Guild?.Name;
        }

        if (GuildName != null)
        {
            Subject.Name = GuildName + "'s Hall";
            var morphCode = GetMorphCode(GuildHouseStateStorage.Value, GuildName);

            if (Subject.Template.TemplateKey != GetMorphCode(GuildHouseStateStorage.Value, GuildName))
            {
                Subject.Morph(morphCode);
                SpawnNPCsForMorphCode(morphCode);
            }
        }
    }

    private string GetMorphCode(GuildHouseState guildHouseState, string guildName)
    {
        var properties = new HashSet<string?>
        {
            guildHouseState.HasProperty(guildName, "bank") ? "bank" : null,
            guildHouseState.HasProperty(guildName, "armory") ? "armory" : null,
            guildHouseState.HasProperty(guildName, "tailor") ? "tailor" : null,
            guildHouseState.HasProperty(guildName, "combatroom") ? "combatroom" : null
        };

        properties.RemoveWhere(p => p == null);

        return properties switch
        {
            var p when p.SetEquals(new HashSet<string>()) => "27000",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "bank"
                }) => "27001",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "bank",
                    "armory"
                }) => "27002",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "armory"
                }) => "27003",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "tailor"
                }) => "27004",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "tailor",
                    "bank",
                    "armory"
                }) => "27005",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "tailor",
                    "armory"
                }) => "27006",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "tailor",
                    "bank"
                }) => "27007",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom"
                }) => "27008",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "bank",
                    "armory",
                    "tailor"
                }) => "27009",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "armory"
                }) => "27010",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "armory",
                    "bank"
                }) => "27011",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "bank"
                }) => "27012",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "tailor",
                    "bank"
                }) => "27013",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "tailor"
                }) => "27014",
            var p when p.SetEquals(
                new HashSet<string>
                {
                    "combatroom",
                    "tailor",
                    "armory"
                }) => "27015",
            _ => "27000"
        };
    }

    private void HandleMobOutsideRectangle(Monster mob)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
            aisling.SendOrangeBarMessage($"{mob.Name} removed for leaving combat sim!");

        Subject.RemoveEntity(mob);
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling || !Subject.IsShard || aisling.Guild is null)
            return;

        GuildName = aisling.Guild.Name;
        Subject.Name = GuildName + "'s Hall";

        if (aisling.IsAdmin && !aisling.MapInstance.Name.StartsWith(GuildName, StringComparison.Ordinal))
            return;

        var morphCode = GetMorphCode(GuildHouseStateStorage.Value, GuildName);

        if (!MorphHappened)
        {
            Subject.Morph(morphCode);
            MorphHappened = true;
            SpawnNPCsForMorphCode(morphCode);
        }
    }

    private void SpawnNPCsForMorphCode(string morphCode)
    {
        if (!NpcSpawns.TryGetValue(morphCode, out var spawns))
            return;

        // Get all existing merchants and store them in a dictionary by their unique TemplateKey
        var existingNPCs = Subject.GetEntities<Merchant>()
                                  .GroupBy(npc => npc.Template.TemplateKey) // Group duplicates
                                  .ToDictionary(g => g.Key, g => g.First()); // Keep only one instance

        foreach (var spawn in spawns)
            if (!existingNPCs.ContainsKey(spawn.Name)) // Only spawn if not already present
            {
                var merch = MerchantFactory.Create(spawn.Name, Subject, new Point(spawn.X, spawn.Y));
                Subject.AddEntity(merch, new Point(spawn.X, spawn.Y));
            }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);

        if (!UpdateTimer.IntervalElapsed)
            return;

        CheckIfNeedMorph();

        var rect = new Rectangle(
            59,
            25,
            11,
            10);

        var mobs = Subject.GetEntities<Monster>()
                          .Where(x => x.Template.TemplateKey != "trainingDummy0")
                          .ToList();

        if (mobs.Count == 0)
            return;

        foreach (var mob in mobs)
        {
            var mobPosition = new Point(mob.X, mob.Y);

            if (!rect.GetPoints()
                     .Contains(mobPosition))
                HandleMobOutsideRectangle(mob);
        }
    }

    public sealed record NPCSpawn(
        string Name,
        int X,
        int Y,
        Direction FacingDirection);
}