using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class RespawnHandlerScript(MapInstance subject) : MapScriptBase(subject)
{
    private readonly Point BlueRespawnPointColorClash = new(29, 29);
    private readonly Point ColorClashHoldingArea = new(17, 0);
    private readonly Dictionary<uint, int> DeathCounts = new();

    private readonly Point EscortDefensiveRespawnPoint = new(39, 6);

    // Temporary Holding Area for dead players
    private readonly Point EscortHoldingArea = new(30, 19);

    // Respawn Points
    private readonly Point EscortOffensiveRespawnPoint = new(5, 37);
    private readonly Point GoldRespawnPointColorClash = new(2, 2);
    private readonly Point GreenRespawnPointColorClash = new(29, 2);
    private readonly Point LavaArenaHoldingArea = new(1, 21);
    private readonly Point RedRespawnPointColorClash = new(2, 29);
    private readonly Dictionary<uint, TimeSpan> RespawnTimes = new();

    private void CompleteRespawn(Aisling aisling, Point respawnPoint, string message)
    {
        aisling.IsDead = false;
        aisling.StatSheet.SetHealthPct(100);
        aisling.StatSheet.SetManaPct(100);
        aisling.TraverseMap(Subject, respawnPoint);
        aisling.Refresh(true);
        aisling.SendOrangeBarMessage(message);
    }

    private void InitializeDeath(Aisling aisling)
    {
        const int BASE_RESPAWN_DURATION = 7;
        const int ADDITIONAL_TIME_PER_DEATH = 10;

        if (!DeathCounts.TryGetValue(aisling.Id, out var deaths))
            deaths = 0;

        DeathCounts[aisling.Id] = deaths + 1;

        var respawnDuration = BASE_RESPAWN_DURATION + ADDITIONAL_TIME_PER_DEATH * DeathCounts[aisling.Id];
        RespawnTimes[aisling.Id] = DateTime.Now.TimeOfDay + TimeSpan.FromSeconds(respawnDuration);

        aisling.SendActiveMessage($"Deaths: {DeathCounts[aisling.Id]}, Respawning in: {respawnDuration} seconds.");

        if (DeathCounts[aisling.Id] > 1)
            aisling.SendOrangeBarMessage("Warning: Your respawn time is increasing with each death.");
    }

    private void MoveToHoldingArea(Aisling aisling)
    {
        var holdingArea = Subject.Name switch
        {
            "Lava Arena"          => LavaArenaHoldingArea,
            "Color Clash - Teams" => ColorClashHoldingArea,
            "Escort - Teams"      => EscortHoldingArea,
            _                     => new Point(1, 1)
        };
        aisling.TraverseMap(Subject, holdingArea);
    }

    private void PostRespawnHandling(Aisling aisling) => RespawnTimes.Remove(aisling.Id);

    private void RespawnInColorClash(Aisling aisling, ArenaTeam team)
    {
        var respawnPointClash = team switch
        {
            ArenaTeam.Gold  => GoldRespawnPointColorClash,
            ArenaTeam.Green => GreenRespawnPointColorClash,
            ArenaTeam.Blue  => BlueRespawnPointColorClash,
            ArenaTeam.Red   => RedRespawnPointColorClash,
            _               => new Point(16, 16)
        };

        CompleteRespawn(aisling, respawnPointClash, "You've respawned! Head back into the fight.");
    }

    private void RespawnInEscort(Aisling aisling)
    {
        aisling.Trackers.Enums.TryGetValue(out ArenaSide side);

        var respawnPointEscort = side switch
        {
            ArenaSide.Offensive => EscortOffensiveRespawnPoint,
            ArenaSide.Defender  => EscortDefensiveRespawnPoint,
            _                   => new Point(31, 26)
        };

        CompleteRespawn(aisling, respawnPointEscort, "You've respawned! Head back into the fight.");
    }

    private void RespawnInLavaArena(Aisling aisling)
    {
        Point point;

        do
            point = Subject.Template.Bounds.GetRandomPoint();
        while (Subject.IsWall(point) || Subject.IsBlockingReactor(point));

        CompleteRespawn(aisling, point, "You've respawned! Head back into the fight.");
    }

    private void RespawnPlayer(Aisling aisling)
    {
        aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

        switch (Subject.Name)
        {
            case "Lava Arena":
                RespawnInLavaArena(aisling);

                break;
            case "Color Clash - Teams":
                RespawnInColorClash(aisling, team);

                break;
            case "Escort - Teams":
                RespawnInEscort(aisling);

                break;
        }
    }

    public override void Update(TimeSpan delta)
    {
        var currentTime = DateTime.Now.TimeOfDay;

        foreach (var aisling in Subject.GetEntities<Aisling>())
            switch (aisling.IsAlive)
            {
                case false when !RespawnTimes.ContainsKey(aisling.Id):
                    InitializeDeath(aisling);
                    MoveToHoldingArea(aisling);

                    break;
                case false when RespawnTimes.TryGetValue(aisling.Id, out var respawnTime) && (currentTime > respawnTime):
                    RespawnPlayer(aisling);

                    break;
                case true when RespawnTimes.ContainsKey(aisling.Id):
                    PostRespawnHandling(aisling);

                    break;
            }
    }
}