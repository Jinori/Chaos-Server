using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class WizardDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly ISimpleCache SimpleCache;
        private readonly ISkillFactory SkillFactory;

        public WizardDedicateScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache, ISkillFactory skillFactory) : base(subject)
        {
            ItemFactory = itemFactory;
            SimpleCache = simpleCache;
            SkillFactory= skillFactory;
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
                var skill = SkillFactory.Create("assail");
                if (!source.SkillBook.Contains(skill))
                {
                    source.SkillBook.TryAddToNextSlot(skill);
                }
                var mapInstance = SimpleCache.Get<MapInstance>("toc");
                var point = new Point(8, 5);
                source.TraverseMap(mapInstance, point);
                source.Animate(ani, source.Id);
            }
        }
    }
}
