using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.HalloweenEvents.Senaan;

public class SenaanDialogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public SenaanDialogScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "senaan_initial":
            {
                if (source.Trackers.Flags.HasFlag(SenaanFlagsHalloweenEvent.CompletedYear1))
                {
                    Subject.Reply(source, "You've already scared me this year, I'm not going to be scared by you again.");

                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "senaan_initial1",
                    OptionText = "Do I scare you?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "senaan_initial1":
            {
                if (source.Equipment.ContainsByTemplateKey("ghostface"))
                {
                    Subject.Reply(
                        source,
                        "Woah! You're creepy to look at! That kinda got me scared. Ghost are not my favorite. Here take this as a reward.");

                    if (source.UserStatSheet.Level >= 99)
                        ExperienceDistributionScript.GiveExp(source, 10000000);
                    else
                        ExperienceDistributionScript.GiveExp(source, twentyPercent);

                    var item = ItemFactory.Create("lionbuddy");
                    source.GiveItemOrSendToBank(item);
                    source.TryGiveGamePoints(20);
                    source.Trackers.Flags.AddFlag(SenaanFlagsHalloweenEvent.CompletedYear1);

                    return;
                }

                Subject.Reply(source, "That doesn't scare me.");

                break;
            }
        }
    }
}