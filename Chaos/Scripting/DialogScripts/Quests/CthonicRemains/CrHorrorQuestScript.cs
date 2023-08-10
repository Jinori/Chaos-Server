using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Quests.CthonicRemains;

public class CrHorrorQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly ILogger<CrHorrorQuestScript> Logger;
    
    /// <inheritdoc />
    public CrHorrorQuestScript(Dialog subject, ILogger<CrHorrorQuestScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out CrHorror stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "brynhorn_initial":

                if (!hasStage || (stage == CrHorror.None))

                {
                    if (source.UserStatSheet.Level is <= 98)
                    {
                        Subject.Reply(source, "skip", "brynhorn_initial4");

                        return;
                    }

                    Subject.Reply(source, "skip", "brynhorn_initial1");
                }

                if (stage == CrHorror.Start)
                {
                    Subject.Reply(source, "skip", "Brynhorn_initial2");

                    return;
                }

                if (stage == CrHorror.Complete)
                    Subject.Reply(source, "skip", "brynhorn_initial3");

                break;

            case "crhorror2start":
            {
                source.Trackers.Enums.Set(CrHorror.Start);
                source.Trackers.Flags.AddFlag(QuestFlag1.CrHorror);
                Subject.Reply(source, "skip", "crhorror2");
            }

                break;

            case "crhorror_killed":
            {
                if (!source.Trackers.Counters.TryGetValue("undead_king", out var value) || (value < 1))
                {
                    Subject.Reply(source, "You have not defeated the Undead King yet.");
                    source.SendOrangeBarMessage("You have not defeated the Undead King yet.");

                    return;
                }

                Subject.Reply(source, "I'm impressed.");
                
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Experience, Topics.Entities.Gold, Topics.Entities.Dialog, Topics.Entities.Quest)
                      .WithProperty(source).WithProperty(Subject)
                      .LogInformation("{@AislingName} has received {@ExpAmount} exp and {@GoldAmount} from a quest", source.Name, 10000000, 150000);
                
                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 10000000);
                source.TryGiveGold(150000);
                source.SendOrangeBarMessage("10000000 Exp and 150000 Gold Rewarded!");
                source.Trackers.Enums.Set(CrHorror.Complete);
                source.Trackers.Counters.Remove("undead_king", out _);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Defeated the Undead King",
                        "Undead_king",
                        MarkIcon.Warrior,
                        MarkColor.White,
                        1,
                        GameTime.Now));
            }

                break;
        }
    }
}