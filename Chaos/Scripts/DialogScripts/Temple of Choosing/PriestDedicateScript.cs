using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Factories;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class PriestDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISimpleCache SimpleCache;

        public PriestDedicateScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache) : base(subject)
        {
            ItemFactory = itemFactory;
            SimpleCache = simpleCache;
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.Legend.TryGetValue("base", out var legendMark))
                return;

            var ani = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 78,
            };
            source.UserStatSheet.SetBaseClass(BaseClass.Priest);
            source.Animate(ani, source.Id);
            if (source.Gender is Gender.Female)
                source.TryGiveItems(ItemFactory.Create("gorgetgown"));
            if (source.Gender is Gender.Male)
                source.TryGiveItems(ItemFactory.Create("cowl"));
            source.Legend.AddOrAccumulate(new LegendMark("Priest Class Devotion", "base", MarkIcon.Priest, MarkColor.Blue, 1, Time.GameTime.Now));

            var mapInstance = SimpleCache.Get<MapInstance>("toclobby");
            var point = new Point(9, 6);
            source.TraverseMap(mapInstance, point);
        }
    }
}
