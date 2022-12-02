using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
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
            if (!source.Flags.HasFlag(QuestFlag1.ChosenClass))
            {
                var ani = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 78,
                };
                source.UserStatSheet.SetBaseClass(BaseClass.Priest);
                if (source.Gender is Gender.Female)
                    source.TryGiveItems(ItemFactory.Create("gorgetgown"));
                if (source.Gender is Gender.Male)
                    source.TryGiveItems(ItemFactory.Create("cowl"));
                source.Legend.AddOrAccumulate(new LegendMark("Priest Class Devotion", "base", MarkIcon.Priest, MarkColor.Blue, 1, Time.GameTime.Now));
                source.Flags.AddFlag(QuestFlag1.ChosenClass);
                var mapInstance = SimpleCache.Get<MapInstance>("toc");
                var point = new Point(8, 5);
                source.TraverseMap(mapInstance, point);
                source.Animate(ani, source.Id);
            }
        }
    }
}
