using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Temple_of_Choosing;

public class RogueDedicateScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;

    public RogueDedicateScript(
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

            source.UserStatSheet.SetBaseClass(BaseClass.Rogue);

            if (source.Gender is Gender.Female)
                source.TryGiveItems(ItemFactory.Create("cotte"));

            if (source.Gender is Gender.Male)
                source.TryGiveItems(ItemFactory.Create("scoutleather"));

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Rogue Class Devotion",
                    "base",
                    MarkIcon.Rogue,
                    MarkColor.Blue,
                    1,
                    GameTime.Now));

            source.Flags.AddFlag(QuestFlag1.ChosenClass);
            var skill = SkillFactory.Create("assail");

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);

            var mapInstance = SimpleCache.Get<MapInstance>("toc");
            var point = new Point(8, 5);
            source.TraverseMap(mapInstance, point);
            source.Animate(ani, source.Id);
        }
    }
}