using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Generic;

public class TerminusTutorialScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public TerminusTutorialScript(
        Dialog subject,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        source.Enums.TryGetValue(out TutorialQuestStage stage);

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

                    if (!Subject.HasOption(option))
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

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "terminusgotosgrios":
                if (stage == TutorialQuestStage.GiantFloppy)
                {
                    if (source.IsAlive)
                        return;

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Completed Tutorial",
                            "base",
                            MarkIcon.Heart,
                            MarkColor.White,
                            1,
                            GameTime.Now));

                    source.SpellBook.Remove("srad tut");
                    source.Enums.Set(TutorialQuestStage.CompletedTutorial);
                    ExperienceDistributionScript.GiveExp(source, 1000);
                    Point point;
                    point = new Point(13, 10);
                    var mapInstance = SimpleCache.Get<MapInstance>("after_life");
                    source.TraverseMap(mapInstance, point, true);
                }

                break;
        }
    }
}