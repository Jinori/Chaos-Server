using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Lava_Flow;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaUndergroundScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptFactory<IMapScript, MapInstance> ScriptFactory;
    private readonly Point LavaGreenPoint = new(23, 6);
    private readonly Point LavaRedPoint = new(8, 5);
    private readonly Point LavaBluePoint = new(8, 23);
    private readonly Point LavaGoldPoint = new(23, 21);
    private readonly Point CenterWarp = new(11,10);
    private Point CenterWarpPlayer;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    
    
    /// <inheritdoc />
    public ArenaUndergroundScript(Dialog subject, ISimpleCache simpleCache, IScriptFactory<IMapScript, MapInstance> scriptFactory,
        IClientRegistry<IWorldClient> clientRegistry)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        SimpleCache = simpleCache;
        ScriptFactory = scriptFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_initial":
                HideDialogOptions(source);

                break;

            case "ophie_skandaragauntlet":
            {
                foreach (var aisling in ClientRegistry)
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Arena Host {source.Name} is announcing a Skandara's Gauntlet!");

                break;
            }
            
            case "ophie_serendaelsandbox":
            {
                foreach (var aisling in ClientRegistry)
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Arena Host {source.Name} is announcing a Serendael's Sandbox!");

                break;
            }
            
            case "ophie_startffalavaflowhostnotplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowFFAHostNotPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowFFAHostNotPlayingScript), ScriptFactory);
                
                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;
                    
                    do 
                        point = mapInstance.Template.Bounds.GetRandomPoint();
                    while (mapInstance.IsWall(point) || mapInstance.IsBlockingReactor(point));
                    
                    aisling.TraverseMap(mapInstance, point);
                }
                
                source.TraverseMap(mapInstance, new Point(14, 14));
                Subject.Close(source);
                break;
            }

            case "ophie_startffalavaflowhostplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowFFAHostPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowFFAHostPlayingScript), ScriptFactory);

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    Point point;

                    do
                        point = mapInstance.Template.Bounds.GetRandomPoint();
                    while (mapInstance.IsWall(point) || mapInstance.IsBlockingReactor(point));
                    
                    aisling.TraverseMap(mapInstance, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "ophie_startteamgamelavaflowhostplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowTeamsHostPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowTeamsHostPlayingScript), ScriptFactory);

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    if (!aisling.IsAlive)
                    {
                        aisling.IsDead = false;
                        aisling.StatSheet.SetHealthPct(100);
                        aisling.StatSheet.SetManaPct(100);
                        aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    }
                    
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

                    switch (team)
                    {
                        case ArenaTeam.Blue:
                            aisling.TraverseMap(mapInstance, LavaBluePoint);
                            break;
                        case ArenaTeam.Green:
                            aisling.TraverseMap(mapInstance, LavaGreenPoint);
                            break;
                        case ArenaTeam.Gold:
                            aisling.TraverseMap(mapInstance, LavaGoldPoint);
                            break;
                        case ArenaTeam.Red:
                            aisling.TraverseMap(mapInstance, LavaRedPoint);
                            break;
                    }
                }
                Subject.Close(source);
                break;
            }
            
            case "ophie_startteamgamelavaflowhostnotplayingstart":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_lava");
                var script = mapInstance.Script.As<LavaFlowTeamsHostNotPlayingScript>();
                
                if (script == null) 
                    mapInstance.AddScript(typeof(LavaFlowTeamsHostNotPlayingScript), ScriptFactory);

                foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                {
                    if (!aisling.IsAlive)
                    {
                        aisling.IsDead = false;
                        aisling.StatSheet.SetHealthPct(100);
                        aisling.StatSheet.SetManaPct(100);
                        aisling.Client.SendAttributes(StatUpdateType.Vitality);
                    }
                    
                    aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

                    switch (team)
                    {
                        case ArenaTeam.Blue:
                            aisling.TraverseMap(mapInstance, LavaBluePoint);
                            break;
                        case ArenaTeam.Green:
                            aisling.TraverseMap(mapInstance, LavaGreenPoint);
                            break;
                        case ArenaTeam.Gold:
                            aisling.TraverseMap(mapInstance, LavaGoldPoint);
                            break;
                        case ArenaTeam.Red:
                            aisling.TraverseMap(mapInstance, LavaRedPoint);
                            break;
                    }
                }
                
                source.TraverseMap(mapInstance, new Point(14, 14));
                Subject.Close(source);
                break;
            }
            case "ophie_battlering":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(13, 13));
                Subject.Close(source);

                break;
            }

            case "ophie_revive":
            {
                Subject.Close(source);

                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(100);
                    source.StatSheet.SetManaPct(100);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("Ophie has revived you.");
                    source.Refresh();
                }

                break;
            }

            case "ophie_leave":
            {
                Subject.Close(source);
                var worldMap = SimpleCache.Get<WorldMap>("field001");

                //if we cant set the active object, return
                if (!source.ActiveObject.SetIfNull(worldMap))
                    return;

                source.MapInstance.RemoveObject(source);
                source.Client.SendWorldMap(worldMap);

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ophie_placeonredconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Red);
                var rect = new Rectangle(new Point(11, 17), 4, 3);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Red team!");

                break;
            }
            case "ophie_placeongreenconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Green);
                var rect = new Rectangle(new Point(18, 10), 3, 4);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Green team!");

                break;
            }
            case "ophie_placeongoldconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Gold);
                var rect = new Rectangle(new Point(5, 10), 4, 3);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Gold team!");

                break;
            }
            case "ophie_placeonblueconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Set(ArenaTeam.Blue);
                var rect = new Rectangle(new Point(12, 4), 4, 3);
                aisling?.WarpTo(rect.GetRandomPoint());
                aisling?.SendActiveMessage("You've been placed on the Blue team!");

                break;
            }
            case "ophie_removeteamconfirm":
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var playerToPlace))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }
                
                var rect = new Rectangle(new Point(11, 10), 3, 4);
                    
                do
                {
                    CenterWarpPlayer = rect.GetRandomPoint();
                } 
                while (CenterWarp == CenterWarpPlayer);
                
                var aisling = source.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Name.EqualsI(playerToPlace));
                aisling?.Trackers.Enums.Remove<ArenaTeam>();
                aisling?.WarpTo(CenterWarpPlayer);
                break;
            }
        }
    }
    
    
    public void HideDialogOptions(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ArenaHost stage);   
        if ((stage != ArenaHost.Host) && (stage != ArenaHost.MasterHost))
            RemoveOption(Subject, "Host Options"); 
    }
    
    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName).HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }
}