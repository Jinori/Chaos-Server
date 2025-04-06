using Chaos.Collections;
using Chaos.Collections.Time;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusArenaHostedPortScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    
    /// <inheritdoc />
    public TerminusArenaHostedPortScript(Dialog subject, ISimpleCache simpleCache, IClientRegistry<IChaosWorldClient> clientRegistry)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var host = ClientRegistry.Any(x => x.Aisling.IsOnArenaMap() && x.Aisling.IsArenaHost() && x.Aisling.IsHostingArena());

                if (host && !source.Trackers.TimedEvents.HasActiveEvent("Jail", out _))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "terminus_arenahostedport",
                        OptionText = "Portal to Drowned Labyrinth"
                    };
                
                    if (!Subject.HasOption(option.OptionText) )
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "terminus_arenahostedportyes":
            {
                var map = SimpleCache.Get<MapInstance>("arena_underground");
                source.TraverseMap(map, new Point(2, 2));
                break;
            }
        }
    }
}