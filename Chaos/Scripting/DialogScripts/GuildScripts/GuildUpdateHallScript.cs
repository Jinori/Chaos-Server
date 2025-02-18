using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildUpdateHallScript : DialogScriptBase
{
    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;
    private const int GOLD_UPGRADE_COST = 2500000;
    private const int GP_UPGRADE_COST = 750;

    public GuildUpdateHallScript(Dialog subject, IStorage<GuildHouseState> guildHouseStateStorage) : base(subject)
    {
        GuildHouseStateStorage = guildHouseStateStorage;
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
        source.MapInstance.Morph(GetMorphCode(guildHouseState, guildName));
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
}