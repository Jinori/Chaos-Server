using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class WizardDedicateScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<WizardDedicateScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    public WizardDedicateScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        ILogger<WizardDedicateScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        Logger = logger;
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

            source.UserStatSheet.SetBaseClass(BaseClass.Wizard);

            if (source.Gender is Gender.Female)
                source.GiveItemOrSendToBank(ItemFactory.Create("magiskirt"));

            if (source.Gender is Gender.Male)
                source.GiveItemOrSendToBank(ItemFactory.Create("gardcorp"));

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Wizard Class Devotion",
                    "wizardClass",
                    MarkIcon.Wizard,
                    MarkColor.Blue,
                    1,
                    GameTime.Now));

            source.Trackers.Flags.AddFlag(QuestFlag1.ChosenClass);
            var skill = SkillFactory.Create("energybolt");
            var spell = SpellFactory.Create("arcanebolt");

            if (!source.SpellBook.Contains(spell))
                source.SpellBook.TryAddToNextSlot(spell);

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);

            if (!source.SkillBook.Contains("assail"))
                source.SkillBook.RemoveByTemplateKey("assail");

            var mapInstance = SimpleCache.Get<MapInstance>("toc");
            var point = new Point(8, 5);
            source.TraverseMap(mapInstance, point);
            source.Animate(ani, source.Id);

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Promote)
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} has become wizard", source.Name);
        }
    }
}