using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Mileth;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Abel;

public class DragonScaleQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<SpareAStickScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public DragonScaleQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<SpareAStickScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out DragonScale stage);
        var hasFlags = source.Trackers.Flags.TryGetFlag(out DragonScaleFlags flag);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "callo_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dragonscalecallo_initial",
                    OptionText = "Dragon Scale"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "dragonscalecallo_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dragonscalewait", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I am not done yet with your Dragon Scale Sword. Please return to me in {cdtime.Remaining.ToReadableString()}");
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleSword)
                {
                    Subject.Reply(source, "Skip", "dragonscale_pickup");
                    return;
                }

                if (hasStage && stage == DragonScale.CompletedDragonScale)
                {
                    Subject.Reply(source, $"You have conquered the power of the Dragon Scale. Use it wisely.");
                    return;
                }

                if (source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo))
                {
                    Subject.Reply(source, "Skip", "dragonscalecallo_return");
                    return;
                }

                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    Subject.Reply(source, "Skip", "dragonscalesword_turnin");
                }

                break;
            }

            case "dragonscalecallo_start2":
            {
                source.Trackers.Flags.AddFlag(DragonScaleFlags.Callo);

                var hasallFlags = source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo);

                if (hasallFlags)
                {
                    source.Trackers.Enums.Set(DragonScale.FoundAllClues);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);
                    source.SendOrangeBarMessage("That was the last piece of advice from Weaponsmiths.");
                }
                else
                {
                    source.SendOrangeBarMessage("Speak to the other weaponsmith to gather information.");
                }

                break;
            }

            case "dragonscalesword_turnin3":
            {
                var hasRequiredDragonScale = source.Inventory.HasCount("Dragon's Scale", 1);


                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    if (hasRequiredDragonScale && source.Gold >= 75000)
                    {
                        source.TryTakeGold(75000);
                        source.Inventory.RemoveQuantity("Dragon's Scale", 1, out _);
                        source.Trackers.Enums.Set(DragonScale.TurnedInScaleSword);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);

                        source.Trackers.TimedEvents.AddEvent("dragonscalewait", TimeSpan.FromHours(1), true);

                        Logger.WithTopics(
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
                                150000);

                        ExperienceDistributionScript.GiveExp(source, 150000);
                        source.TryGiveGamePoints(10);

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Harnessed the power of the Dragon Scale",
                                "dragonScale",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));
                    }

                    if (!hasRequiredDragonScale || source.Gold < 75000)
                    {
                        Subject.Reply(source,
                            "You are missing the Dragon Scale or my fee of 75,000 gold. I require both to forge the Dragon Scale Sword.");
                        return;
                    }
                }

                break;
            }

            case "dragonscale_pickup":
            {
                if (hasStage && stage == DragonScale.TurnedInScaleSword)
                {
                    source.Trackers.Enums.Set(DragonScale.CompletedDragonScale);
                    Subject.Reply(source, "Thank you for waiting, here is your Dragon Scale Sword.");
                    var dragonScaleSword = ItemFactory.Create("dragonscalesword");

                    source.TryGiveItem(ref dragonScaleSword);
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleRing)
                {
                    source.Trackers.Enums.Set(DragonScale.CompletedDragonScale);
                    Subject.Reply(source, "Thank you for waiting, here is your Dragon Scale Ring.");
                    var dragonScaleRing = ItemFactory.Create("dragonscalering");

                    source.TryGiveItem(ref dragonScaleRing);
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleClaws)
                {
                    source.Trackers.Enums.Set(DragonScale.CompletedDragonScale);
                    Subject.Reply(source, "Thank you for waiting, here is your Dragon Scale Claws.");
                    var dragonScaleClaws = ItemFactory.Create("dragonscaleclaws");

                    source.TryGiveItem(ref dragonScaleClaws);
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleGauntlet)
                {
                    source.Trackers.Enums.Set(DragonScale.CompletedDragonScale);
                    Subject.Reply(source, "Thank you for waiting, here is your Dragon Scale Gauntlet.");
                    var dragonScaleGauntlet = ItemFactory.Create("dragonscalegauntlet");

                    source.TryGiveItem(ref dragonScaleGauntlet);
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleDagger)
                {
                    source.Trackers.Enums.Set(DragonScale.CompletedDragonScale);
                    Subject.Reply(source, "Thank you for waiting, here is your Dragon Scale Dagger.");
                    var dragonScaleDagger = ItemFactory.Create("dragonscaledagger");

                    source.TryGiveItem(ref dragonScaleDagger);
                    return;
                }


                break;
            }

            case "vidar_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dragonscalevidar_initial",
                    OptionText = "Dragon Scale"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }
            case "dragonscalevidar_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dragonscalewait", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I am not done yet with your Dragon Scale Ring. Please return to me in {cdtime.Remaining.ToReadableString()}");
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleRing)
                {
                    Subject.Reply(source, "Skip", "dragonscale_pickup");
                    return;
                }

                if (hasStage && stage == DragonScale.CompletedDragonScale)
                {
                    Subject.Reply(source, $"You have conquered the power of the Dragon Scale. Use it wisely.");
                    return;
                }

                if (source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar))
                {
                    Subject.Reply(source, "Skip", "dragonscalevidar_return");
                    return;
                }

                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    Subject.Reply(source, "Skip", "dragonscalering_turnin");
                }

                break;
            }

            case "dragonscalevidar_start2":
            {
                source.Trackers.Flags.AddFlag(DragonScaleFlags.Vidar);

                var hasallFlags = source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo);

                if (hasallFlags)
                {
                    source.Trackers.Enums.Set(DragonScale.FoundAllClues);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);
                    source.SendOrangeBarMessage("That was the last piece of advice from Weaponsmiths.");
                }
                else
                {
                    source.SendOrangeBarMessage("Speak to the other weaponsmith to gather information.");
                }

                break;
            }

            case "dragonscalering_turnin3":
            {
                var hasRequiredDragonScale = source.Inventory.HasCount("Dragon's Scale", 1);


                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    if (hasRequiredDragonScale && source.Gold >= 75000)
                    {
                        source.TryTakeGold(75000);
                        source.Inventory.RemoveQuantity("Dragon's Scale", 1, out _);
                        source.Trackers.Enums.Set(DragonScale.TurnedInScaleRing);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);

                        source.Trackers.TimedEvents.AddEvent("dragonscalewait", TimeSpan.FromHours(1), true);

                        Logger.WithTopics(
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
                                150000);

                        ExperienceDistributionScript.GiveExp(source, 150000);
                        source.TryGiveGamePoints(10);

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Harnessed the power of the Dragon Scale",
                                "dragonScale",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));
                    }

                    if (!hasRequiredDragonScale || source.Gold < 75000)
                    {
                        Subject.Reply(source,
                            "You are missing the Dragon Scale or my fee of 75,000 gold. I require both to forge the Dragon Scale Ring.");
                        return;
                    }
                }

                break;
            }
            case "marcelo_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dragonscalemarcelo_initial",
                    OptionText = "Dragon Scale"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }
            case "dragonscalemarcelo_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dragonscalewait", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I am not done yet with your Dragon Scale Claws. Please return to me in {cdtime.Remaining.ToReadableString()}");
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleClaws)
                {
                    Subject.Reply(source, "Skip", "dragonscale_pickup");
                    return;
                }

                if (hasStage && stage == DragonScale.CompletedDragonScale)
                {
                    Subject.Reply(source, $"You have conquered the power of the Dragon Scale. Use it wisely.");
                    return;
                }

                if (source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo))
                {
                    Subject.Reply(source, "Skip", "dragonscalemarcelo_return");
                    return;
                }

                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    Subject.Reply(source, "Skip", "dragonscaleclaws_turnin");
                }

                break;
            }

            case "dragonscalemarcelo_start2":
            {
                source.Trackers.Flags.AddFlag(DragonScaleFlags.Marcelo);

                var hasallFlags = source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo);

                if (hasallFlags)
                {
                    source.Trackers.Enums.Set(DragonScale.FoundAllClues);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);
                    source.SendOrangeBarMessage("That was the last piece of advice from Weaponsmiths.");
                }
                else
                {
                    source.SendOrangeBarMessage("Speak to the other weaponsmith to gather information.");
                }

                break;
            }

            case "dragonscaleclaws_turnin3":
            {
                var hasRequiredDragonScale = source.Inventory.HasCount("Dragon's Scale", 1);


                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    if (hasRequiredDragonScale && source.Gold >= 75000)
                    {
                        source.TryTakeGold(75000);
                        source.Inventory.RemoveQuantity("Dragon's Scale", 1, out _);
                        source.Trackers.Enums.Set(DragonScale.TurnedInScaleClaws);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);

                        source.Trackers.TimedEvents.AddEvent("dragonscalewait", TimeSpan.FromHours(1), true);

                        Logger.WithTopics(
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
                                150000);

                        ExperienceDistributionScript.GiveExp(source, 150000);
                        source.TryGiveGamePoints(10);

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Harnessed the power of the Dragon Scale",
                                "dragonScale",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));
                    }

                    if (!hasRequiredDragonScale || source.Gold < 75000)
                    {
                        Subject.Reply(source,
                            "You are missing the Dragon Scale or my fee of 75,000 gold. I require both to forge the Dragon Scale Claws.");
                        return;
                    }
                }

                break;
            }
            case "avel_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dragonscaleavel_initial",
                    OptionText = "Dragon Scale"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "dragonscaleavel_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dragonscalewait", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I am not done yet with your Dragon Scale Gauntlet. Please return to me in {cdtime.Remaining.ToReadableString()}");
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleGauntlet)
                {
                    Subject.Reply(source, "Skip", "dragonscale_pickup");
                    return;
                }

                if (hasStage && stage == DragonScale.CompletedDragonScale)
                {
                    Subject.Reply(source, $"You have conquered the power of the Dragon Scale. Use it wisely.");
                    return;
                }

                if (source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel))
                {
                    Subject.Reply(source, "Skip", "dragonscaleavel_return");
                    return;
                }

                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    Subject.Reply(source, "Skip", "dragonscalegauntlet_turnin");
                }

                break;
            }

            case "dragonscaleavel_start2":
            {
                source.Trackers.Flags.AddFlag(DragonScaleFlags.Avel);

                var hasallFlags = source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo);

                if (hasallFlags)
                {
                    source.Trackers.Enums.Set(DragonScale.FoundAllClues);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);
                    source.SendOrangeBarMessage("That was the last piece of advice from Weaponsmiths.");
                }
                else
                {
                    source.SendOrangeBarMessage("Speak to the other weaponsmith to gather information.");
                }

                break;
            }

            case "dragonscalegauntlet_turnin3":
            {
                var hasRequiredDragonScale = source.Inventory.HasCount("Dragon's Scale", 1);


                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    if (hasRequiredDragonScale && source.Gold >= 75000)
                    {
                        source.TryTakeGold(75000);
                        source.Inventory.RemoveQuantity("Dragon's Scale", 1, out _);
                        source.Trackers.Enums.Set(DragonScale.TurnedInScaleGauntlet);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);

                        source.Trackers.TimedEvents.AddEvent("dragonscalewait", TimeSpan.FromHours(1), true);

                        Logger.WithTopics(
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
                                150000);

                        ExperienceDistributionScript.GiveExp(source, 150000);
                        source.TryGiveGamePoints(10);

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Harnessed the power of the Dragon Scale",
                                "dragonScale",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));
                    }

                    if (!hasRequiredDragonScale || source.Gold < 75000)
                    {
                        Subject.Reply(source,
                            "You are missing the Dragon Scale or my fee of 75,000 gold. I require both to forge the Dragon Scale Gauntlet.");
                        return;
                    }
                }

                break;
            }
            case "torbjorn_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dragonscaletorbjorn_initial",
                    OptionText = "Dragon Scale"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "dragonscaletorbjorn_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dragonscalewait", out var cdtime))
                {
                    Subject.Reply(source,
                        $"I am not done yet with your Dragon Scale Dagger. Please return to me in {cdtime.Remaining.ToReadableString()}");
                    return;
                }

                if (hasStage && stage == DragonScale.TurnedInScaleDagger)
                {
                    Subject.Reply(source, "Skip", "dragonscale_pickup");
                    return;
                }

                if (hasStage && stage == DragonScale.CompletedDragonScale)
                {
                    Subject.Reply(source, $"You have conquered the power of the Dragon Scale. Use it wisely.");
                    return;
                }

                if (source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn))
                {
                    Subject.Reply(source, "Skip", "dragonscaletorbjorn_return");
                    return;
                }

                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    Subject.Reply(source, "Skip", "dragonscaledagger_turnin");
                }

                break;
            }

            case "dragonscaletorbjorn_start2":
            {
                source.Trackers.Flags.AddFlag(DragonScaleFlags.Torbjorn);
                var hasallFlags = source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo);

                if (hasallFlags)
                {
                    source.Trackers.Enums.Set(DragonScale.FoundAllClues);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);
                    source.SendOrangeBarMessage("That was the last piece of advice from Weaponsmiths.");
                }
                else
                {
                    source.SendOrangeBarMessage("Speak to the other weaponsmith to gather information.");
                }

                break;
            }

            case "dragonscaledagger_turnin3":
            {
                var hasRequiredDragonScale = source.Inventory.HasCount("Dragon's Scale", 1);


                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    if (hasRequiredDragonScale && source.Gold >= 75000)
                    {
                        source.TryTakeGold(75000);
                        source.Inventory.RemoveQuantity("Dragon's Scale", 1, out _);
                        source.Trackers.Enums.Set(DragonScale.TurnedInScaleDagger);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                        source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);

                        source.Trackers.TimedEvents.AddEvent("dragonscalewait", TimeSpan.FromHours(1), true);

                        Logger.WithTopics(
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
                                150000);

                        ExperienceDistributionScript.GiveExp(source, 150000);
                        source.TryGiveGamePoints(10);

                        source.Legend.AddOrAccumulate(
                            new LegendMark(
                                "Harnessed the power of the Dragon Scale",
                                "dragonScale",
                                MarkIcon.Victory,
                                MarkColor.White,
                                1,
                                GameTime.Now));
                    }

                    if (!hasRequiredDragonScale || source.Gold < 75000)
                    {
                        Subject.Reply(source,
                            "You are missing the Dragon Scale or my fee of 75,000 gold. I require both to forge the Dragon Scale Dagger.");
                        return;
                    }
                }

                break;
            }
            case "gunnar_initial":
            {
                if (source.UserStatSheet.Level < 41)
                    return;

                var option = new DialogOption
                {
                    DialogKey = "dragonscalegunnar_initial",
                    OptionText = "Dragon Scale"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                break;
            }

            case "dragonscalegunnar_initial":
            {
                if (hasStage && stage == DragonScale.CompletedDragonScale)
                {
                    Subject.Reply(source, $"You have conquered the power of the Dragon Scale. Use it wisely.");
                    return;
                }

                if (source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar))
                {
                    Subject.Reply(source, "Skip", "dragonscalegunnar_return");
                    return;
                }

                if (hasStage && stage == DragonScale.KilledDragon)
                {
                    Subject.Reply(source,
                        "That's great you retrieved the Dragon Scale! Unfortunately, I have no knowledge of how to forge anything with it. Speak to the other weaponsmith, I'm sure they'll be happy to help.");
                }

                break;
            }

            case "dragonscalegunnar_start2":
            {
                source.Trackers.Flags.AddFlag(DragonScaleFlags.Gunnar);

                var hasallFlags = source.Trackers.Flags.HasFlag(DragonScaleFlags.Avel)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Callo)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Torbjorn)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Gunnar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Vidar)
                                  && source.Trackers.Flags.HasFlag(DragonScaleFlags.Marcelo);

                if (hasallFlags)
                {
                    source.Trackers.Enums.Set(DragonScale.FoundAllClues);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Callo);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Avel);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Torbjorn);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Gunnar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Vidar);
                    source.Trackers.Flags.RemoveFlag(DragonScaleFlags.Marcelo);
                    source.SendOrangeBarMessage("That was the last piece of advice from Weaponsmiths.");
                }
                else
                {
                    source.SendOrangeBarMessage("Speak to the other weaponsmith to gather information.");
                }

                break;
            }
        }
    }
}