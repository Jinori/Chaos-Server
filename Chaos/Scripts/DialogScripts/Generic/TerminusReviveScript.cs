using System.Diagnostics.Eventing.Reader;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts;

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
                MapInstance mapInstance;
                Point point;
                point = new Point(13, 10);
                mapInstance = SimpleCache.Get<MapInstance>("after_life");
                source.TraverseMap(mapInstance, point, true);
                break;
        }
    }
}