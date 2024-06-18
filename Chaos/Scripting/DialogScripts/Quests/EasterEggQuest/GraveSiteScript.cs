using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.EasterEggQuest;

public class GraveSiteScript (Dialog subject, ILogger<GraveSiteScript> logger, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bellagrave_initial":
            {
                if (source.Trackers.Enums.HasValue(GraveSite.CompletedQuest))
                {
                    Subject.Reply(source, "You quickly glance at Bella's grave and pay your respects.");
                    return;
                }
                if (source.Trackers.Enums.HasValue(GraveSite.StartedQuest))
                {
                    Subject.Reply(source, "Skip", "bellagrave_return");
                }

                if (source.Trackers.Enums.HasValue(GraveSite.ReadTheHeadstone))
                {
                    Subject.Reply(source, "Skip", "bellagrave_initial2");
                }

                break;
            }

            case "bellagrave_start1":
            {
                Subject.Close(source);
                source.Client.SendServerMessage(
                    ServerMessageType.ScrollWindow,
                    "{=u          IN LOVING MEMORY OF BELLA{=g\n\n        A Beloved Daughter and Friend\n\n        Born in Piet, a heart so kind,\n      In Loures Castle, her light did shine.\n        Taken too soon, in a tragic fate,\n      Her memory and love, eternally great.\n\n{=u  *Note* Bella's Favorite Flower was Pink Roses.");
                source.Trackers.Enums.Set(GraveSite.ReadTheHeadstone);
                break;
            }

            case "bellagrave_start2":
            {
                source.Trackers.Enums.Set(GraveSite.StartedQuest);
                source.SendOrangeBarMessage("Bring five pink roses to Bella's Grave.");
                break;
            }

            case "bellagrave_turnin":
            {
                var hasRequiredPinkRoses = source.Inventory.HasCount("Pink Rose", 5);

                if (source.Trackers.Enums.HasValue(GraveSite.StartedQuest))
                {
                    switch (hasRequiredPinkRoses)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Pink Rose", 5, out _);
                            
                            logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@ExpAmount} exp from a quest",
                                      source.Name,
                                      125000);

                            ExperienceDistributionScript.GiveExp(source, 125000);
                            source.TryGiveGamePoints(10);
                            source.Trackers.Flags.AddFlag(EasterEggs.GraveSite);
                            var toydoll = itemFactory.Create("faewings");
                            source.GiveItemOrSendToBank(toydoll);
                            source.SendOrangeBarMessage("You lay the Pink Roses at Bella's Grave...");
                            break;
                        }
                        case false:
                        {
                            var pinkrosecount = source.Inventory.CountOf("Pink Rose");

                            Subject.Reply(source,
                                $"{pinkrosecount} Pink Roses are not a full bouquet. Please bring back 5 Pink Roses.");

                            break;
                        }
                    }
                }
                break;
            }
        }
    } 
}