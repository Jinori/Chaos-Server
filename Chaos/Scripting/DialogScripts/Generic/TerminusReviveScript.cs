using Chaos.Collections;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusReviveScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public TerminusReviveScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) => SimpleCache = simpleCache;

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