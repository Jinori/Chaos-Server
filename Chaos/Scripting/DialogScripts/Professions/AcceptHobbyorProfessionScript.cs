using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Professions;

public class AcceptHobbyorProfessionScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    public AcceptHobbyorProfessionScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasHobby = source.Trackers.Flags.TryGetFlag(out Hobbies hobby);
        var hasCraft = source.Trackers.Enums.TryGetValue(out Craft craft);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "kamel_initial":
            {
                if (!source.Trackers.Flags.HasFlag(Hobbies.Fishing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "fishing_starthobby",
                        OptionText = "I want to be a fisherman."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }
            }

                break;
            case "fishing_accepthobby":
            {
                source.Trackers.Flags.AddFlag(Hobbies.Fishing);
                source.SendOrangeBarMessage("You are now a fisherman! Grab a pole and some bait from Kamel.");

                return;
            }
            case "gendusa_initial":
            {
                if (!source.Trackers.Flags.HasFlag(Hobbies.Cooking))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "cooking_starthobby",
                        OptionText = "I want to be a Chef."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
            }

                break;
            case "cooking_accepthobby":
            {
                source.Trackers.Flags.AddFlag(Hobbies.Cooking);
                source.Trackers.Flags.AddFlag(CookingRecipes.DinnerPlate);
                source.Trackers.Flags.AddFlag(CookingRecipes.FruitBasket);
                source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Beginner Chef",
                    "cooking",
                    MarkIcon.Yay,
                    MarkColor.White,
                    1,
                    GameTime.Now));
                source.SendOrangeBarMessage("You are now a Chef!");

                return;
            }
            case "thorin_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "weaponsmithing_startcraft",
                        OptionText = "I want to be an Weaponsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "weaponsmith_acceptcraft":
            {
                source.Trackers.Enums.Set(Craft.Weaponsmithing);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eppe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Saber);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DullClaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SnowDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CenterDagger);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Weaponsmith",
                        "wpnsmth",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                source.SendOrangeBarMessage("You are now a Beginner Weaponsmith!");

                return;
            }

            case "armorsmithnpc_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "armorsmithing_startcraft",
                        OptionText = "I want to be an Armorsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;
            }

            case "armorsmith_acceptcraft":
            {
                source.Trackers.Enums.Set(Craft.Armorsmithing);
                var book = ItemFactory.Create("recipe_basicarmors");
                source.TryGiveItem(book);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Armorsmith",
                        "armsmth",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                source.SendOrangeBarMessage("You are now an Armorsmith!");

                return;
            }
            case "mathis_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "alchemy_startcraft",
                        OptionText = "I want to be an Alchemist."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;
            }

            case "alchemy_acceptcraft":
            {
                source.Trackers.Enums.Set(Craft.Alchemy);
                source.SendOrangeBarMessage("You are now an Alchemist!");

                return;
            }
            case "elara_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "enchanting_startcraft",
                        OptionText = "I want to be an Enchanter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;
            }

            case "enchanting_acceptcraft":
            {
                source.Trackers.Enums.Set(Craft.Enchanting);
                source.SendOrangeBarMessage("You are now an Enchanter!");

                return;
            }
            case "jewelcraftingnpc_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "jewelcrafting_startcraft",
                        OptionText = "I want to be a Jewelcrafter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                break;
            }

            case "jewelcrafting_acceptcraft":
            {
                source.Trackers.Enums.Set(Craft.Jewelcrafting);
                source.SendOrangeBarMessage("You are now a Jewelcrafter!");

                return;
            }
        }
    }
}