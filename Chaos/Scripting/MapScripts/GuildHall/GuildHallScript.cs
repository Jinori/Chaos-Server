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
            guildHouseState.HasProperty(guildName, "tailor") ? "tailor" : null
        };

        properties.RemoveWhere(p => p == null);

        return properties switch
        {
            _ when properties.SetEquals(new HashSet<string>()) => "27000",
            _ when properties.SetEquals(new HashSet<string> { "bank" }) => "27001",
            _ when properties.SetEquals(new HashSet<string> { "bank", "armory" }) => "27002",
            _ when properties.SetEquals(new HashSet<string> { "armory" }) => "27003",
            _ when properties.SetEquals(new HashSet<string> { "tailor" }) => "27004",
            _ when properties.SetEquals(new HashSet<string> { "tailor", "bank", "armory" }) => "27005",
            _ when properties.SetEquals(new HashSet<string> { "tailor", "armory" }) => "27006",
            _ when properties.SetEquals(new HashSet<string> { "tailor", "bank" }) => "27007",
            _ => "27000"
        };
    }
}
