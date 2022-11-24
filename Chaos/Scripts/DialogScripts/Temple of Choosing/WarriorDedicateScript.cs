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

namespace Chaos.Scripts.DialogScripts
{
    public class WarriorDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly SimpleCache SimpleCache;
        public WarriorDedicateScript(Dialog subject, IItemFactory itemFactory, SimpleCache simpleCache) : base(subject)
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
            source.UserStatSheet.SetBaseClass(BaseClass.Warrior);
            source.Animate(ani, source.Id);
            if (source.Gender is Gender.Female)
                source.TryGiveItems(ItemFactory.Create("leatherbliaut"));
            if (source.Gender is Gender.Male)
                source.TryGiveItems(ItemFactory.Create("leathertunic"));
            source.Legend.AddOrAccumulate(new LegendMark("Warrior Class Devotion", "base", MarkIcon.Warrior, MarkColor.Blue, 1, Time.GameTime.Now));

            var mapInstance = SimpleCache.Get<MapInstance>("toclobby");
            var point = new Point(9, 6);
            source.TraverseMap(mapInstance, point);
        }
    }
}