using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class MonkDedicateScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;

    public MonkDedicateScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        ISkillFactory skillFactory
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        SkillFactory = skillFactory;
    }

    public override void OnDisplayed(Aisling source)
    {
        if (!source.Flags.HasFlag(QuestFlag1.ChosenClass))
        {
            var ani = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 78
            };

            source.UserStatSheet.SetBaseClass(BaseClass.Monk);

            if (source.Gender is Gender.Female)
                source.TryGiveItems(ItemFactory.Create("earthbodice"));

            if (source.Gender is Gender.Male)
                source.TryGiveItems(ItemFactory.Create("dobok"));

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Monk Class Devotion",
                    "monkClass",
                    MarkIcon.Monk,
                    MarkColor.Blue,
                    1,
                    GameTime.Now));

            source.SkillBook.Remove("assail");

            source.Flags.AddFlag(QuestFlag1.ChosenClass);
            var skill = SkillFactory.Create("punch");

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);

            var mapInstance = SimpleCache.Get<MapInstance>("toc");
            var point = new Point(8, 5);
            source.TraverseMap(mapInstance, point);
            source.Animate(ani, source.Id);
        }
    }
}