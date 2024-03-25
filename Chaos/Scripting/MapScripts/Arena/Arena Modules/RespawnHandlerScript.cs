using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public class RespawnHandlerScript(MapInstance subject) : MapScriptBase(subject)
{
    private Dictionary<uint, TimeSpan> RespawnTimes = new();
    private Dictionary<uint, int> DeathCounts = new();

    // Respawn Points
    private readonly Point GoldRespawnPoint = new(5, 37);
    private readonly Point GreenRespawnPoint = new(39, 6);
    // Temporary Holding Area for dead players
    private readonly Point HoldingArea = new(30, 19);

    public override void Update(TimeSpan delta)
    {
        var currentTime = DateTime.Now.TimeOfDay;

        foreach (var aisling in Subject.GetEntities<Aisling>())
            if (!aisling.IsAlive)
            {
                if (!RespawnTimes.TryGetValue(aisling.Id, out var respawnTime))
                {
                    // Initial death, move to holding area, set respawn time, and increment death count
                    aisling.TraverseMap(Subject, HoldingArea);
                    RespawnTimes[aisling.Id] = currentTime + TimeSpan.FromSeconds(7);
                    DeathCounts[aisling.Id] = 1;
                }
                else if (currentTime > respawnTime)
                {
                    // Time to respawn and revive
                    RespawnPlayer(aisling);
                    // Update respawn time for next death
                    RespawnTimes[aisling.Id] = currentTime + TimeSpan.FromSeconds(7 * (++DeathCounts[aisling.Id]));
                }
            }
            else if (RespawnTimes.ContainsKey(aisling.Id))
            {
                // Clear respawn time if player is alive
                RespawnTimes.Remove(aisling.Id);
                DeathCounts.Remove(aisling.Id);
            }
    }



    private void RespawnPlayer(Aisling aisling)
    {
        aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
        var respawnPoint = team switch
        {
            ArenaTeam.Gold => GoldRespawnPoint,
            ArenaTeam.Green => GreenRespawnPoint,
            _ => new Point(31, 26) // Default respawn point
        };
        
        aisling.IsDead = false;
        aisling.StatSheet.SetHealthPct(100);
        aisling.StatSheet.SetManaPct(100);
        aisling.TraverseMap(Subject, respawnPoint);
        aisling.Refresh(true);
    }
}
