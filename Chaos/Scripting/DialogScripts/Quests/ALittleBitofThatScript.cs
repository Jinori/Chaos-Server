using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Quests;

public class ALittleBitofThatScript : DialogScriptBase
{
    private readonly ILogger<ALittleBitofThatScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public ALittleBitofThatScript(Dialog subject, ILogger<ALittleBitofThatScript> logger)
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out ALittleBitofThatStage stage);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = Convert.ToInt32(.20 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "egil_initial":
            {
                if (source.UserStatSheet.Level < 21)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "alittlebitofthat_initial",
                        OptionText = "A Little Bit of That"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }

                break;

            case "alittlebitofthat_initial":
                if (!hasStage || (stage == ALittleBitofThatStage.None))
                    if (source.Trackers.TimedEvents.HasActiveEvent("ALittleBitofThatCd", out var timedEvent))
                    {
                        Subject.Reply(source, $"I'm still working on this, come back later. (({timedEvent.Remaining.ToReadableString()}))");

                        return;
                    }

                if (stage == ALittleBitofThatStage.StartedApple)
                {
                    Subject.Reply(source, "skip", "alittlebitofthat_applestart");

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedBaguette)
                {
                    Subject.Reply(source, "Skip", "alittlebitofthat_baguettestart");

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedCherry)
                {
                    Subject.Reply(source, "Skip", "alittlebitofthat_cherrystart");

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedGrapes)
                {
                    Subject.Reply(source, "Skip", "alittlebitofthat_grapesstart");

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedMold)
                {
                    Subject.Reply(source, "skip", "alittlebitofthat_Moldstart");

                    return;
                }

                if (stage == ALittleBitofThatStage.StartedTomato)
                    Subject.Reply(source, "Skip", "alittlebitofthat_tomatostart");

                break;

            case "alittlebitofthat_yes":
                if (!hasStage || (stage == ALittleBitofThatStage.None))
                {
                    var randomALittleBitofThatStage = new[]
                    {
                        ALittleBitofThatStage.StartedApple,
                        ALittleBitofThatStage.StartedBaguette,
                        ALittleBitofThatStage.StartedCherry,
                        ALittleBitofThatStage.StartedGrapes,
                        ALittleBitofThatStage.StartedMold,
                        ALittleBitofThatStage.StartedTomato
                    }.PickRandom();

                    source.Trackers.Enums.Set(randomALittleBitofThatStage);

                    switch (randomALittleBitofThatStage)
                    {
                        case ALittleBitofThatStage.StartedApple:
                        {
                            Subject.Reply(source, "I really appreciate your help Aisling, please bring me five Apples.");
                        }

                            break;

                        case ALittleBitofThatStage.StartedBaguette:
                        {
                            Subject.Reply(source, "I really appreciate your help Aisling, please bring me three Baguette.");
                        }

                            break;

                        case ALittleBitofThatStage.StartedCherry:
                        {
                            Subject.Reply(source, "I really appreciate your help Aisling, please bring me ten Cherries.");
                        }

                            break;
                        case ALittleBitofThatStage.StartedMold:
                        {
                            Subject.Reply(source, "I really appreciate your help Aisling, please bring me three Mold.");
                        }

                            break;

                        case ALittleBitofThatStage.StartedTomato:
                        {
                            Subject.Reply(source, "I really appreciate your help Aisling, please bring me a Tomato.");
                        }

                            break;
                        case ALittleBitofThatStage.StartedGrapes:
                        {
                            Subject.Reply(source, "I really appreciate your help Aisling, please bring me ten Grapes.");
                        }

                            break;
                    }
                }

                break;

            case "alittlebitofthat_apple":

                if (stage == ALittleBitofThatStage.StartedApple)
                {
                    if (!source.Inventory.HasCount("Apple", 5))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some apples, you remember he needs five.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, twentyPercent);

                    source.Inventory.RemoveQuantity("Apple", 5);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Trackers.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("ALittleBitofThatCd", TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_baguette":
                if (stage == ALittleBitofThatStage.StartedBaguette)
                {
                    if (!source.Inventory.HasCount("Baguette", 3))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some baguettes, you remember he needs three.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, twentyPercent);

                    source.Inventory.RemoveQuantity("baguette", 3);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Trackers.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("ALittleBitofThatCd", TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_cherry":
                if (stage == ALittleBitofThatStage.StartedCherry)
                {
                    if (!source.Inventory.HasCount("Cherry", 10))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some cherries, you remember he needs ten.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, twentyPercent);

                    source.Inventory.RemoveQuantity("Cherry", 10);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Trackers.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("ALittleBitofThatCd", TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_grape":
                if (stage == ALittleBitofThatStage.StartedGrapes)
                {
                    if (!source.Inventory.HasCount("Grape", 10))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some grapes, you remember he needs ten.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, twentyPercent);

                    source.Inventory.RemoveQuantity("Grape", 10);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Trackers.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("ALittleBitofThatCd", TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_mold":
                if (stage == ALittleBitofThatStage.StartedMold)
                {
                    if (!source.Inventory.HasCount("Mold", 3))
                    {
                        source.SendOrangeBarMessage("You seem to be missing some mold, you remember he needs three.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, twentyPercent);

                    source.Inventory.RemoveQuantity("Mold", 3);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Trackers.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("ALittleBitofThatCd", TimeSpan.FromHours(2), true);
                }

                break;

            case "alittlebitofthat_tomato":
                if (stage == ALittleBitofThatStage.StartedTomato)
                {
                    if (!source.Inventory.HasCount("Tomato", 1))
                    {
                        source.SendOrangeBarMessage("You seem to be missing a Tomato.");
                        Subject.Close(source);

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, twentyPercent);

                    source.Inventory.RemoveQuantity("Tomato", 1);
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.Trackers.Enums.Set(ALittleBitofThatStage.None);
                    Subject.Close(source);
                    source.Trackers.TimedEvents.AddEvent("ALittleBitofThatCd", TimeSpan.FromHours(2), true);
                }

                break;
        }
    }
}