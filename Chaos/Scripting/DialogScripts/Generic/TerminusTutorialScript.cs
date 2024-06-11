using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusTutorialScript : DialogScriptBase
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public TerminusTutorialScript(
        Dialog subject,
        ISimpleCache simpleCache,
        IMerchantFactory merchantFactory
    )
        : base(subject)
    {
        SimpleCache = simpleCache;
        MerchantFactory = merchantFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out TutorialQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
                
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    if (source.IsAlive)
                        return;

                    var option = new DialogOption
                    {
                        DialogKey = "TerminusDeathExplanation",
                        OptionText = "I'm dead?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (stage == TutorialQuestStage.CompletedTutorial)
                {
                    if (source.IsAlive)
                        return;

                    var option = new DialogOption
                    {
                        DialogKey = "terminus_existance",
                        OptionText = "Send me to the Afterlife"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "terminusgotosgrios":
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    if (source.IsAlive)
                        return;

                    source.SpellBook.Remove("srad tut");
                    ExperienceDistributionScript.GiveExp(source, 5000);
                    Point point;
                    point = new Point(13, 10);
                    var mapInstance = SimpleCache.Get<MapInstance>("after_life");
                    source.TraverseMap(mapInstance, point, true);
                }

                break;
        }
    }
}