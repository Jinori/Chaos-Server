using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.GuildHall;

public class GuildHallScript(MapInstance subject, IStorage<GuildHouseState> guildHouseStateStorage)
    : MapScriptBase(subject)
{
    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling || !Subject.IsShard || aisling.Guild is null)
            return;

        string guildName = aisling.Guild.Name;
        Subject.Morph(GetMorphCode(guildHouseStateStorage.Value, guildName));
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
