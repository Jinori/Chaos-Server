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
    public class PeasantDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISimpleCache SimpleCache;
        public PeasantDedicateScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache) : base(subject)
        {
            ItemFactory = itemFactory;
            SimpleCache = simpleCache;
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.UserStatSheet.BaseClass.HasFlag(BaseClass.None))
            {

                var ani = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 78,
                };
                source.UserStatSheet.SetBaseClass(BaseClass.Peasant);
                if (source.Gender is Gender.Female)
                    source.TryGiveItems(ItemFactory.Create("femalenomad"));
                if (source.Gender is Gender.Male)
                    source.TryGiveItems(ItemFactory.Create("malenomad"));
                source.Legend.AddOrAccumulate(new LegendMark("Peasant Class Devotion", "base", MarkIcon.Heart, MarkColor.Blue, 1, Time.GameTime.Now));
                var mapInstance = SimpleCache.Get<MapInstance>("toc");
                var point = new Point(9, 6);
                source.TraverseMap(mapInstance, point);
                source.Animate(ani, source.Id);
            }
        }
    }
}