using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Quests;

public class RionaTutorialQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<RionaTutorialQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public RionaTutorialQuestScript(Dialog subject, IItemFactory itemFactory, ILogger<RionaTutorialQuestScript> logger)
        : base(subject)
    {
        ItemFactory = itemFactory;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage stage);
        source.Trackers.Flags.TryGetFlag(out RionaTutorialQuestFlags flag);

        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var fiftypercent = MathEx.GetPercentOf<int>(tnl, 50);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "riona_initial":

                if (stage is RionaTutorialQuestStage.StartedCrafting
                             or RionaTutorialQuestStage.CompletedCrafting
                             or RionaTutorialQuestStage.StartedLeveling
                             or RionaTutorialQuestStage.CompletedLeveling
                             or RionaTutorialQuestStage.CompletedTutorialQuest)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "crafting_initial",
                        OptionText = "Tell me about Crafting."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (stage is RionaTutorialQuestStage.CompletedTutorialQuest)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "hobby_explanations",
                        OptionText = "What hobbies are there?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (!hasStage || (stage == RionaTutorialQuestStage.StartedRatQuest))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "ratquest_initial",
                        OptionText = "Rat problem?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                switch (stage)
                {
                    case RionaTutorialQuestStage.CompletedRatQuest:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_SpareAStick",
                            OptionText = "I need a stick."
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.StartedSpareAStick:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_SpareAStickreturn",
                            OptionText = "Spare a Stick"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.CompletedSpareAStick:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_GetGuided",
                            OptionText = "I got a stick"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.StartedGetGuided:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_GetGuidedreturn",
                            OptionText = "I picked my class."
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.CompletedGetGuided:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_helpskarn",
                            OptionText = "What's next?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.StartedSkarn:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_helpskarnreturn",
                            OptionText = "Skarn"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.CompletedSkarn:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_beautyshop",
                            OptionText = "Need me for something else?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.StartedBeautyShop:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_beautyshopreturn",
                            OptionText = "Beauty Shop"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.CompletedBeautyShop:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_crafting",
                            OptionText = "Josephine has your hairdye."
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.StartedCrafting:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_craftingreturn",
                            OptionText = "Crafting"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.CompletedCrafting:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_levelstart",
                            OptionText = "I'm working hard."
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.StartedLeveling:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_levelstart",
                            OptionText = "Leveling"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case RionaTutorialQuestStage.CompletedLeveling:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "riona_tutorialcomplete",
                            OptionText = "I am level 11!"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                }

                break;

            case "ratquest_initial":
                if (!hasStage || (stage == RionaTutorialQuestStage.None))
                    return;

                if (stage == RionaTutorialQuestStage.StartedRatQuest)
                    Subject.Reply(source, "Skip", "ratquest_turninstart");

                break;

            case "ratquest_yes":
                if (!hasStage || (stage == RionaTutorialQuestStage.None))
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedRatQuest);
                    Subject.Reply(source, "Please kill five of these little rodents, I can't stand to look at them.");
                    source.SendOrangeBarMessage("Kill 5 tavern rats.");
                }

                break;

            case "ratquest_turnin":
                if (stage == RionaTutorialQuestStage.StartedRatQuest)
                {
                    if (!source.Trackers.Counters.TryGetValue("tavern_rat", out var value) || (value < 5))
                    {
                        Subject.Reply(source, "They're still everywhere! Please take care of them.");
                        source.SendOrangeBarMessage("You watch a rat crawl across your foot");

                        return;
                    }

                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Experience,
                              Topics.Entities.Item,
                              Topics.Entities.Dialog,
                              Topics.Entities.Quest)
                          .WithProperty(source)
                          .WithProperty(Subject)
                          .LogInformation(
                              "{@AislingName} has received {@ExpAmount} exp from a quest",
                              source.Name,
                              1000);

                    ExperienceDistributionScript.GiveExp(source, 1000);
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedRatQuest);
                    source.SendServerMessage(ServerMessageType.PersistentMessage, "");
                    source.Trackers.Counters.Remove("tavern_rat", out _);

                    Subject.Reply(
                        source,
                        "Thank you so much for taking care of those rats! There is someone else that could use your help.",
                        "riona_spareastick");
                }

                break;

            case "riona_spareastick":
            {
                if ((flag & RionaTutorialQuestFlags.SpareAStick) == RionaTutorialQuestFlags.SpareAStick)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedSpareAStick);

                    Subject.Reply(
                        source,
                        "You already did Spare a Stick I see. Good job! Let's talk about the next step then.",
                        "riona_getguided");

                    return;
                }

                if (stage == RionaTutorialQuestStage.StartedSpareAStick)
                    Subject.Reply(
                        source,
                        "Did you find Callo? He's the Mileth Weaponsmith just west of here. He's the one that has sticks.");
            }

                break;

            case "spareastick_yes":
            {
                source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedSpareAStick);
                source.SendOrangeBarMessage("Complete the Spare a Stick quest from Callo, Mileth Weaponsmith.");
            }

                break;

            case "riona_spareastickreturn":
            {
                if (flag == RionaTutorialQuestFlags.SpareAStick)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedSpareAStick);

                    Subject.Reply(
                        source,
                        "Well done! You handled that quickly. Our next focus is picking a class.",
                        "riona_getguided");
                }
            }

                break;

            case "riona_getguided":
            {
                if (source.UserStatSheet.BaseClass is BaseClass.Monk or BaseClass.Warrior or BaseClass.Wizard
                                                      or BaseClass.Rogue or BaseClass.Priest or BaseClass.Wizard)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedGetGuided);

                    Subject.Reply(
                        source,
                        "Oh, you already picked a class! Well, now I have a big ask of you...",
                        "riona_helpskarn");

                    return;
                }

                if (stage == RionaTutorialQuestStage.StartedGetGuided)
                    Subject.Reply(
                        source,
                        "Were you able to find the Temple of Choosing to pick a class? It's just up the path from here to the left past the Special Skills Masters. Once you enter the Mileth Village Way, the temple is on the right. Pick your class and return to me.");
            }

                break;

            case "getguided_yes":
            {
                source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedGetGuided);
                source.SendOrangeBarMessage("Go to the Temple of Choosing to pick a class.");
            }

                break;

            case "riona_getguidedreturn":
            {
                if (source.UserStatSheet.BaseClass is BaseClass.Monk or BaseClass.Warrior or BaseClass.Wizard
                                                      or BaseClass.Rogue or BaseClass.Priest or BaseClass.Wizard)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedGetGuided);
                    Subject.Reply(source, "Skip", "riona_helpskarn");
                }
            }

                break;

            case "riona_helpskarn":
            {
                if ((flag & RionaTutorialQuestFlags.Skarn) == RionaTutorialQuestFlags.Skarn)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedSkarn);
                    source.Trackers.Flags.RemoveFlag(RionaTutorialQuestFlags.Skarn);

                    Subject.Reply(
                        source,
                        "You already helped Skarn! That's awesome. I want you to run an errand for me then.",
                        "riona_beautyshop");

                    return;
                }

                if (stage == RionaTutorialQuestStage.StartedSkarn)
                    Subject.Reply(
                        source,
                        "Go find Skarn, he's next to the Mileth Crypt. Follow the path south of here and you'll find him.");
            }

                break;

            case "riona_helpskarn_yes":
            {
                source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedSkarn);
                source.SendOrangeBarMessage("Help Skarn, he's right outside of the Mileth Crypt.");
            }

                break;

            case "riona_helpskarnreturn":
            {
                if ((flag & RionaTutorialQuestFlags.Skarn) == RionaTutorialQuestFlags.Skarn)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedSkarn);
                    source.Trackers.Flags.RemoveFlag(RionaTutorialQuestFlags.Skarn);
                    Subject.Reply(source, "Skip", "riona_beautyshop");
                }
            }

                break;

            case "riona_beautyshop":
            {
                if (stage == RionaTutorialQuestStage.StartedBeautyShop)
                    Subject.Reply(
                        source,
                        "Josephine's Beauty Shop is in the south part of town. Go find out if she has my dye and come back please.");
            }

                break;

            case "riona_beautyshop_yes":
            {
                source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedBeautyShop);
                var mount = ItemFactory.Create("mount");

                source.Trackers.Enums.Set(CurrentMount.Horse);
                source.Trackers.Flags.AddFlag(AvailableMounts.Horse);
                source.Trackers.Enums.Set(CurrentCloak.Green);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Green);

                source.TryGiveItem(ref mount);
                source.SendOrangeBarMessage("Congrats on your first mount! Now go check on Riona's Dye!");
            }

                break;

            case "riona_beautyshopreturn":
            {
                if (stage == RionaTutorialQuestStage.CompletedBeautyShop)
                    Subject.Reply(
                        source,
                        "Oh joy! My dye is there! I will pick it up this afternoon. Thank you.",
                        "riona_crafting");
            }

                break;

            case "riona_crafting":
            {
                if ((flag & RionaTutorialQuestFlags.Crafting) == RionaTutorialQuestFlags.Crafting)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedCrafting);

                    Subject.Reply(
                        source,
                        "You started crafting already? That's fantastic. Very pro-active of you Aisling.",
                        "riona_levelstart");

                    return;
                }

                if (stage == RionaTutorialQuestStage.StartedCrafting)
                {
                    Subject.Reply(
                        source,
                        "Ask me questions about crafting, I'll tell you where to go and explain each craft in detail.");
                }
            }

                break;

            case "riona_crafting_yes":
            {
                source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedCrafting);
                source.SendOrangeBarMessage("Talk to Riona about crafting then go learn a craft.");
            }

                break;

            case "riona_craftingreturn":
            {
                if ((flag & RionaTutorialQuestFlags.Crafting) == RionaTutorialQuestFlags.Crafting)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedCrafting);
                    Subject.Reply(source, "Skip", "riona_levelstart");
                }
            }

                break;

            case "riona_levelstart":
            {
                if (stage == RionaTutorialQuestStage.CompletedCrafting)
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.StartedLeveling);

                if (source.UserStatSheet.Level >= 11)
                {
                    source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedLeveling);
                    Subject.Reply(source, "Skip", "riona_tutorialcomplete");

                    return;
                }

                if (stage == RionaTutorialQuestStage.StartedLeveling)
                    Subject.Reply(
                        source,
                        "Once you've reached level 11, come back and talk to me, I might have a gift for you.");
            }

                break;

            case "riona_tutorialcomplete":
            {
                Subject.InjectTextParameters(source.Name);

                source.Trackers.Flags.RemoveFlag(RionaTutorialQuestFlags.Crafting);
                source.Trackers.Flags.RemoveFlag(RionaTutorialQuestFlags.SpareAStick);
                source.Trackers.Flags.RemoveFlag(RionaTutorialQuestFlags.Skarn);
                source.Trackers.Flags.RemoveFlag(RionaTutorialQuestFlags.None);
                source.Trackers.Enums.Set(RionaTutorialQuestStage.CompletedTutorialQuest);

                if (source.HasClass(BaseClass.Wizard))
                {
                    var armor = source.Gender == Gender.Female
                        ? ItemFactory.Create("milethbenusta")
                        : ItemFactory.Create("milethjourneyman");

                    source.TryGiveItem(ref armor);
                }

                if (source.HasClass(BaseClass.Warrior))
                {
                    var armor = source.Gender == Gender.Female
                        ? ItemFactory.Create("milethcuirass")
                        : ItemFactory.Create("milethjupe");

                    source.TryGiveItem(ref armor);
                }

                if (source.HasClass(BaseClass.Priest))
                {
                    var armor = source.Gender == Gender.Female
                        ? ItemFactory.Create("milethmysticgown")
                        : ItemFactory.Create("milethgaluchatcoat");

                    source.TryGiveItem(ref armor);
                }

                if (source.HasClass(BaseClass.Rogue))
                {
                    var armor = source.Gender == Gender.Female
                        ? ItemFactory.Create("milethbrigandine")
                        : ItemFactory.Create("milethdwarvishleather");

                    source.TryGiveItem(ref armor);
                }

                if (source.HasClass(BaseClass.Monk))
                {
                    var armor = source.Gender == Gender.Female
                        ? ItemFactory.Create("milethlotusbodice")
                        : ItemFactory.Create("milethculotte");

                    source.TryGiveItem(ref armor);
                }

                ExperienceDistributionScript.GiveExp(source, fiftypercent);

                source.SendOrangeBarMessage("You've completed the tutorial quest!");
            }

                break;
        }
    }
}