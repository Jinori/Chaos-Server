using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest;

public class PentaSymbolScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    
    public PentaSymbolScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "pentasymbol":
            {
                switch (source.UserStatSheet.BaseClass)
                {
                    case BaseClass.Warrior:
                    {
                        Subject.AddOption("Stare at the tree...", "pentawarriorsymbol1");
                    }

                        break;

                    case BaseClass.Rogue:
                    {
                        Subject.AddOption("Stare at the bones...", "pentaroguesymbol1");
                    }

                        break;

                    case BaseClass.Wizard:
                    {
                        Subject.AddOption("Stare at the potions...", "pentawizardsymbol1");
                    }

                        break;

                    case BaseClass.Priest:
                    {
                        Subject.AddOption("Gaze into the fire...", "pentapriestsymbol1");
                    }

                        break;

                    case BaseClass.Monk:
                    {
                        Subject.AddOption("Stare at the pipe...", "pentamonksymbol1");
                    }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

                break;
            
            case "pentasymbolend":
            {
                if (source.UserStatSheet.BaseClass == BaseClass.Monk)
                {
                    var pieceofritual = ItemFactory.Create("pentagrampiece2");
                    source.GiveItemOrSendToBank(pieceofritual);
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Priest)
                {
                    var pieceofritual = ItemFactory.Create("pentagrampiece3");
                    source.GiveItemOrSendToBank(pieceofritual);
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Rogue)
                {
                    var pieceofritual = ItemFactory.Create("pentagrampiece4");
                    source.GiveItemOrSendToBank(pieceofritual);
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Wizard)
                {
                    var pieceofritual = ItemFactory.Create("pentagrampiece1");
                    source.GiveItemOrSendToBank(pieceofritual);
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Warrior)
                {
                    var pieceofritual = ItemFactory.Create("pentagrampiece5");
                    source.GiveItemOrSendToBank(pieceofritual);
                }

                Subject.Close(source);
                source.Trackers.Enums.Set(PentagramQuestStage.FoundPentagramPiece);
                source.SendOrangeBarMessage("You have found your pentagram piece, but what is missing?");
            }

                break;

            case "celestia_initial":
            {
                if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage)
                    && (stage == PentagramQuestStage.FoundPentagramPiece))
                {
                    Subject.Options.RemoveAll(optionToRemove => optionToRemove.DialogKey == "generic_buyShop_initial");
                    Subject.Options.RemoveAll(optionToRemove => optionToRemove.DialogKey == "generic_sellShop_initial");
                    
                    var option = new DialogOption
                    {
                        DialogKey = "empowerritualpiece1",
                        OptionText = "I found this."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                if ((stage == PentagramQuestStage.EmpoweringPentagramPiece))
                {
                    Subject.Options.RemoveAll(optionToRemove => optionToRemove.DialogKey == "generic_buyShop_initial");
                    Subject.Options.RemoveAll(optionToRemove => optionToRemove.DialogKey == "generic_sellShop_initial");
                    
                    var option = new DialogOption
                    {
                        DialogKey = "empowerritualpiece4",
                        OptionText = "I have the Pristine Gem."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }

                break;

            case "empowerritualpiece3":
            {
                switch (source.UserStatSheet.BaseClass)
                {
                    case BaseClass.Monk:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "empowerritualpiecemonk",
                            OptionText = "What gem should I empower this with?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                   

                        return;
                    }
                    case BaseClass.Priest:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "empowerritualpiecepriest",
                            OptionText = "What gem should I empower this with?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                   

                        return;
                    }
                    case BaseClass.Rogue:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "empowerritualpiecerogue",
                            OptionText = "What gem should I empower this with?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                   

                        return;
                    }
                    case BaseClass.Warrior:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "empowerritualpiecewarrior",
                            OptionText = "What gem should I empower this with?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                   

                        return;
                    }
                    case BaseClass.Wizard:
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "empowerritualpiecewizard",
                            OptionText = "What gem should I empower this with?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);

                        break;
                    }
                }
            }
                break;

            case "empowerritualpiecepriest":
            case "empowerritualpiecemonk":
            case "empowerritualpiecewarrior":
            case "empowerritualpiecewizard":
            case "empowerritualpiecerogue":
                source.Trackers.Enums.Set(PentagramQuestStage.EmpoweringPentagramPiece);

                break;

            case "empowerritualpiece4":
            {
                if (source.UserStatSheet.BaseClass == BaseClass.Monk)
                {
                    var empoweredPiece = ItemFactory.Create("BerylPentagramPiece");

                    if (source.Inventory.HasCount("Pristine Beryl", 1) && source.Inventory.HasCount("Pentagram Piece", 1))
                    {
                        source.Inventory.RemoveQuantity("Pristine Beryl", 1);
                        source.Inventory.RemoveQuantity("Pentagram Piece", 1);
                        source.GiveItemOrSendToBank(empoweredPiece);

                        Subject.Reply(
                            source,
                            "All done, that was nerve wracking... If you and your group head to the forge, talk to Thorin, he might be able to put the pieces together.");
                    } else
                    {
                        if (!source.Inventory.HasCount("Pristine Beryl", 1) && (!source.Inventory.HasCount("Pentagram Piece", 1)))
                            Subject.Reply(source, "Where's the Pristine Beryl and Pentagram Piece that I need to do this procedure?");
                        
                        if (!source.Inventory.HasCount("Pristine Beryl", 1))
                            Subject.Reply(source, "Where's the Pristine Beryl I need to do this procedure?");

                        if (!source.Inventory.HasCount("Pentagram Piece", 1))
                            Subject.Reply(source, "Where's the Pentagram Piece I need to do this procedure?");

                        return;
                    }
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Priest)
                {
                    var empoweredPiece = ItemFactory.Create("SapphirePentagramPiece");

                    if (source.Inventory.HasCount("Pristine Sapphire", 1) && source.Inventory.HasCount("Pentagram Piece", 1))
                    {
                        source.Inventory.RemoveQuantity("Pristine Sapphire", 1);
                        source.Inventory.RemoveQuantity("Pentagram Piece", 1);
                        source.GiveItemOrSendToBank(empoweredPiece);

                        Subject.Reply(
                            source,
                            "All done, that was nerve wracking... If you and your group head to the forge, talk to Thorin, he might be able to put the pieces together.");
                    } else
                    {
                        if (!source.Inventory.HasCount("Pristine Sapphire", 1) && (!source.Inventory.HasCount("Pentagram Piece", 1)))
                            Subject.Reply(source, "Where's the Pristine Sapphire and Pentagram Piece that I need to do this procedure?");
                        
                        if (!source.Inventory.HasCount("Pristine Sapphire", 1))
                            Subject.Reply(source, "Where's the Pristine Sapphire I need to do this procedure?");

                        if (!source.Inventory.HasCount("Pentagram Piece", 1))
                            Subject.Reply(source, "Where's the Pentagram Piece I need to do this procedure?");
                        return;
                    }
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Rogue)
                {
                    var empoweredPiece = ItemFactory.Create("EmeraldPentagramPiece");

                    if (source.Inventory.HasCount("Pristine Emerald", 1) && source.Inventory.HasCount("Pentagram Piece", 1))
                    {
                        source.Inventory.RemoveQuantity("Pristine Emerald", 1);
                        source.Inventory.RemoveQuantity("Pentagram Piece", 1);
                        source.GiveItemOrSendToBank(empoweredPiece);

                        Subject.Reply(
                            source,
                            "All done, that was nerve wracking... If you and your group head to the forge, talk to Thorin, he might be able to put the pieces together.");
                    } else
                    {
                        if (!source.Inventory.HasCount("Pristine Emerald", 1) && (!source.Inventory.HasCount("Pentagram Piece", 1)))
                            Subject.Reply(source, "Where's the Pristine Emerald and Pentagram Piece that I need to do this procedure?");
                        
                        if (!source.Inventory.HasCount("Pristine Emerald", 1))
                            Subject.Reply(source, "Where's the Pristine Emerald I need to do this procedure?");

                        if (!source.Inventory.HasCount("Pentagram Piece", 1))
                            Subject.Reply(source, "Where's the Pentagram Piece I need to do this procedure?");
                        return;
                    }
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Warrior)
                {
                    var empoweredPiece = ItemFactory.Create("RubyPentagramPiece");

                    if (source.Inventory.HasCount("Pristine Ruby", 1) && source.Inventory.HasCount("Pentagram Piece", 1))
                    {
                        source.Inventory.RemoveQuantity("Pristine Ruby", 1);
                        source.Inventory.RemoveQuantity("Pentagram Piece", 1);
                        source.GiveItemOrSendToBank(empoweredPiece);

                        Subject.Reply(
                            source,
                            "All done, that was nerve wracking... If you and your group head to the forge, talk to Thorin, he might be able to put the pieces together.");
                    } else
                    {
                        if (!source.Inventory.HasCount("Pristine Ruby", 1) && (!source.Inventory.HasCount("Pentagram Piece", 1)))
                            Subject.Reply(source, "Where's the Pristine Ruby and Pentagram Piece that I need to do this procedure?");
                        
                        if (!source.Inventory.HasCount("Pristine Ruby", 1))
                            Subject.Reply(source, "Where's the Pristine Ruby I need to do this procedure?");

                        if (!source.Inventory.HasCount("Pentagram Piece", 1))
                            Subject.Reply(source, "Where's the Pentagram Piece I need to do this procedure?");
                        
                        return;
                    }
                }
                if (source.UserStatSheet.BaseClass == BaseClass.Wizard)
                {
                    var empoweredPiece = ItemFactory.Create("HeartstonePentagramPiece");

                    if (source.Inventory.HasCount("Pristine Heartstone", 1) && source.Inventory.HasCount("Pentagram Piece", 1))
                    {
                        source.Inventory.RemoveQuantity("Pristine Heartstone", 1);
                        source.Inventory.RemoveQuantity("Pentagram Piece", 1);
                        source.GiveItemOrSendToBank(empoweredPiece);

                        Subject.Reply(
                            source,
                            "All done, that was nerve wracking... If you and your group head to the forge, talk to Thorin, he might be able to put the pieces together.");
                    } else
                    {
                        if (!source.Inventory.HasCount("Pristine Heartstone", 1) && (!source.Inventory.HasCount("Pentagram Piece", 1)))
                            Subject.Reply(source, "Where's the Pristine Heartstone and Pentagram Piece that I need to do this procedure?");
                        
                        if (!source.Inventory.HasCount("Pristine Heartstone", 1))
                            Subject.Reply(source, "Where's the Pristine Heartstone I need to do this procedure?");

                        if (!source.Inventory.HasCount("Pentagram Piece", 1))
                            Subject.Reply(source, "Where's the Pentagram Piece I need to do this procedure?");

                        return;
                    }
                }

                source.Trackers.Enums.Set(PentagramQuestStage.EmpoweredPentagramPiece);
                source.SendOrangeBarMessage("Your pentagram piece has been empowered.");
            }

                break;
                
            case "thorin_initial":
            {
                if (source.UserStatSheet.BaseClass == BaseClass.Wizard)
                    if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage)
                        && (stage == PentagramQuestStage.EmpoweredPentagramPiece))
                    {
                        Subject.Options.RemoveAll(optionToRemove => optionToRemove.DialogKey == "generic_buyShop_initial");
                        Subject.Options.RemoveAll(optionToRemove => optionToRemove.DialogKey == "generic_sellShop_initial");
                        
                        var option = new DialogOption
                        {
                            DialogKey = "craftpentagram1",
                            OptionText = "Will you help us craft the Pentagram?"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    }
            }

                break;

            case "craftpentagram4":
            {
                switch (source.Group)
                {
                    case { Count: > 5 }:
                        Subject.Reply(source, "There are too many of you, I can't help you.");

                        return;
                    case { Count: < 5 }:
                        Subject.Reply(source, "You're missing someone, they must be here with you.");

                        return;
                }

                foreach (var groupmember in source.Group!)
                {
                    if (groupmember.Trackers.Enums.TryGetValue(out PentagramQuestStage stage)
                        && (stage != PentagramQuestStage.EmpoweredPentagramPiece))
                    {
                        Subject.Reply(source, "One of your members are missing the empowered ritual piece.");

                        return;
                    }

                    if (groupmember.UserStatSheet.BaseClass == BaseClass.Monk)
                    {
                        if (!groupmember.Inventory.HasCount("Beryl Pentagram Piece", 1))
                        {
                            source.SendOrangeBarMessage("You need a Beryl Pentagram Piece to craft the pentagram.");

                            return;
                        }
                    }

                    if (groupmember.UserStatSheet.BaseClass == BaseClass.Wizard)
                    {
                        if (!groupmember.Inventory.HasCount("Heartstone Pentagram Piece", 1))
                        {
                            source.SendOrangeBarMessage(
                                "You need a Heartstone Pentagram Piece to craft the pentagram.");

                            return;
                        }
                    }

                    if (groupmember.UserStatSheet.BaseClass == BaseClass.Rogue)
                    {
                        if (!groupmember.Inventory.HasCount("Emerald Pentagram Piece", 1))
                        {
                            source.SendOrangeBarMessage("You need a Emerald Pentagram Piece to craft the pentagram.");

                            return;
                        }
                    }

                    if (groupmember.UserStatSheet.BaseClass == BaseClass.Priest)
                    {
                        if (!groupmember.Inventory.HasCount("Sapphire Pentagram Piece", 1))
                        {
                            source.SendOrangeBarMessage("You need a Sapphire Pentagram Piece to craft the pentagram.");

                            return;
                        }
                    }

                    if (groupmember.UserStatSheet.BaseClass == BaseClass.Warrior)
                    {
                        if (!groupmember.Inventory.HasCount("Ruby Pentagram Piece", 1))
                        {
                            source.SendOrangeBarMessage("You need a Ruby Pentagram Piece to craft the pentagram.");

                            return;
                        }
                    }

                    groupmember.Inventory.RemoveQuantity("Beryl Pentagram Piece", 1);
                    groupmember.Inventory.RemoveQuantity("Heartstone Pentagram Piece", 1);
                    groupmember.Inventory.RemoveQuantity("Emerald Pentagram Piece", 1);
                    groupmember.Inventory.RemoveQuantity("Sapphire Pentagram Piece", 1);
                    groupmember.Inventory.RemoveQuantity("Ruby Pentagram Piece", 1);
                    
                    var pentagram = ItemFactory.Create("Pentagram");

                    if (groupmember.UserStatSheet.BaseClass == BaseClass.Wizard)
                    {
                        groupmember.GiveItemOrSendToBank(pentagram);
                        groupmember.SendOrangeBarMessage("Thorin hands you the crafted Pentagram.");
                    }

                    if (groupmember.UserStatSheet.BaseClass != BaseClass.Wizard)
                    {
                        groupmember.SendOrangeBarMessage("You watch Thorin hand the crafted pentagram to your wizard.");
                    }

                    groupmember.Trackers.Enums.Set(PentagramQuestStage.CreatedPentagram);
                    Subject.Reply(groupmember, "Thorin hands the Pentagram to the wizard, then addresses the group.\nGood luck to you all, you'll need it.");
                }
            }

                break;
        }
    }
}