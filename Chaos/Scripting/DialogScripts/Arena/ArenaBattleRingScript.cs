using System.Reactive.Subjects;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaBattleRingScript : DialogScriptBase
{
    private readonly IScriptFactory<IMapScript, MapInstance> ScriptFactory;

    /// <inheritdoc />
    public ArenaBattleRingScript(Dialog subject, IScriptFactory<IMapScript, MapInstance> scriptFactory)
        : base(subject) =>
        ScriptFactory = scriptFactory;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alex_initial":
                HideDialogOptions(source);
                break;

            case "alex_goldteam":
            {
                var aislings = source.MapInstance.GetEntities<Aisling>().ToList();

                break;
            }
            case "alex_revive":
            {
                Subject.Close(source);
                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("Alex has revived you.");
                    source.Refresh();
                }
                break;
            }
            case "alex_north":
            {
                Subject.Close(source);
                source.WarpTo(new Point(5, 5));
                break;
            }
            case "alex_east":
            {
                
                Subject.Close(source);
                source.WarpTo(new Point(20, 5));
                break;
            }
            case "alex_south":
            {
                Subject.Close(source);
                source.WarpTo(new Point(20, 20));
                break;
            }
            case "alex_west":
            {
                Subject.Close(source);
                source.WarpTo(new Point(5, 20));
                break;
            }
            case "alex_hiddenhavoc":
            {
                foreach (var enabledScript in source.MapInstance.ScriptKeys)
                {
                    source.MapInstance.ScriptKeys.Remove(enabledScript);
                    source.SendActiveMessage($"{enabledScript} has been removed from the Arena Battle Ring.");
                }
                
                var script = source.MapInstance.Script.As<MapScripts.HiddenHavocGameScript>();

                if (script == null)
                    source.MapInstance.AddScript(typeof(MapScripts.HiddenHavocGameScript), ScriptFactory);

                var aislings = source.MapInstance.GetEntities<Aisling>().ToList();

                foreach (var aisling in aislings)
                {
                    aisling.SendServerMessage(ServerMessageType.OrangeBar2, $"Hidden Havoc mode has enabled by {source.Name}!");
                }
                Subject.Close(source);
                break;
            }
        }
    }

    public void HideDialogOptions(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ArenaHost stage);
        //if ((stage != ArenaHost.Host) && (stage != ArenaHost.MasterHost))
          //  RemoveOption(Subject, "Host Options");   
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