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
    public class WizardDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISimpleCache SimpleCache;
        public WizardDedicateScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache) : base(subject)
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

                source.UserStatSheet.SetBaseClass(BaseClass.Wizard);
                if (source.Gender is Gender.Female)
                    source.TryGiveItems(ItemFactory.Create("magiskirt"));
                if (source.Gender is Gender.Male)
                    source.TryGiveItems(ItemFactory.Create("gardcorp"));
                source.Legend.AddOrAccumulate(new LegendMark("Wizard Class Devotion", "base", MarkIcon.Wizard, MarkColor.Blue, 1, Time.GameTime.Now));
                source.Flags.AddFlag(QuestFlag1.ChosenClass);
                var mapInstance = SimpleCache.Get<MapInstance>("toc");
                var point = new Point(8, 5);
                source.TraverseMap(mapInstance, point);
                source.Animate(ani, source.Id);
            }
        }
    }
}
