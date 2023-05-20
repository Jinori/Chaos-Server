using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class TownScrollScript : ConfigurableItemScriptBase
{
    private readonly ISimpleCache SimpleC;
    protected Location Destination { get; init; }

    public TownScrollScript(Item subject, ISimpleCache simpleCache)
        : base(subject) => SimpleC = simpleCache;

    public override void OnUse(Aisling source)
    {
        if (source.IsAlive)
        {
            var instance = SimpleC.Get<MapInstance>(Destination.Map);
            source.TraverseMap(instance, Destination);
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1, out _);
        }
    }
}