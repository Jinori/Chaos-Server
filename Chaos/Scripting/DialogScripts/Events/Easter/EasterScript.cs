using Chaos.Collections;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Easter;

public class EasterScript : DialogScriptBase
{
    /// <inheritdoc />
    public EasterScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    private readonly ISimpleCache SimpleCache;
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "cadburry_accept":
            {
                var hopmaze = SimpleCache.Get<MapInstance>("hopmaze");

                if (source.Inventory.ContainsByTemplateKey("undinechickenegg"))
                    source.Inventory.RemoveByTemplateKey("undinechickenegg");
                
                if (source.Inventory.ContainsByTemplateKey("undinegoldenchickenegg"))
                    source.Inventory.RemoveByTemplateKey("undinegoldenchickenegg");
                
                source.TraverseMap(hopmaze, new Point(10,14));
                Subject.Close(source);
                break;
            }
        }
    }
}