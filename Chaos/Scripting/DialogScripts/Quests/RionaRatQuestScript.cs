using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests;

public class RionaRatQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public RionaRatQuestScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out RionaRatQuestStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "riona_initial":
                if ((source.UserStatSheet.Level < 11) && (stage != RionaRatQuestStage.CompletedRatQuest))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "ratquest_initial",
                        OptionText = "Rat problem?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if ((source.UserStatSheet.Level < 11) && (stage == RionaRatQuestStage.CompletedRatQuest))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "riona_beginnerquest",
                        OptionText = "Got any other quest?"
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }


                break;

            case "ratquest_initial":
                if (!hasStage || (stage == RionaRatQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "ratquest_yes",
                        OptionText = "I can help you with that."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "ratquest_no",
                        OptionText = "Mice are friends, no thanks."
                    };

                    var option2 = new DialogOption
                    {
                        DialogKey = "ratquest_where",
                        OptionText = "Where'd they come from?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Insert(1, option1);

                    if (!Subject.HasOption(option2))
                        Subject.Options.Insert(2, option2);
                }

                if (stage == RionaRatQuestStage.StartedRatQuest)
                {
                    Subject.Reply(source, "Did you take care of those rats?");

                    {
                        var option = new DialogOption
                        {
                            DialogKey = "ratquest_turnin",
                            OptionText = "Yeah, they're gone."
                        };

                        var option1 = new DialogOption
                        {
                            DialogKey = "ratquest_refuse",
                            OptionText = "Rats are too fast"
                        };

                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);

                        if (!Subject.HasOption(option1))
                            Subject.Options.Insert(1, option1);
                    }
                }

                break;

            case "ratquest_yes":
                if (!hasStage || (stage == RionaRatQuestStage.None))
                {
                    source.Trackers.Enums.Set(RionaRatQuestStage.StartedRatQuest);
                    Subject.Reply(source, "Please kill five of these little rodents, I can't stand to look at them.");
                    source.SendOrangeBarMessage("Kill 5 tavern rats.");
                }

                break;

            case "ratquest_turnin":
                if (stage == RionaRatQuestStage.StartedRatQuest)
                {
                    if (!source.Trackers.Counters.TryGetValue("StartedRatQuest", out var value) || (value < 5))
                    {
                        Subject.Reply(source, "They're still everywhere! Please take care of them.");
                        source.SendOrangeBarMessage("You watch a rat crawl across your foot");

                        return;
                    }

                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    Subject.Reply(source, "Thank you so much for taking care of those rats!");
                    source.Trackers.Enums.Set(RionaRatQuestStage.CompletedRatQuest);
                    source.SendServerMessage(ServerMessageType.PersistentMessage, "");
                    source.Trackers.Counters.Remove("StartedRatQuest", out _);
                }

                break;
        }
    }
}