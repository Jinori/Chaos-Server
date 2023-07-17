using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class RecipeLibraryScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private ItemDetails? ItemDetails;
    protected HashSet<string>? ItemTemplateKeys { get; init; }
    private Item? FauxItem => ItemDetails?.Item;

    public RecipeLibraryScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        var hasFlag1 = source.Trackers.Flags.TryGetFlag(out CookingRecipes _);
        var hasFlag2 = source.Trackers.Flags.TryGetFlag(out AlchemyRecipes _);
        var hasFlag3 = source.Trackers.Flags.TryGetFlag(out ArmorsmithingRecipes _);
        var hasFlag4 = source.Trackers.Flags.TryGetFlag(out WeaponSmithingRecipes _);
        var hasFlag5 = source.Trackers.Flags.TryGetFlag(out EnchantingRecipes _);
        var hasFlag6 = source.Trackers.Flags.TryGetFlag(out JewelcraftingRecipes _);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                if (hasFlag1 || hasFlag2 || hasFlag3 || hasFlag4 || hasFlag5 || hasFlag6)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "recipelibrary_initial",
                        OptionText = "Recipe Library"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "recipelibrary_initial":
            {
                void AddBookOption(string dialogKey, string optionText)
                {
                    var option = new DialogOption
                    {
                        DialogKey = dialogKey,
                        OptionText = optionText
                    };

                    if (!Subject.HasOption(optionText))
                        Subject.Options.Insert(0, option);
                }

                if (hasFlag1)
                    AddBookOption("cookbook", "Cook Book");

                if (hasFlag2)
                    AddBookOption("alchemybook", "Alchemy Book");

                if (hasFlag3)
                    AddBookOption("armorsmithingbook", "Armorsmithing Book");

                if (hasFlag4)
                    AddBookOption("weaponsmithingbook", "Weaponsmithing Book");

                if (hasFlag5)
                    AddBookOption("enchantingbook", "Enchanting Book");

                if (hasFlag6)
                    AddBookOption("jewelcraftingbook", "Jewelcrafting Book");

                break;
            }

            #region Cook Book
            case "cookbook":
            {
                if (source.Trackers.Flags.HasFlag(CookingRecipes.DinnerPlate))
                {
                    var item = ItemFactory.CreateFaux("dinnerplate");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.FruitBasket))
                {
                    var item = ItemFactory.CreateFaux("fruitbasket");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.LobsterDinner))
                {
                    var item = ItemFactory.CreateFaux("lobsterdinner");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.Pie))
                {
                    var item = ItemFactory.CreateFaux("pie");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.Salad))
                {
                    var item = ItemFactory.CreateFaux("salad");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.Sandwich))
                {
                    var item = ItemFactory.CreateFaux("sandwich");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.Soup))
                {
                    var item = ItemFactory.CreateFaux("soup");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.SteakMeal))
                {
                    var item = ItemFactory.CreateFaux("steakmeal");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                if (source.Trackers.Flags.HasFlag(CookingRecipes.SweetBuns))
                {
                    var item = ItemFactory.CreateFaux("sweetbuns");
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }

                break;
            }
            #endregion

            #region Alchemy Book
            case "alchemybook":
            {
                // Checking if the Alchemy recipe is available or not.
                if (source.Trackers.Flags.TryGetFlag(out AlchemyRecipes recipes))
                    // Iterating through the Alchemy recipe requirements.
                    foreach (var recipe in CraftingRequirements.AlchemyRequirements)
                        // Checking if the recipe is available or not.
                        if (recipes.HasFlag(recipe.Key))
                        {
                            // Creating a faux item for the recipe.
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            // Adding the recipe to the subject's dialog window.
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                break;
            }
            #endregion

            #region Armorsmithing Book
            case "armorsmithingbook":
            {
                if (source.Trackers.Flags.TryGetFlag(out CraftedArmors armorRecipes))
                    foreach (var recipe in CraftingRequirements.ArmorSmithingArmorRequirements)
                        if (armorRecipes.HasFlag(recipe.Key))
                        {
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                if (source.Trackers.Flags.TryGetFlag(out ArmorsmithingRecipes gearRecipes))
                    foreach (var recipe in CraftingRequirements.ArmorSmithingGearRequirements)
                        if (gearRecipes.HasFlag(recipe.Key))
                        {
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                break;
            }
            #endregion

            #region Weaponsmithing Book
            case "weaponsmithingbook":
            {
                if (source.Trackers.Flags.TryGetFlag(out WeaponSmithingRecipes recipes))
                    foreach (var recipe in CraftingRequirements.WeaponSmithingCraftRequirements)
                        if (recipes.HasFlag(recipe.Key))
                        {
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                break;
            }
            #endregion

            #region Enchanting Book
            case "enchantingbook":
            {
                // Checking if the Alchemy recipe is available or not.
                if (source.Trackers.Flags.TryGetFlag(out EnchantingRecipes recipes))
                    // Iterating through the Alchemy recipe requirements.
                    foreach (var recipe in CraftingRequirements.EnchantingRequirements)
                        // Checking if the recipe is available or not.
                        if (recipes.HasFlag(recipe.Key))
                        {
                            // Creating a faux item for the recipe.
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            // Adding the recipe to the subject's dialog window.
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                break;
            }
            #endregion

            #region Jewelcrafting Book
            case "jewelcraftingbook":
            {
                // Checking if the Alchemy recipe is available or not.
                if (source.Trackers.Flags.TryGetFlag(out JewelcraftingRecipes recipes))
                    // Iterating through the Alchemy recipe requirements.
                    foreach (var recipe in CraftingRequirements.JewelcraftingRequirements)
                        // Checking if the recipe is available or not.
                        if (recipes.HasFlag(recipe.Key))
                        {
                            // Creating a faux item for the recipe.
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            // Adding the recipe to the subject's dialog window.
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                break;
            }
            #endregion
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region Cook Book
            case "cookbook":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }

                    ItemDetails =
                        Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "dinnerplate":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 meat of any type, 10 fruit of any type, and 5 vegetables of any type. To cook this, go to the Mileth Kitchen and go to the stove.",
                            "cookbook");

                        return;
                    }
                    case "sweetbuns":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 15 fruit of any type, 5 vegetable of any type, and 2 flour.",
                            "cookbook");

                        return;
                    }
                    case "fruitbasket":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 15 fruit of any type, 10 fruit of any type, and 5 fruit of any type. Must be all different fruits.",
                            "cookbook");

                        return;
                    }
                    case "lobsterdinner":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires a lobster tail, 25 cherries, 5 vegetables, 5 tomatos, and 1 salt.",
                            "cookbook");

                        return;
                    }
                    case "pie":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 50 fruit of any type and 1 flour. The type of pie made depends on the fruit used.",
                            "cookbook");

                        return;
                    }
                    case "salad":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 meat of any type, 15 vegetables of any type, 15 vegetables of any type, and 1 cheese. The two vegetables must be different.",
                            "cookbook");

                        return;
                    }
                    case "sandwich":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 meat of any type, 5 vegetable, 5 tomato, 2 bread, and 1 cheese.",
                            "cookbook");

                        return;
                    }
                    case "soup":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 meat of any type, 5 vegetable of any type, 1 flour, and 1 salt.",
                            "cookbook");

                        return;
                    }
                    case "steakmeal":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 raw meat, 10 fruit of any type, 5 vegetable of any type, and 1 marinade.",
                            "cookbook");

                        return;
                    }
                }

                break;
            }
            #endregion

            #region Alchemy Book
            case "alchemybook":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }

                    ItemDetails =
                        Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "hemlochformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Mold and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    
                    case "smallhealthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Apple and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    
                    case "smallmanapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Acorn and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallrejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Baguette and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallhastebrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Sparkflower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallpowerbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallaccuracypotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kabine Blossom and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "juggernautbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Scorpion Sting, 1 Kobold Tail and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "astralbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Giant Bat Wing, 1 Kobold Tail and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "antidotepotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Frog Leg and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallfirestormtonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 3 Wolf's Fur and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallstuntonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Dochas Bloom, 3 Viper's Gland and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "healthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Apple, 1 Passion Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "manapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Acorn, 1 Lilypad and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "rejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Baguette, 1 Raineach and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "hastebrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Spark Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "powerbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Cactus Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "accuracypotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Kabine Blossom and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "revivepotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Blossom of Betrayal, 1 Spark Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongjuggernautbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Scorpion Sting, 2 Kobold Tail and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongastralbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Giant Bat Wing, 2 Kobold Tail and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "cleansingbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Water Lily and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "firestormtonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Cactus Flower, 5 Wolf's Fur and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "stuntonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Dochas Bloom, 1 Viper Gland and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "warmthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Bocan Bough and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "amnesiabrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Blossom of Betrayal and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "stronghealthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Apple, 1 Passion Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongmanapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Acorn, 1 Lily Pad and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongrejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Baguette, 1 Raineach and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "stronghastebrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Spark Flower, 1 Wisp Core and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongpowerbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Mummy Bandage, 2 Cactus Flower and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongaccuracypotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kabine Blossom, 1 Kraken Tentacle and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "statboostelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Viper's Venom, 1 Murauder's Spine, 1 Satyr's Hoof, 1 Polyp Sac and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "knowledgeelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Ancient Bone, 1 Lion Fish and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                }

                break;
            }
            #endregion

            #region Enchanting Book
            case "enchantingbook":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }

                    ItemDetails =
                        Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "ignatarenvy":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Ignatar.",
                            "enchantingbook");

                        return;
                    }
                    case "geolithgratitude":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Geolith.",
                            "enchantingbook");

                        return;
                    }
                    case "miraelisserenity":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Miraelis.",
                            "enchantingbook");

                        return;
                    }
                    case "theseleneelusion":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Theselene.",
                            "enchantingbook");

                        return;
                    }
                    case "aquaedonclarity":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Aquaedon.",
                            "enchantingbook");

                        return;
                    }
                    case "serendaelluck":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Serendael.",
                            "enchantingbook");

                        return;
                    }
                    case "skandaramight":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Skandara.",
                            "enchantingbook");

                        return;
                    }
                    case "zephyraspirit":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Zephyra.",
                            "enchantingbook");

                        return;
                    }
                    case "ignatargrief":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Ignatar.",
                            "enchantingbook");

                        return;
                    }
                    case "geolithpride":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Geolith.",
                            "enchantingbook");

                        return;
                    }
                    case "miraelisblessing":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Miraelis.",
                            "enchantingbook");

                        return;
                    }
                    case "theseleneshadow":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Theselene.",
                            "enchantingbook");

                        return;
                    }
                    case "aquaedoncalming":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Aquaedon.",
                            "enchantingbook");

                        return;
                    }
                    case "serendaelmagic":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Serendael.",
                            "enchantingbook");

                        return;
                    }
                    case "skandaratriumph":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Skandara.",
                            "enchantingbook");

                        return;
                    }
                    case "zephyramist":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Zephyra.",
                            "enchantingbook");

                        return;
                    }
                    case "ignatarregret":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Ignatar.",
                            "enchantingbook");

                        return;
                    }
                    case "geolithconstitution":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Geolith.",
                            "enchantingbook");

                        return;
                    }
                    case "miraelisintellect":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Miraelis.",
                            "enchantingbook");

                        return;
                    }
                    case "theselenedexterity":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Theselene.",
                            "enchantingbook");

                        return;
                    }
                    case "aquaedonwisdom":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Aquaedon.",
                            "enchantingbook");

                        return;
                    }
                    case "serendaelchance":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Serendael.",
                            "enchantingbook");

                        return;
                    }
                    case "skandarastrength":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Skandara.",
                            "enchantingbook");

                        return;
                    }
                    case "zephyrawind":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Zephyra.",
                            "enchantingbook");

                        return;
                    }
                    case "ignatarjealousy":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Ignatar.",
                            "enchantingbook");

                        return;
                    }
                    case "geolithobsession":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Geolith.",
                            "enchantingbook");

                        return;
                    }
                    case "miraelisharmony":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Miraelis.",
                            "enchantingbook");

                        return;
                    }
                    case "theselenebalance":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Theselene.",
                            "enchantingbook");

                        return;
                    }
                    case "aquaedonwill":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Aquaedon.",
                            "enchantingbook");

                        return;
                    }
                    case "serendaelroll":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Serendael.",
                            "enchantingbook");

                        return;
                    }
                    case "skandaradrive":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Skandara.",
                            "enchantingbook");

                        return;
                    }
                    case "zephyravortex":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Zephyra.",
                            "enchantingbook");

                        return;
                    }
                    case "ignatardestruction":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Ignatar.",
                            "enchantingbook");

                        return;
                    }
                    case "geolithfortitude":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Geolith.",
                            "enchantingbook");

                        return;
                    }
                    case "miraelisnurturing":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Miraelis.",
                            "enchantingbook");

                        return;
                    }
                    case "theselenerisk":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Theselene.",
                            "enchantingbook");

                        return;
                    }
                    case "aquaedonresolve":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Aquaedon.",
                            "enchantingbook");

                        return;
                    }
                    case "serendaeladdiction":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Serendael.",
                            "enchantingbook");

                        return;
                    }
                    case "skandarapierce":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Skandara.",
                            "enchantingbook");

                        return;
                    }
                    case "zephyragust":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Zephyra.",
                            "enchantingbook");

                        return;
                    }
                }

                break;
            }
            #endregion

            #region Jewelcrafting Book
            case "jewelcraftingbook":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }

                    ItemDetails =
                        Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "bronzeberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished bronze and 1 flawed beryl to craft.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzerubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzesapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzeheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzeemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished mythril and 1 finished sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished mythril and 1 finished heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished hybrasyl and 3 finished beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished hybrasyl and 3 finished ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron, and 1 uncut beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished mythril and 1 finished sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished mythril and 1 finished heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished hybrasyl and 3 finished beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished hybrasyl and 3 finished ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicfirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicwindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze and 1 flawed emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "apprenticeearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "apprenticefirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "apprenticeseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "apprenticewindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished iron and 1 uncut emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedfirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished mythril and 1 finished sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedwindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished mythril and 1 finished emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished hybrasyl and 3 finished beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starfirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished hybrasyl and 3 finished ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starwindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 polished hybrasyl and 3 finished emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                }

                break;
            }
            #endregion

            #region Weaponsmithing Book
            case "weaponsmithingbook":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }

                    ItemDetails =
                        Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "eppe":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze.",
                            "weaponsmithingbook");

                        return;
                }

                break;
            }
            #endregion

            #region Armorsmithing Book
            case "armorsmithingbook":

            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }

                    ItemDetails =
                        Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.ReplyToUnknownInput(source);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "gardcorp":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 polished bronze.",
                            "armorsmithingbook");

                        return;
                }

                break;
            }
            #endregion
        }
    }
}