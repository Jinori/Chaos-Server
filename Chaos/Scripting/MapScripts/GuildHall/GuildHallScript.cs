using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.GuildHall;

public class GuildHallScript : MapScriptBase
{
    
    private readonly IIntervalTimer UpdateTimer;
    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;
    public GuildHallScript(MapInstance subject, IStorage<GuildHouseState> guildHouseStateStorage)
        : base(subject)
    {
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(500), false);
        GuildHouseStateStorage = guildHouseStateStorage;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);

        if (!UpdateTimer.IntervalElapsed)
            return;

        var rect = new Rectangle(59, 25, 11, 10);
        
        var mobs = Subject.GetEntities<Monster>().ToList();

        if (mobs.Count == 0)
            return;
        
        foreach (var mob in mobs)
        {
            var mobPosition = new Point(mob.X, mob.Y);
            
            if (!rect.GetPoints().Contains(mobPosition))
                HandleMobOutsideRectangle(mob);
        }
    }

    private void HandleMobOutsideRectangle(Monster mob)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
        {
            aisling.SendOrangeBarMessage($"{mob.Name} removed for leaving combat sim!");
        }
        Subject.RemoveEntity(mob);
    }


    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling || !Subject.IsShard || aisling.Guild is null)
            return;

        var guildName = aisling.Guild.Name;
        Subject.Morph(GetMorphCode(GuildHouseStateStorage.Value, guildName));
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
