using Chaos.Collections;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class DamageGameScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public DamageGameScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "hazel_acceptedquest":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("hm_damagegame");
                source.TraverseMap(mapInstance, new Point(4, 2));

                break;
            }
        }
    }
}