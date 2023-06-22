using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Quests;

public class BeggarFoodQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public BeggarFoodQuestScript(Dialog subject)
        : base(subject) =>
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out BeggarFoodQuestStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "beggar_initial":
                if (!hasStage)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "beggarfood_initial",
                        OptionText = "Are you hungry?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (stage == BeggarFoodQuestStage.Started)
                {
                    Subject.Reply(source, "Skip", "beggarfood_turninstart");

                    return;
                }

                if (stage == BeggarFoodQuestStage.Completed)
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("BeggarFood", out var timedEvent))
                    {
                        Subject.Reply(source, $"Thank you again for the dinner plate, it was delicious! I hope you come back tomorrow. \n(({timedEvent.Remaining.ToReadableString()}))");

                        return;
                    }
                    source.Trackers.Enums.Remove(typeof(BeggarFoodQuestStage));
                    Subject.Reply(source, "Skip", "beggarfood_initial");
                }


                break;

            case "beggarfood_initial":
                if (!hasStage || (stage == BeggarFoodQuestStage.None))
                    return;

                if (stage == BeggarFoodQuestStage.Started)
                {
                    Subject.Reply(source, "Skip", "beggarfood_turninstart");
                }

                break;

            case "beggarfood_yes":
                if (!hasStage || (stage == BeggarFoodQuestStage.None))
                {
                    source.Trackers.Enums.Set(BeggarFoodQuestStage.Started);
                    Subject.Reply(source, "I'd really appreciate one of those dinner plates... Could you cook me one?");
                    source.SendOrangeBarMessage("Bring the beggar a dinner plate.");
                }

                break;

            case "beggarfood_turnin":
                if (stage == BeggarFoodQuestStage.Started)
                {
                    if (!source.Inventory.Remove("dinnerplate"))
                    {
                        Subject.Reply(source, "Ah, my stomach, its grumbling...");
                        source.SendOrangeBarMessage("You don't have the dinner plate.");

                        return;
                    }

                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, 25000);
                    Subject.Reply(source, "That's exactly what I needed, delicious! I wonder where I'll get my next meal. Thank you kind Aisling.");
                    source.Trackers.Enums.Set(BeggarFoodQuestStage.Completed);
                    source.Trackers.TimedEvents.AddEvent("BeggarFood", TimeSpan.FromHours(24), true);
                }

                break;
        }
    }
}