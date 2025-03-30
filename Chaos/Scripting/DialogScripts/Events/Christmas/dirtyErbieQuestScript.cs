using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.Astrid;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Christmas;

public class dirtyErbieQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger, IItemFactory itemFactory)
    : DialogScriptBase(subject)
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out DirtyErbie stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var seventyfivePercent = Convert.ToInt32(.75 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "elf2_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "mtmerrybattle_initial",
                    OptionText = "Battle of Mount Merry"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "mtmerrybattle_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dirtyerbiecd", out var cdtime))
                {
                    Subject.Reply(
                        source,
                        $"The dirty erbies are being taken care of for now. We may need some more help in {cdtime.Remaining.ToReadableString()}.",
                        "elf2_initial");

                    return;
                }

                if (!hasStage || (stage == DirtyErbie.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mtmerrybattle_start1",
                        OptionText = "I will fight for you."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (hasStage && (stage == DirtyErbie.StartedErbies))
                {
                    if (!source.Trackers.Counters.TryGetValue("dirtyerbie", out var dirtyerbie) || (dirtyerbie < 20))
                    {
                        Subject.Reply(source, "There's still too many of them Dirty Erbies!", "elf2_initial");

                        return;
                    }

                    var expRewarded = source.UserStatSheet.Level == 99 ? 75000000 : seventyfivePercent;
                    ExperienceDistributionScript.GiveExp(source, expRewarded);

                    logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, expRewarded);

                    Subject.Reply(
                        source,
                        "You definitely knocked them down! They must be scattering now. Thank you Aisling for your dedication to the elves.",
                        "elf2_initial");
                    source.TryGiveGamePoints(15);
                    var stockingstuffer = ItemFactory.Create("stockingstuffer");
                    source.GiveItemOrSendToBank(stockingstuffer);
                    source.Trackers.TimedEvents.AddEvent("dirtyerbiecd", TimeSpan.FromHours(6), true);
                    source.Trackers.Counters.Remove("dirtyerbie", out _);
                    source.Trackers.Enums.Set(DirtyErbie.None);
                }

                break;
            }

            case "mtmerrybattle_start2":
            {
                source.Trackers.Enums.Set(DirtyErbie.StartedErbies);
                source.SendOrangeBarMessage("Take down 20 Dirty Erbies.");
            }

                break;
        }
    }
}