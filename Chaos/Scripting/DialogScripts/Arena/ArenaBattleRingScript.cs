using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaBattleRingScript : DialogScriptBase
{
    private readonly IScriptFactory<IMapScript, MapInstance> ScriptFactory;
    private readonly IEffectFactory EffectFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public ArenaBattleRingScript(Dialog subject, IScriptFactory<IMapScript, MapInstance> scriptFactory, ISimpleCache simpleCache, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ScriptFactory = scriptFactory;
        SimpleCache = simpleCache;
    }

    public void HideDialogOptions(Aisling source) => source.Trackers.Enums.TryGetValue(out ArenaHost stage);

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alex_initial":
                HideDialogOptions(source);

                break;

            case "alex_revive":
            {
                Subject.Close(source);

                if (source.Effects.Contains("Arena Revive"))
                {
                    source.SendActiveMessage("You must wait to be revived.");
                    return;
                }
                
                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("Alex has revived you.");
                    source.Refresh();
                    source.Display();
                }

                break;
            }
            case "alex_north":
            {
                Subject.Close(source);

                if (source.Effects.Contains("Arena Revive"))
                {
                    source.SendActiveMessage("You must wait to enter the Battle Ring.");
                    return;
                }
                
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(3, 3));

                break;
            }
            case "alex_east":
            {
                Subject.Close(source);
                
                if (source.Effects.Contains("Arena Revive"))
                {
                    source.SendActiveMessage("You must wait to enter the Battle Ring.");
                    return;
                }
                
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(53, 3));

                break;
            }
            case "alex_south":
            {
                Subject.Close(source);
                
                if (source.Effects.Contains("Arena Revive"))
                {
                    source.SendActiveMessage("You must wait to enter the Battle Ring.");
                    return;
                }
                
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(53, 53));

                break;
            }
            case "alex_west":
            {
                Subject.Close(source);
                
                if (source.Effects.Contains("Arena Revive"))
                {
                    source.SendActiveMessage("You must wait to enter the Battle Ring.");
                    return;
                }
                
                var mapInstance = SimpleCache.Get<MapInstance>("arena_battle_ring");
                source.TraverseMap(mapInstance, new Point(3, 53));

                break;
            }
            
            case "alex_hostedstaging":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
                source.TraverseMap(mapInstance, new Point(12, 10));
                Subject.Close(source);

                break;
            }
            
            case "alex_leave":
            {
                Subject.Close(source);
                var worldMap = SimpleCache.Get<WorldMap>("field001");

                //if we cant set the active object, return
                if (!source.ActiveObject.SetIfNull(worldMap))
                    return;
                
                if (source.IsDead)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("You have been revived.");
                    source.Refresh();
                    source.Display();
                }

                source.Trackers.Enums.Remove<ArenaTeam>();
                source.MapInstance.RemoveEntity(source);
                source.Client.SendWorldMap(worldMap);
                
                if (!source.Effects.Contains("Arena Revive"))
                {
                    var effect = EffectFactory.Create("ArenaRevive");
                    source.Effects.Apply(source, effect);
                }
                
                break;
            }
        }
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