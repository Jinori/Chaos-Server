using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Factories;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Storage;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class RogueDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISimpleCache SimpleCache;
        public RogueDedicateScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache) : base(subject)
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
            source.UserStatSheet.SetBaseClass(BaseClass.Rogue);
            source.Animate(ani, source.Id);
            if (source.Gender is Gender.Female)
                source.TryGiveItems(ItemFactory.Create("cotte"));
            if (source.Gender is Gender.Male)
                source.TryGiveItems(ItemFactory.Create("scoutleather"));
            source.Legend.AddOrAccumulate(new LegendMark("Rogue Class Devotion", "base", MarkIcon.Rogue, MarkColor.Blue, 1, Time.GameTime.Now));
            var mapInstance = SimpleCache.Get<MapInstance>("toclobby");
            var point = new Point(9, 6);
            source.TraverseMap(mapInstance, point);
        }
    }
}