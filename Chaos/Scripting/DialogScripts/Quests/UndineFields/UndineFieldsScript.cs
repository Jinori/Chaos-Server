using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.UndineFields;

public class UndineFieldsScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public UndineFieldsScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.Level < 41)
            return;
        
        var hasStage = source.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alexios_initial":
            {

                if (stage == UndineFieldDungeon.CompletedUF)
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "undinefields_initial",
                    OptionText = "What's in the Undine Fields?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;
                
                case "undinefields_initial":
            {
                if (stage == UndineFieldDungeon.StartedDungeon)
                {
                    Subject.Reply(source, "Skip", "uf_return");

                    return;
                }

                if (stage == UndineFieldDungeon.KilledCarnun)
                {
                    Subject.Reply(source, "Skip", "uf_turnin");

                    return;
                }
            }

                break;

            case "uf_yes":
                if (!hasStage || (stage == UndineFieldDungeon.None))
                    source.Trackers.Enums.Set(UndineFieldDungeon.StartedDungeon);

                break;

            case "uf_turnin":
                if (stage == UndineFieldDungeon.KilledCarnun)
                {
                    source.Trackers.Enums.Set(UndineFieldDungeon.CompletedUF);
                    ExperienceDistributionScript.GiveExp(source, 200000);
                    source.Trackers.Flags.AddFlag(AvailableCloaks.Red);
                    source.SendOrangeBarMessage("You unlocked the Red Cloak for mounts!");
                    
                    Logger.WithTopics(
                            Topics.Entities.Aisling,
                            Topics.Entities.Experience,
                            Topics.Entities.Item,
                            Topics.Entities.Dialog,
                            Topics.Entities.Quest)
                        .WithProperty(source)
                        .WithProperty(Subject)
                        .LogInformation(
                            "{@AislingName} has received {@ExpAmount} exp and Red Cloak from Undine Fields Dungeon",
                            source.Name,
                            200000);
                }
                break;
        }
    }
}