using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaEntranceScript : DialogScriptBase
{
    private readonly IScriptFactory<IMapScript, MapInstance> ScriptFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public ArenaEntranceScript(Dialog subject, IScriptFactory<IMapScript, MapInstance> scriptFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ScriptFactory = scriptFactory;
        SimpleCache = simpleCache;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "rlyeh_revive":
            {
                Subject.Close(source);

                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("R'lyeh has revived you.");
                    source.Refresh();
                }

                break;
            }
            
            case "rlyeh_hostedstaging":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                source.TraverseMap(mapInstance, new Point(12, 10));
                Subject.Close(source);

                break;
            }
            
            case "rlyeh_battlering":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(13, 13));
                Subject.Close(source);
                break;
            }
            
            case "rlyeh_leave":
            {
                Subject.Close(source);
                var worldMap = SimpleCache.Get<WorldMap>("field001");

                //if we cant set the active object, return
                if (!source.ActiveObject.SetIfNull(worldMap))
                    return;

                source.MapInstance.RemoveEntity(source);
                source.Client.SendWorldMap(worldMap);

                break;
            }
        }
    }
}