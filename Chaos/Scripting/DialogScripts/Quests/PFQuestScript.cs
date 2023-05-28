using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests;

public class PFQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public PFQuestScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out PFQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "porteforest_initial":
            {
                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest1",
                        OptionText = "What do you know?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (stage == PFQuestStage.StartedPFQuest)
                {
                    Subject.Reply(source, "Skip", "porteforest_rootturninstart");

                    return;
                }

                if (stage == PFQuestStage.TurnedInRoots)
                {
                    Subject.Reply(source, "Skip", "porteforest_repeatstart");

                    return;
                }

                if (stage is PFQuestStage.CompletedPFQuest or PFQuestStage.TurnedInTristar)
                    Subject.Reply(source,
                        "Well done Aisling, I'm glad that beast won't bother us again. I can go farm my own trent roots again. Thank you.");
            }

                break;

            case "porteforest_quest2":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_yes",
                    OptionText = "I can do that."
                };

                var option1 = new DialogOption
                {
                    DialogKey = "porteforest_no",
                    OptionText = "Not gonna happen, see ya."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(1, option1);
            }

                break;

            case "porteforest_yes":
                if (!hasStage || (stage == PFQuestStage.None))
                    source.Trackers.Enums.Set(PFQuestStage.StartedPFQuest);

                break;

            case "porteforest_rootturnin":
                if (stage == PFQuestStage.StartedPFQuest)
                {
                    if (!source.Inventory.HasCount("trent root", 4))
                    {
                        Subject.Reply(source, "Can you bring some more, this isn't enough.");
                        source.SendOrangeBarMessage("Torbjorn isn't impressed. He wants four trent roots.");

                        return;
                    }

                    source.Inventory.RemoveQuantity("trent root", 4);
                    source.Trackers.Enums.Set(PFQuestStage.TurnedInRoots);
                    ExperienceDistributionScript.GiveExp(source, 100000);
                }

                break;

            case "porteforest_initial2":
                if (stage == PFQuestStage.TurnedInRoots)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest3",
                        OptionText = "Torbjorn sent me."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                if (stage == PFQuestStage.WolfManes)
                {
                    Subject.Reply(source, "Skip", "porteforest_wolfmanesstart");

                    return;
                }

                if (stage is PFQuestStage.CompletedPFQuest or PFQuestStage.TurnedInTristar)
                    Subject.Reply(source,
                        "It is so great to hear about your battle with the Giant Mantis. I can't believe you took it down. Praise you Aisling.");

                break;

            case "porteforest_quest3":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_quest4",
                    OptionText = "You dropped the pendant and you may have more information."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }

                break;

            case "porteforest_quest4":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_quest5",
                    OptionText = "Yes, please tell me."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }

                break;

            case "porteforest_quest5":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_yes2",
                    OptionText = "I will go get them."
                };

                var option1 = new DialogOption
                {
                    DialogKey = "porteforest_no2",
                    OptionText = "No way, I don't need it."
                };

                var option2 = new DialogOption
                {
                    DialogKey = "porteforest_explain",
                    OptionText = "What if I don't?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);

                if (!Subject.HasOption(option1.OptionText))
                    Subject.Options.Insert(1, option1);

                if (!Subject.HasOption(option2.OptionText))
                    Subject.Options.Insert(2, option2);
            }

                break;

            case "porteforest_yes2":
                if (stage == PFQuestStage.TurnedInRoots)
                {
                    source.Trackers.Enums.Set(PFQuestStage.WolfManes);
                    source.SendOrangeBarMessage("Return to Bertil with five Silver Wolf Mane Hairs.");
                }

                break;

            case "porteforest_wolfmanes":
                if (stage == PFQuestStage.WolfManes)
                {
                    if (!source.Inventory.HasCount("Silver Wolf Mane Hair", 5))
                    {
                        Subject.Reply(source,
                            "You don't have enough Aisling, I need at least five Silver Wolf Mane Hair to make the Silver Wolf Leather.");

                        return;
                    }

                    Subject.Reply(source,
                        "Great work Aisling! Here is your Silver Wolf Leather, that should help you fight that beast off. Go talk to Isabelle again, she may know where to go with it.");

                    source.Inventory.RemoveQuantity("Silver Wolf Mane Hair", 5);

                    var leather = ItemFactory.Create("silverwolfleather");
                    source.Trackers.Enums.Set(PFQuestStage.WolfManesTurnedIn);
                    ExperienceDistributionScript.GiveExp(source, 150000);
                    source.TryGiveItem(leather);
                }

                break;

            case "isabelle_initial":
                if (!hasStage || (stage == PFQuestStage.None))
                {
                    if (source.UserStatSheet.Level is <= 10 or >= 42)
                        return;

                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_initial3",
                        OptionText = "Porte Forest"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                switch (stage)
                {
                    case PFQuestStage.TurnedInRoots or PFQuestStage.WolfManesTurnedIn:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "porteforest_pendant",
                            OptionText = "Turuc Pendant"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);

                        break;
                    }
                    case PFQuestStage.FoundPendant:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "porteforest_pendant3",
                            OptionText = "Turuc Pendant"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Add(option);

                        break;
                    }
                    case PFQuestStage.CompletedPFQuest or PFQuestStage.TurnedInTristar:
                        Subject.Reply(source, "Thank you so much Aisling, I can enjoy the peak again! And all the flowers and bunnies!");

                        break;
                }

                break;

            case "porteforest_initial3":
                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest6",
                        OptionText = "What did you see?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "porteforest_quest6":
                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest7",
                        OptionText = "Did they get away safely?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;

            case "porteforest_pendant":
                if (stage is PFQuestStage.TurnedInRoots or PFQuestStage.WolfManesTurnedIn)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant2",
                        OptionText = "I'll go get it now."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;

            case "porteforest_pendant3":
                if (stage == PFQuestStage.FoundPendant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant4",
                        OptionText = "Yeah I found the pendant."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;

            case "porteforest_pendant4":
                if (stage == PFQuestStage.FoundPendant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant5",
                        OptionText = "Yes, a few of us will. Where do we go?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;

            case "porteforest_pendant5":
                if (stage == PFQuestStage.FoundPendant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant6",
                        OptionText = "Is there anything else we should know?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;

            case "porteforest_pendant6":
                if (stage == PFQuestStage.FoundPendant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "Close",
                        OptionText = "Thank you Isabelle."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;

            case "rennie_initial":
                if (stage == PFQuestStage.KilledGiantMantis)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_rennie",
                        OptionText = "Trevor? Who?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Add(option);
                }

                break;

            case "porteforest_rennie":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_rennie1",
                    OptionText = "Oh... Trevor is dead..."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Add(option);
            }

                break;

            case "porteforest_rennie1":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_rennie2",
                    OptionText = "Sorry for your loss."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Add(option);
            }

                break;

            case "porteforest_rennie5":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_rennie3",
                    OptionText = "I will return it to him."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Add(option);
            }

                break;

            case "porteforest_rennie3":
            {
                Subject.Close(source);
                var tristar = ItemFactory.Create("tristarring");

                var rectangle = new Rectangle(
                    11,
                    7,
                    3,
                    4);

                var mapInstance = SimpleCache.Get<MapInstance>("pf_shop");

                Point point;

                do
                    point = rectangle.GetRandomPoint();

                while (!mapInstance.IsWalkable(point, source.Type));

                source.TraverseMap(mapInstance, point);
                source.Trackers.Enums.Set(PFQuestStage.CompletedPFQuest);
                ExperienceDistributionScript.GiveExp(source, 500000);
                source.TryGiveItem(tristar);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Saved the Daughter of Porte Forest",
                        "PorteForest",
                        MarkIcon.Heart,
                        MarkColor.White,
                        1,
                        GameTime.Now));
            }

                break;

            case "lureca_initial":
            {
                if (stage == PFQuestStage.CompletedPFQuest)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_lureca",
                        OptionText = "Rennie"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }

                break;

            case "porteforest_lureca":
            {
                if (stage == PFQuestStage.CompletedPFQuest)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_lureca1",
                        OptionText = "She gave me this ring to give to you."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "porteforest_keeptristar",
                        OptionText = "Umm... You'll have to take my word for it. Good bye!"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(1, option1);
                }
            }

                break;

            case "porteforest_lureca1":
            {
                if (!source.Inventory.HasCount("tristar ring", 1))
                {
                    source.SendOrangeBarMessage("You don't have the Tristar Ring.");
                    Subject.Reply(source, "What ring? Are you trying to fool me?");

                    return;
                }

                source.Inventory.Remove("tristar ring");
                ExperienceDistributionScript.GiveExp(source, 250000);
                source.Trackers.Enums.Set(PFQuestStage.TurnedInTristar);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Eased the suffering of Porte Forest",
                        "PorteForest1",
                        MarkIcon.Heart,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));
            }

                break;
        }
    }
}