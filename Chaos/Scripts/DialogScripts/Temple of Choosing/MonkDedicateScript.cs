using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class MonkDedicateScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        public MonkDedicateScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
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
            source.UserStatSheet.SetBaseClass(BaseClass.Monk);
            source.Animate(ani, source.Id);
            if (source.Gender is Gender.Female)
                source.TryGiveItems(ItemFactory.Create("earthbodice"));
            if (source.Gender is Gender.Male)
                source.TryGiveItems(ItemFactory.Create("dobok"));
            source.Legend.AddOrAccumulate(new LegendMark("Monk Class Devotion", "base", MarkIcon.Monk, MarkColor.Blue, 1, Time.GameTime.Now));
        }
    }
}
