using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class RogueDedicateScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    public RogueDedicateScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
    }

    public override void OnDisplayed(Aisling source)
    {
        if (!source.Trackers.Flags.HasFlag(QuestFlag1.ChosenClass))
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
                    "rogueClass",
                    MarkIcon.Rogue,
                    MarkColor.Blue,
                    1,
                    GameTime.Now));

            source.Trackers.Flags.AddFlag(QuestFlag1.ChosenClass);
            var skill = SkillFactory.Create("assail");
            var skill2 = SkillFactory.Create("stab");
            var spell = SpellFactory.Create("needletrap");

            if (!source.SpellBook.Contains(spell))
                source.SpellBook.TryAddToNextSlot(spell);

            if (!source.SkillBook.Contains(skill2))
                source.SkillBook.TryAddToNextSlot(skill2);

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);

            var mapInstance = SimpleCache.Get<MapInstance>("toc");
            var point = new Point(8, 5);
            source.TraverseMap(mapInstance, point);
            source.Animate(ani, source.Id);
        }
    }
}