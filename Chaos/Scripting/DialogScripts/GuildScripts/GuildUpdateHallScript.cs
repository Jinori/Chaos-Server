using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildUpdateHallScript : DialogScriptBase
{
    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;
    private readonly IMerchantFactory MerchantFactory;
    private const int GOLD_UPGRADE_COST = 2500000;
    private const int GP_UPGRADE_COST = 750;

    public GuildUpdateHallScript(Dialog subject, IStorage<GuildHouseState> guildHouseStateStorage, IMerchantFactory merchantFactory) : base(subject)
    {
        GuildHouseStateStorage = guildHouseStateStorage;
        MerchantFactory = merchantFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        var guildHouseState = GuildHouseStateStorage.Value;
        guildHouseState.SetStorage(GuildHouseStateStorage);

        if (source.Guild == null)
        {
            Subject.Reply(source, "You are not part of a guild.");
            return;
        }

        var guildName = source.Guild.Name;
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "tibbs_purchase_tailor_confirm":
                HandleUpgrade(source, guildName, "tailor", "You do not have enough gold or game points to purchase a tailor.");
                break;
            case "tibbs_purchase_armory_confirm":
                HandleUpgrade(source, guildName, "armory", "You do not have enough gold or game points to purchase an armory.");
                break;
            case "tibbs_purchase_bank_confirm":
                HandleUpgrade(source, guildName, "bank", "You do not have enough gold or game points to purchase a bank.");
                break;
            case "tibbs_purchase_combatroom_confirm":
                HandleUpgrade(source, guildName, "combatroom", "You do not have enough gold or game points to purchase a combat room.");
                break;
        }
    }

    private void HandleUpgrade(Aisling source, string guildName, string property, string insufficientFundsMessage)
    {
        var guildHouseState = GuildHouseStateStorage.Value;

        if (guildHouseState.HasProperty(guildName, property))
        {
            Subject.Reply(source, $"Your guild hall already has a {property} installed.");
            return;
        }

        if (!source.TryTakeGamePoints(GP_UPGRADE_COST) && !source.TryTakeGold(GOLD_UPGRADE_COST))
        {
            Subject.Reply(source, insufficientFundsMessage);
            return;
        }
        
        guildHouseState.EnableProperty(guildName, property);
        var morphCode = GetMorphCode(GuildHouseStateStorage.Value, guildName);
        source.MapInstance.Morph(morphCode);
        SpawnNPCsForMorphCode(source, morphCode);
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
            var p when p.SetEquals(new HashSet<string> { "bank" }) => "27001",
            var p when p.SetEquals(new HashSet<string> { "bank", "armory" }) => "27002",
            var p when p.SetEquals(new HashSet<string> { "armory" }) => "27003",
            var p when p.SetEquals(new HashSet<string> { "tailor" }) => "27004",
            var p when p.SetEquals(new HashSet<string> { "tailor", "bank", "armory" }) => "27005",
            var p when p.SetEquals(new HashSet<string> { "tailor", "armory" }) => "27006",
            var p when p.SetEquals(new HashSet<string> { "tailor", "bank" }) => "27007",
            var p when p.SetEquals(new HashSet<string> { "combatroom" }) => "27008",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "bank", "armory", "tailor" }) => "27009",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "armory" }) => "27010",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "armory", "bank" }) => "27011",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "bank" }) => "27012",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "tailor", "bank" }) => "27013",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "tailor" }) => "27014",
            var p when p.SetEquals(new HashSet<string> { "combatroom", "tailor", "armory" }) => "27015",
            _ => "27000"
        };
    }
    
    private void SpawnNPCsForMorphCode(Aisling source, string morphCode)
    {
        if (!NpcSpawns.TryGetValue(morphCode, out var spawns))
            return;

        var existingNPCs = source.MapInstance.GetEntities<Merchant>().ToDictionary(npc => npc.Template.TemplateKey, npc => npc);

        foreach (var spawn in spawns)
        {
            if (!existingNPCs.ContainsKey(spawn.Name))
            {
                var merch = MerchantFactory.Create(spawn.Name, source.MapInstance, new Point(spawn.X, spawn.Y));
               source.MapInstance.AddEntity(merch, new Point(spawn.X, spawn.Y));
            }
        }
    }    
    
    /// <summary>
/// Dictionary mapping Morph Codes to NPCs that should spawn.
/// </summary>
private static readonly Dictionary<string, List<NPCSpawn>> NpcSpawns = new()
{
    { "27000", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left) ] },
    { "27001", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right) ] },
    { "27002", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("fixx", 81, 24, Direction.Left) ] },
    { "27003", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("fixx", 81, 24, Direction.Left) ] },
    { "27004", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("loom", 76, 52, Direction.Right) ] },
    { "27005", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("fixx", 81, 24, Direction.Left), new NPCSpawn("loom", 76, 52, Direction.Right) ] },
    { "27006", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("fixx", 81, 24, Direction.Left), new NPCSpawn("loom", 76, 52, Direction.Right) ] },
    { "27007", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("loom", 76, 52, Direction.Right) ] },
    { "27008", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("simm", 63, 22, Direction.Left) ] },
    { "27009", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("fixx", 81, 24, Direction.Left), new NPCSpawn("loom", 76, 52, Direction.Right), new NPCSpawn("simm", 63, 22, Direction.Left) ] },
    { "27010", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("simm", 63, 22, Direction.Left), new NPCSpawn("fixx", 81, 24, Direction.Left) ] },
    { "27011", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("fixx", 81, 24, Direction.Left), new NPCSpawn("simm", 63, 22, Direction.Left) ] },
    { "27012", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("simm", 63, 22, Direction.Left) ] },
    { "27013", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("stash", 92, 22, Direction.Right), new NPCSpawn("loom", 76, 52, Direction.Right), new NPCSpawn("simm", 63, 22, Direction.Left) ] },
    { "27014", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("loom", 76, 52, Direction.Right), new NPCSpawn("simm", 63, 22, Direction.Left) ] },
    { "27015", [ new NPCSpawn("quill", 84, 39, Direction.Right), new NPCSpawn("tibbs", 99, 39, Direction.Left), new NPCSpawn("fixx", 81, 24, Direction.Left), new NPCSpawn("loom", 76, 52, Direction.Right), new NPCSpawn("simm", 63, 22, Direction.Left) ] }
};
    
    public sealed record NPCSpawn(string Name, int X, int Y, Direction FacingDirection);
}