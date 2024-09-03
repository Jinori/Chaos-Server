using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Piet;

public class PietWerewolfScript : DialogScriptBase
{
    private readonly ILogger<PietWerewolfScript> Logger;
    private readonly ISpellFactory SpellFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public PietWerewolfScript(Dialog subject, ILogger<PietWerewolfScript> logger, ISpellFactory spellFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        Logger = logger;
        SpellFactory = spellFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "toby_initial":
            {
                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed))
                    Subject.Reply(source, "Thank you Aisling for knocking me out of Werewolf Form! I tried to cure or control it but I just can't. Appie says there is no cure! I'm sorry you're now effected with the curse, go speak to Appie, the Piet Magic Shop owner, he may beable to help you.");
                
                return;
            }
            
            case "appie_initial":
            {
                if (!source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledWerewolf) && 
                    !source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard) &&
                    !source.Trackers.Enums.HasValue(WerewolfOfPiet.SpawnedWerewolf2) && 
                    !source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed) && 
                    !source.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower))
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "werewolf_initial2",
                    OptionText = "Werewolf Cure"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "werewolf_initial":
            {
                if (source.UserStatSheet.Level < 71)
                {
                    Subject.Reply(source, "Please don't mind that conversation, we will figure it out. You need to stay safe and not worry about our problems young one. Maybe when you're older, I'm sure that Werewolf will still be lurking in our town.");
                }

                break;
            }

            case "werewolf_initial2":
            {
                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed))
                {
                    Subject.Reply(source, "Skip", "werewolf_initial3");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower)
                    || source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)
                    || source.Trackers.Enums.HasValue(WerewolfOfPiet.SpawnedWerewolf2))
                {
                    Subject.Reply(source, "Skip", "werewolf_return");
                }

                break;
            }

            case "werewolf_return":
            {
                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower))
                {
                    Subject.Reply(source, "Skip", "werewolf_turnin1");
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpawnedWerewolf2))
                {
                    Subject.Reply(source, "Looks like you took a beating, that werewolf is tough. You have to go back and get that flower.");
                    source.SendOrangeBarMessage("Kill the Werewolf and retrieve the flower.");
                    source.Trackers.Enums.Set(WerewolfOfPiet.RetryWerewolf);
                    return;
                }

                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard))
                {
                    Subject.Reply(source, "Why are you back here? Go search Shinewood Forest for the Werewolf's Woods and retrieve the flower we need for the cure.");
                    source.SendOrangeBarMessage("Search Shinewood Forest for Werewolf's Woods.");
                }
                break;
            }

            case "werewolf_initial6":
            {
                source.Trackers.Enums.Set(WerewolfOfPiet.SpokeToWizard);
                source.SendOrangeBarMessage("Search Shinewood Forest for the Werewolf's Woods.");
                break;
            }

            case "werewolf_start2":
            {
                source.Trackers.Enums.Set(WerewolfOfPiet.StartedQuest);
                source.SendOrangeBarMessage("Search for the Werewolf at night.");
                break;
            }

            case "werewolf_turnin2":
            {
                var hasRequiredBlueFlower = source.Inventory.HasCount("Rose of Sharon", 1);
                
                    switch (hasRequiredBlueFlower)
                    {
                        case true:
                        {
                            source.Inventory.RemoveQuantity("Rose of Sharon", 1, out _);
                            source.Trackers.Enums.Set(WerewolfOfPiet.ReceivedCure);
                            var werewolfspell = SpellFactory.Create("werewolfform");
                            source.SpellBook.TryAdd(73, werewolfspell);

                            Logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Gold,
                                      Topics.Entities.Experience,
                                      Topics.Entities.Dialog,
                                      Topics.Entities.Quest)
                                  .WithProperty(source)
                                  .WithProperty(Subject)
                                  .LogInformation(
                                      "{@AislingName} has received {@GoldAmount} gold and {@ExpAmount} exp from Werewolf Quest",
                                      source.Name,
                                      75000,
                                      600000);

                            ExperienceDistributionScript.GiveExp(source, 600000);
                            source.TryGiveGold(75000);
                            source.TryGiveGamePoints(10);
                            
                                source.Legend.AddUnique(
                                    new LegendMark(
                                        "Learned to control the Werewolf Curse",
                                        "pietwerewolf",
                                        MarkIcon.Yay,
                                        MarkColor.Blue,
                                        1,
                                        GameTime.Now));

                            break;
                        }
                        case false:
                        {
                            Subject.Reply(source,"I know you saw the Rose of Sharon, where did it go?");
                            break;
                        }
                    }
                break;
            }

            case "werewolfwarp1":
            {
                if (source.Group == null)
                {
                    if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpawnedWerewolf2))
                    {
                        Subject.Reply(source, "Skip", "werewolfwarpreturn");
                        return;
                    }
                
                    Subject.Reply(source, "You must have a group to enter these woods.");
                    source.SendOrangeBarMessage("You must have a group to enter these woods.");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard) && source.Group != null)
                {
                    if (source.Group != null && source.Group.Any(x => !x.WithinRange(source)))
                    {
                        Subject.Reply(source, "Your group members are not nearby.");
                        return;
                    }

                    if (source.Group != null && source.Group.Any(x => !x.WithinLevelRange(source)))
                    {
                        Subject.Reply(source, "Your group members must be within your level range.");
                        return;
                    }

                    if (source.Group != null && source.Group.Any(x =>
                            !x.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard) &&
                            !x.Trackers.Enums.HasValue(WerewolfOfPiet.ReceivedCure)))
                    {
                        Subject.Reply(source,
                            "Your group members must be on this part of the quest or finished the quest to help you.");
                    }
                }
                break;
            }

            case "werewolfwarp2":
            {
                var groupmembers = source.MapInstance.GetEntities<Aisling>()
                    .Where(x => x.Group != null && x.WithinLevelRange(source) && x.WithinRange(source) && x.Group.Contains(source));

                foreach (var member in groupmembers)
                {
                    var dialog = member.ActiveDialog.Get();
                    dialog?.Close(member);
                    var rectangle = new Rectangle(12, 1, 2, 2);
                    var mapinstance = SimpleCache.Get<MapInstance>("werewolf_woods25");
                    var pointinrectangle = rectangle.GetRandomPoint();
                    member.TraverseMap(mapinstance, pointinrectangle);
                }
                break;
            }

            case "werewolfwarpreturn":
            {
                var dialog = source.ActiveDialog.Get();
                dialog?.Close(source);
                var rectangle = new Rectangle(12, 1, 2, 2);
                var mapinstance = SimpleCache.Get<MapInstance>("werewolf_woods25");
                var pointinrectangle = rectangle.GetRandomPoint();
                source.TraverseMap(mapinstance, pointinrectangle);
                
                break;
            }
        }
    }
}