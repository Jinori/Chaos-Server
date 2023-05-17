using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests;

public class RionaRatQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public RionaRatQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

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
                    return;

                if (stage == RionaRatQuestStage.StartedRatQuest)
                {
                    Subject.Reply(source, "Skip", "ratquest_turninstart");

                    return;
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
                    
                    var mount = ItemFactory.Create("Mount");
                    source.TryGiveItem(mount);
                    source.TryGiveGamePoints(5);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    Subject.Reply(source, "Thank you so much for taking care of those rats!");
                    source.Trackers.Enums.Set(RionaRatQuestStage.CompletedRatQuest);
                    source.SendServerMessage(ServerMessageType.PersistentMessage, "");
                    source.Trackers.Counters.Remove("StartedRatQuest", out _);
                    source.Trackers.Flags.AddFlag(AvailableMounts.WhiteHorse);
                }

                break;
        }
    }
}