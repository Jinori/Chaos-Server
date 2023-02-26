using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class HomeScript : ConfigurableSpellScriptBase
{
    private readonly ISimpleCache simpleC;

    protected Location destination { get; init; }

    public HomeScript(Spell subject, ISimpleCache simpleCache)
        : base(subject) => simpleC = simpleCache;

    public override void OnUse(SpellContext context)
    {
        //Lots to unpack here. This could use the players legend to grab their home, randomly put them on any of the World's Inn Maps, could be a guild house or whatever. For now, it functions like the scrolls.

        var source = context.Source;

        if (source.IsAlive)
        {
            var MapInsance = simpleC.Get<MapInstance>(destination.Map);
            source.TraverseMap(MapInsance, destination);
        }
    }
}