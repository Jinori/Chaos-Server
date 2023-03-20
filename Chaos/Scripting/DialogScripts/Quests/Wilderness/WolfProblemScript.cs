using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests;

public class WolfProblemScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public WolfProblemScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out WolfProblemStage stage);


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "francis_initial":
                if ((!hasStage) || (stage == WolfProblemStage.None))
                {
                    Subject.Text = "Hey you! Are you busy?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolfproblem_initial",
                        OptionText = "No, what's up?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if (stage == WolfProblemStage.Start)
                {
                    Subject.Text = "Did you get rid of the wolf?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolfproblem_turnin",
                        OptionText = "Yes."
                    };
                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    var option2 = new DialogOption
                    {
                        DialogKey = "close",
                        OptionText = "No, Not yet."
                    };

                    if (!Subject.HasOption(option2))
                        Subject.Options.Add(option2);

                }

                 if (stage == WolfProblemStage.Complete)
                {
                    Subject.Text = "Thank you again for getting rid of that wolf.";
                }


                break;

            case "wolfproblem1":
                {
                    source.Trackers.Enums.Set(WolfProblemStage.Start);
                }
                break;
           
            case "wolfproblem_turnin":
                {
                    if (!source.Trackers.Counters.TryGetValue("wolf", out var value) || (value < 1))
                    {
                        Subject.Text = "I can still hear the wolf! Please get rid of it.";
                        source.SendOrangeBarMessage("You hear the wolf howl in the distance.");

                        return;
                    }

                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, 4000);
                    source.TryGiveGold(1500);
                    source.SendOrangeBarMessage("4000 Exp and 1500 Gold Rewarded!");
                    source.Trackers.Enums.Set(WolfProblemStage.Complete);
                    source.Trackers.Counters.Remove("wolf", out _);
                }

                break;
        }
    }
}