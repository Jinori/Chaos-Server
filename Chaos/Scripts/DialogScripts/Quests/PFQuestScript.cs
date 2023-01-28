using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.DialogScripts.Quests;

public class PFQuestScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public PFQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {

        var hasStage = source.Enums.TryGetValue(out PFQuestStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "porteforest_initial":
            {
                if ((!hasStage) || (stage == PFQuestStage.None))
                {

                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest1",
                        OptionText = "What do you know?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if (stage == PFQuestStage.TurnedInRoots)
                {
                    Subject.Text = "I already told you what I know.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_repeat",
                        OptionText = "What did you say again?"
                    };
                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }
                
                if (stage == PFQuestStage.CompletedPFQuest)
                {
                    Subject.Text = "Well done Aisling, I'm glad that beast won't bother us again. I can go farm my own trent roots again. Thank you.";
                }
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

                if (!Subject.HasOption(option))
                    Subject.Options.Insert(0, option);
                if (!Subject.HasOption(option))
                    Subject.Options.Insert(1, option1);
            }

                break;

            case "porteforest_yes":
                if (!hasStage || (stage == PFQuestStage.None))
                {
                    source.Enums.Set(PFQuestStage.StartedPFQuest);
                }
                break;
            
            case "porteforest_rootturnin":
                if (stage == PFQuestStage.StartedPFQuest)
                {
                    if (!source.Inventory.HasCount("trent root", 4))
                    {
                        Subject.Text = "Can you bring some more?";

                        var option = new DialogOption
                        {
                            DialogKey = "Close",
                            OptionText = "Be right back."
                        };
                        
                        if (!Subject.HasOption(option))
                            Subject.Options.Insert(0, option);
                        
                        source.SendOrangeBarMessage("Torbjorn isn't impressed. He wants four trent roots.");

                        return;
                    }

                    source.Inventory.RemoveQuantity("trent root", 4);
                    source.Enums.Set(PFQuestStage.TurnedInRoots);
                    ExperienceDistributionScript.GiveExp(source, 100000);
                    Subject.Text = "Thank you Aisling! Now I can make some more weapons.";

                    var option1 = new DialogOption
                    {
                        DialogKey = "porteforest_rootturnin1",
                        OptionText = "Where did you last see the pendant?"
                    };
                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);
                    
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
                        if (!Subject.HasOption(option))
                            Subject.Options.Add(option);
                }

                if (stage == PFQuestStage.WolfManes)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_wolfmanes",
                        OptionText = "I have the Silver Wolf Manes."
                    };
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }
                
                if (stage == PFQuestStage.CompletedPFQuest)
                {
                    Subject.Text = "It is so great to hear about your battle with the Giant Mantis. I can't believe you took it down. Praise you Aisling.";
                }

                break;
            
            case "porteforest_quest3":
            {
                var option = new DialogOption
                {
                    DialogKey = "porteforest_quest4",
                    OptionText = "You dropped the pendant and you may have more information."
                };

                if (!Subject.HasOption(option))
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

                if (!Subject.HasOption(option))
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

                if (!Subject.HasOption(option))
                    Subject.Options.Insert(0, option);
                if (!Subject.HasOption(option1))
                    Subject.Options.Insert(1, option1);
                if (!Subject.HasOption(option2))
                    Subject.Options.Insert(2, option2);
            }

                break;
            
            case "porteforest_yes2":
                if (stage == PFQuestStage.TurnedInRoots)
                {
                    source.Enums.Set(PFQuestStage.WolfManes);
                    source.SendOrangeBarMessage("Return to Bertil with five Silver Wolf Mane Hairs.");
                    return;
                }

                break;
            
            case "porteforest_wolfmanes":
                if (stage == PFQuestStage.WolfManes)
                {
                    if (!source.Inventory.HasCount("Silver Wolf Mane Hair", 5))
                    {
                        Subject.Text = "You don't have enough Aisling, I need atleast five Silver Wolf Mane Hair to make the Silver Wolf Leather.";

                        return;
                    }

                    Subject.Text = "Great work Aisling! Here is your Silver Wolf Leather, that should help you fight that beast off. Go talk to Isabelle again, she may know where to go with it.";

                    source.Inventory.RemoveQuantity("Silver Wolf Mane Hair", 5);

                    var leather = ItemFactory.Create("silverwolfleather");
                    source.Enums.Set(PFQuestStage.WolfManesTurnedIn);
                    ExperienceDistributionScript.GiveExp(source, 150000);
                    source.TryGiveItem(leather);

                }

                break;
            
            case "isabelle_initial":
                if ((!hasStage) || (stage == PFQuestStage.None))
                {
                    if (source.UserStatSheet.Level is <= 10 or >= 42)
                    {
                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_initial3",
                        OptionText = "Porte Forest"
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                }

                if (stage == PFQuestStage.TurnedInRoots)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant",
                        OptionText = "Turuc Pendant"
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);
                }

                if (stage == PFQuestStage.FoundPendant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant3",
                        OptionText = "Turuc Pendant"
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }

                if (stage == PFQuestStage.CompletedPFQuest)
                {
                    Subject.Text = "Thank you so much Aisling, I can enjoy the peak again! And all the flowers and bunnies!";
                }

                break;
            
            case "porteforest_initial3":
                if ((!hasStage) || (stage == PFQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest6",
                        OptionText = "What did you see?"
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                }

                break;
            
            case "porteforest_quest6":
                if ((!hasStage) || (stage == PFQuestStage.None))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_quest7",
                        OptionText = "Did they get away safely?"
                    };
                    
                    if (!Subject.HasOption(option))
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
                    if (!Subject.HasOption(option))
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
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }

                break;
            
            case "porteforest_pendant4":
                if (stage == PFQuestStage.FoundPendant)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "porteforest_pendant5",
                        OptionText = "Yes, a few of us will. Where do we go?."
                    };
                    if (!Subject.HasOption(option))
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
                    if (!Subject.HasOption(option))
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
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                }

                break;



        }
        
        
    }
}