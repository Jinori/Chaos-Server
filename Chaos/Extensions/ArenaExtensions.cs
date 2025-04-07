using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Non_Combat;

namespace Chaos.Extensions;

public static class ArenaExtensions
{
    public static readonly HashSet<string> FriendlyArenaMapNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Typing Arena",
        "Arena Underground",
        "Drowned Labyrinth Entrance"
    };
    
    public static bool IsHostingArena(this Aisling aisling) => aisling.IsOnArenaMap() && aisling.Trackers.Enums.TryGetValue(out HostingArena value) && value is HostingArena.Yes;
    
    public static bool IsArenaHost(this Aisling aisling) => aisling.Trackers.Enums.TryGetValue(out ArenaHost value) && value is ArenaHost.Host or ArenaHost.MasterHost;

    public static bool IsOnPvPArenaMap(this Creature creature) =>
        creature.IsOnArenaMap() && !FriendlyArenaMapNames.Contains(creature.MapInstance.Name);
    public static bool IsOnPvEArenaMap(this Creature creature) =>
        creature.IsOnArenaMap() && FriendlyArenaMapNames.Contains(creature.MapInstance.Name);
    
    public static bool IsOnArenaMap(this Creature creature) => creature.MapInstance.Script.Is<ArenaMapTagScript>();
    
    public static bool IsOnSameArenaTeam(this Aisling source, Aisling target) =>
        source.Trackers.Enums.TryGetValue(out ArenaTeam sourceTeam)
        && target.Trackers.Enums.TryGetValue(out ArenaTeam targetTeam)
        && (sourceTeam == targetTeam);
}