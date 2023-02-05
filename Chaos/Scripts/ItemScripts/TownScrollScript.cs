using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ItemScripts
{
    public class TownScrollScript : ConfigurableItemScriptBase
    {
        protected Location Destination { get; init; }
        private readonly ISimpleCache SimpleC;

        public TownScrollScript(Item subject, ISimpleCache simpleCache) : base(subject) => SimpleC = simpleCache;

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
}
