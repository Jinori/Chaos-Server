using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;

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
                    if (source.UserStatSheet.Level is <= 1 or >= 16)
                        return;
                    
                    Subject.Reply(source, "skip", "wolfproblem_start");
                    return;
                }

                if (stage == WolfProblemStage.Start)
                {
                    Subject.Reply(source, "skip", "wolfproblem_turninstart");
                    return;
                }

                 if (stage == WolfProblemStage.Complete)
                {
                    Subject.Reply(source, "Thank you again for getting rid of that wolf.");
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
                        Subject.Reply(source, "I can still hear the wolf! Please get rid of it.");
                        source.SendOrangeBarMessage("You hear the wolf howl in the distance.");

                        return;
                    }

                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, 5000);
                    source.TryGiveGold(2500);
                    source.SendOrangeBarMessage("5000 Exp and 2500 Gold Rewarded!");
                    source.Trackers.Enums.Set(WolfProblemStage.Complete);
                    source.Trackers.Counters.Remove("wolf", out _);
                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Saved a cow from the Big Bad Wolf",
                            "wolfproblem",
                            MarkIcon.Heart,
                            MarkColor.White,
                            1,
                            GameTime.Now));
                }

                break;
        }
    }
}