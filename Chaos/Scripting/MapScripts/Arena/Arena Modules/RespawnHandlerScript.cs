using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class RespawnHandlerScript(MapInstance subject) : MapScriptBase(subject)
{
    private readonly Dictionary<uint, TimeSpan> RespawnTimes = new();
    private readonly Dictionary<uint, int> DeathCounts = new();

    // Respawn Points
    private readonly Point EscortOffensiveRespawnPoint = new(5, 37);
    private readonly Point EscortDefensiveRespawnPoint = new(39, 6);
    private readonly Point GoldRespawnPointColorClash = new(2, 2);
    private readonly Point GreenRespawnPointColorClash = new(29, 2);
    private readonly Point RedRespawnPointColorClash = new(2, 29);
    private readonly Point BlueRespawnPointColorClash = new(29, 29);
    // Temporary Holding Area for dead players
    private readonly Point EscortHoldingArea = new(30, 19);
    private readonly Point ColorClashHoldingArea = new(17, 0);
    private readonly Point LavaArenaHoldingArea = new(1, 21);

    public override void Update(TimeSpan delta)
    {
        var currentTime = DateTime.Now.TimeOfDay;

        foreach (var aisling in Subject.GetEntities<Aisling>())
        {
            if (!aisling.IsAlive && !RespawnTimes.ContainsKey(aisling.Id))
            {
                InitializeDeath(aisling);
                MoveToHoldingArea(aisling);
            }
            else if (!aisling.IsAlive && RespawnTimes.TryGetValue(aisling.Id, out var respawnTime) && (currentTime > respawnTime))
                RespawnPlayer(aisling);
            else if (aisling.IsAlive && RespawnTimes.ContainsKey(aisling.Id))
                PostRespawnHandling(aisling);
        }
    }

    private void InitializeDeath(Aisling aisling)
    {
        var startingRespawnDuration = 7;
        if (!DeathCounts.TryGetValue(aisling.Id, out var deaths))
        {
            deaths = 0;
        }
        DeathCounts[aisling.Id] = deaths + 1;
        var respawnDuration = startingRespawnDuration * DeathCounts[aisling.Id];
        RespawnTimes[aisling.Id] = DateTime.Now.TimeOfDay + TimeSpan.FromSeconds(respawnDuration);

        aisling.SendActiveMessage($"Deaths: {DeathCounts[aisling.Id]}, Respawning in: {respawnDuration} seconds.");
    }

    private void RespawnPlayer(Aisling aisling)
    {
        aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
        
        switch (Subject.Name)
        {
            case "Lava Arena":
            {
                Point point;

                do
                    point = Subject.Template.Bounds.GetRandomPoint();
                while (Subject.IsWall(point) || Subject.IsBlockingReactor(point));
                
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(100);
                aisling.StatSheet.SetManaPct(100);
                aisling.TraverseMap(Subject, point);
                aisling.Refresh(true);
                aisling.SendOrangeBarMessage("You've respawned! Head back into the fight.");
                break;
            }
            // Initial death, move to holding area, set respawn time, and increment death count
            case "Color Clash - Teams":
                var respawnPointClash = team switch
                {
                    ArenaTeam.Gold  => GoldRespawnPointColorClash,
                    ArenaTeam.Green => GreenRespawnPointColorClash,
                    ArenaTeam.Blue => BlueRespawnPointColorClash,
                    ArenaTeam.Red => RedRespawnPointColorClash,
                    _               => new Point(16, 16) // Default respawn point
                };
                
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(100);
                aisling.StatSheet.SetManaPct(100);
                aisling.TraverseMap(Subject, respawnPointClash);
                aisling.Refresh(true);
                aisling.SendOrangeBarMessage("You've respawned! Head back into the fight.");
                break;
            case "Escort - Teams":
                aisling.Trackers.Enums.TryGetValue(out ArenaSide side);
                var respawnPointEscort = side switch
                {
                    ArenaSide.Offensive  => EscortOffensiveRespawnPoint,
                    ArenaSide.Defender => EscortDefensiveRespawnPoint,
                    _               => new Point(31, 26) // Default respawn point
                };
                
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(100);
                aisling.StatSheet.SetManaPct(100);
                aisling.TraverseMap(Subject, respawnPointEscort);
                aisling.Refresh(true);
                aisling.SendOrangeBarMessage("You've respawned! Head back into the fight.");
                break;
        }
    }
    
    private void MoveToHoldingArea(Aisling aisling)
    {
        var holdingArea = Subject.Name switch
        {
            "Lava Arena" => LavaArenaHoldingArea,
            "Color Clash - Teams" => ColorClashHoldingArea,
            "Escort - Teams"      => EscortHoldingArea,
            _                     => new Point(1, 1) // Default or error handling
        };
        aisling.TraverseMap(Subject, holdingArea);
    }
    
    private void PostRespawnHandling(Aisling aisling) => RespawnTimes.Remove(aisling.Id);
}
