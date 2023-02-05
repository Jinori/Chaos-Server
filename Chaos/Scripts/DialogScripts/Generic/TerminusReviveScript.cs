using Chaos.Containers;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Generic;

public class TerminusReviveScript : DialogScriptBase
{
    public TerminusReviveScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) => SimpleCache = simpleCache;

    private readonly ISimpleCache SimpleCache;
    
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_existance":
                Point point;
                point = new Point(13, 10);
                var mapInstance = SimpleCache.Get<MapInstance>("after_life");
                source.TraverseMap(mapInstance, point, true);
                break;
        }
    }
}