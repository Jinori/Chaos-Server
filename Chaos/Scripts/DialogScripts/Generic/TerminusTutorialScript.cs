using System.Diagnostics.Eventing.Reader;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts;

public class TerminusTutorialScript : DialogScriptBase
{
    public TerminusTutorialScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Enums.TryGetValue(out TutorialQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "terminus_initial":
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    if (source.IsAlive)
                    {
                        return;
                    }
                    
                    var option = new DialogOption
                    {
                        DialogKey = "TerminusDeathExplanation",
                        OptionText = "I'm dead?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == TutorialQuestStage.CompletedTutorial)
                {
                    if (source.IsAlive)
                    {
                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "terminus_existance",
                        OptionText = "Send me to the Afterlife"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;
                
            case "terminusgotosgrios":
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    if (source.IsAlive)
                    {
                        return;
                    }
                    
                    source.Legend.AddOrAccumulate(new LegendMark("Completed Tutorial", "base", MarkIcon.Heart, MarkColor.White, 1, GameTime.Now));
                    source.SpellBook.Remove("sradtut");
                    source.Enums.Set(TutorialQuestStage.CompletedTutorial);
                    Point point;
                    point = new Point(13,10);
                    var mapInstance = SimpleCache.Get<MapInstance>("after_life");
                    source.TraverseMap(mapInstance, point, true);
                }
                break;
        }
    }
}