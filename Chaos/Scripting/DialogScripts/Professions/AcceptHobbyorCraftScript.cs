using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Professions;

public class AcceptHobbyorCraftScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    public AcceptHobbyorCraftScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Flags.TryGetFlag(out Hobbies hobby);
        var hasCraft = source.Trackers.Enums.TryGetValue(out Crafts craft);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "kamel_initial":
            {
                if (!source.Trackers.Flags.HasFlag(Hobbies.Fishing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "fishing_starthobby",
                        OptionText = "I love to fish."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
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
                        OptionText = "I am ready to be a Chef."
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

                return;
            }
            case "thorin_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "weaponsmithing_startcraft",
                        OptionText = "I want to be a Weaponsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }
                
                if (hasCraft && (craft == Crafts.Weaponsmithing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be a Weaponsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "weaponsmithing_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Weaponsmithing);
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
                }
                if (hasCraft && (craft == Crafts.Armorsmithing))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be an Armorsmith."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "armorsmith_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Armorsmithing);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherGauntlet);
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
                        OptionText = "I'd like to be an Alchemist."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                if (hasCraft && (craft == Crafts.Alchemy))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be an Alchemist."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "alchemy_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Alchemy);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHealthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallManaPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHasteBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallPowerBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallAccuracyPotion);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Alchemist",
                        "alch",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));
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
                        OptionText = "Could you teach me Enchanting?"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }
                if (hasCraft && (craft == Crafts.Enchanting))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be an Enchanter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "enchanting_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Enchanting);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithGratitude);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarEnvy);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisSerenity);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneElusion);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonClarity);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Enchanter",
                        "ench",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                source.SendOrangeBarMessage("You are now an Enchanter!");

                return;
            }
            case "celestia_initial":
            {
                if (!hasCraft)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "jewelcrafting_startcraft",
                        OptionText = "I'd like to be a Jewelcrafter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (hasCraft && (craft == Crafts.Jewelcrafting))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "craft_remove1",
                        OptionText = "I don't want to be a Jewelcrafter."
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "craft_remove3":
            {
                source.Trackers.Enums.Remove<Crafts>();
                source.Legend.Remove("alch", out _);
                source.Legend.Remove("jwlcrftng", out _);
                source.Legend.Remove("wpnsmth", out _);
                source.Legend.Remove("armsmth", out _);
                source.Legend.Remove("ench", out _);
                source.SendOrangeBarMessage("You are no longer a Crafter.");

                return;
            }
            
            case "jewelcrafting_acceptcraft":
            {
                source.Trackers.Enums.Set(Crafts.Jewelcrafting);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.SmallRubyRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BerylRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.EarthNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.SeaNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.FireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.WindNecklace);
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Beginner Jewelcrafter",
                        "jwlcrftng",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                source.SendOrangeBarMessage("You are now a Jewelcrafter!");
                
                

                return;
            }
        }
    }
}