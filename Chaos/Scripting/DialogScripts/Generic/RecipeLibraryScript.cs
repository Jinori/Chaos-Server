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
                            var templateKey = recipe.Value.TemplateKey;

                            var item = ItemFactory.CreateFaux(
                                templateKey.StartsWith("f", StringComparison.Ordinal) ? templateKey[1..] : templateKey);

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
                    foreach (var recipe in CraftingRequirements.WeaponSmithingUpgradeRequirements)
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
                {
                    foreach (var recipe in CraftingRequirements.JewelcraftingRequirements)
                        // Checking if the recipe is available or not.
                        if (recipes.HasFlag(recipe.Key))
                        {
                            // Creating a faux item for the recipe.
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                            // Adding the recipe to the subject's dialog window.
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }
                }
                
                // Checking if the Alchemy recipe is available or not.
                if (source.Trackers.Flags.TryGetFlag(out JewelcraftingRecipes2 recipes2))
                {
                    foreach (var recipe2 in CraftingRequirements.JewelcraftingRequirements2)
                        // Checking if the recipe is available or not.
                        if (recipes2.HasFlag(recipe2.Key))
                        {
                            // Creating a faux item for the recipe.
                            var item = ItemFactory.CreateFaux(recipe2.Value.TemplateKey);
                            // Adding the recipe to the subject's dialog window.
                            Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }
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
                            } requires 1 meat of any type, 10 fruit of any type, and 5 vegetables of any type.",
                            "cookbook");

                        return;
                    }
                    case "sweetbuns":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 15 fruit of any type, 5 vegetable of any type, and 1 flour.",
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
                            } requires 2 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;

                    case "smallhealthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;

                    case "smallmanapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallrejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Lesser Monster Extract and 1 Empty Bottle.",
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
                            } requires 2 Lesser Monster Extract, 1 Kobold Tail and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "astralbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Lesser Monster Extract, 1 Kobold Tail and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "antidotepotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallfirestormtonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 2 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "smallstuntonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Dochas Bloom, 2 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "healthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "manapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "rejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "hastebrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Spark Flower, 1 Basic Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "powerbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 1 Basic Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "accuracypotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 2 Kabine Blossom, 1 Basic Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "revivepotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Blossom of Betrayal, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongjuggernautbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kobold Tail, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongastralbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kobold Tail, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "cleansingbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Water Lily, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "firestormtonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "stuntonicformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Dochas Bloom, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "warmthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Bocan Bough, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "amnesiabrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Blossom of Betrayal, 1 Greater Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "stronghealthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongmanapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongrejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "stronghastebrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Spark Flower, 1 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongpowerbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 1 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongaccuracypotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kabine Blossom, 1 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "statboostelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Marauder's Spine, 1 Satyr's Hoof, 1 Polyp Sac, 1 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "knowledgeelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Ancient Bone, 1 Lion Fish and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    
                    case "potenthealthpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 8 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potentmanapotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 8 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potentrejuvenationpotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 8 Lesser Monster Extract and 1 Empty Bottle.",
                            "alchemybook");
                        return;
                    case "potenthastebrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Spark Flower, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potentpowerbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potentaccuracybrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kabine Blossom, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");
                        return;
                        
                    case "potentjuggernautbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Blossom of Betrayal, 3 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");
                        return;
                        
                    case "potentastralbrewformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Passion Flower, 3 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "invisibilitypotionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Kobold Tail, 3 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "poisonimmunityelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Raineach, 1 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potionofstrengthformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Cactus Flower, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potionofintellectformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Blossom of Betrayal, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potionofwisdomformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Lily Pad, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potionofconstitutionformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Bocan Bough, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "potionofdexterityformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Dochas Bloom, 2 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongstatboostelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Losgann Tail, 1 Red Shocker Piece, 1 Ice Skeleton Skull, 3 Superior Monster Extract and 1 Empty Bottle.",
                            "alchemybook");

                        return;
                    case "strongknowledgeelixirformula":
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Black Shocker Piece, 1 Ice Elemental Flame, 1 Rock Fish, 3 Superior Monster Extract and 1 Empty Bottle.",
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
                    case "aquaedoncalming":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Aquaedon.",
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
                    case "aquaedonresolve":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Aquaedon.",
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
                    case "aquaedonwisdom":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Aquaedon.",
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
                    case "ignatarenvy":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Ignatar.",
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
                    case "ignatarjealousy":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Ignatar.",
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
                    case "geolithfortitude":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Geolith.",
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
                    case "geolithobsession":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Geolith.",
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
                    case "miraelisharmony":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Miraelis.",
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
                    case "miraelisnurturing":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Miraelis.",
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
                    case "serendaeladdiction":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 10 Essence of Serendael.",
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
                    case "serendaelluck":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Serendael.",
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
                    case "skandaramight":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Skandara.",
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
                    case "skandarastrength":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Skandara.",
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
                    case "theselenebalance":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 7 Essence of Theselene.",
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
                    case "theseleneelusion":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Essence of Theselene.",
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
                    case "theseleneshadow":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Theselene.",
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
                    case "zephyramist":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 3 Essence of Zephyra.",
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
                    case "zephyrawind":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 5 Essence of Zephyra.",
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
                    case "smallrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Raw Bronze and 1 Raw Ruby to craft.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "berylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Raw Bronze and 1 Raw Beryl to craft.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzeberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Bronze Bar and 1 Flawed Beryl to craft.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzerubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzesapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzeheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bronzeemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar and 1 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar and 1 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Hy-Brasyl Bar and 3 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Hy-Brasyl Bar and 3 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "basicemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished iron, and 1 Uncut Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "ironemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar and 1 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar and 1 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "mythrilemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Hy-Brasyl Bar and 3 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Hy-Brasyl Bar and 3 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "hybrasylemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "seanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Raw Bronze and 1 Raw Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "earthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Raw Bronze and 1 Raw Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "firenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Raw Bronze and 1 Raw Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "windnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Raw Bronze and 1 Raw Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "boneearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bonefirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "boneseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "bonewindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Bronze Bar and 1 Flawed Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "kannaearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "kannafirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "kannaseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "kannawindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Iron Bar and 1 Uncut Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedfirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar and 1 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "polishedwindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Mythril Bar and 1 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Hy-Brasyl Bar and 3 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starfirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name} requires 1 Polished Hy-Brasyl Bar and 3 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    case "starwindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 1 Polished Hy-Brasyl Bar and 3 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Crimsonite Bar and 5 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniterubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Crimsonite Bar and 5 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsonitesapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Crimsonite Bar and 5 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Crimsonite Bar and 5 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Crimsonite Bar and 5 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                        case "azuriumberylring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Azurium Bar and 5 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumrubyring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Azurium Bar and 5 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumsapphirering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Azurium Bar and 5 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumemeraldring":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Azurium Bar and 5 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumheartstonering":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 3 Polished Azurium Bar and 5 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                        case "crimsoniteberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Crimsonite Bar and 4 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniterubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Crimsonite Bar and 4 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsonitesapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Crimsonite Bar and 4 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Crimsonite Bar and 4 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Crimsonite Bar and 4 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                        case "azuriumberylearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Azurium Bar and 4 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumrubyearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Azurium Bar and 4 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumsapphireearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Azurium Bar and 4 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumemeraldearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Azurium Bar and 4 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumheartstoneearrings":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 4 Polished Azurium Bar and 4 Pristine Heartstone.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Crimsonite Bar and 4 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsonitefirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Crimsonite Bar and 4 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsoniteseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Crimsonite Bar and 4 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsonitewindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Crimsonite Bar and 4 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsonitelightnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 20 Polished Crimsonite Bar and 1 Radiant Pearl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "crimsonitedarknecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 20 Polished Crimsonite Bar and 1 Eclipse Pearl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                            case "azuriumearthnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Azurium Bar and 4 Pristine Beryl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumfirenecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Azurium Bar and 4 Pristine Ruby.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumseanecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Azurium Bar and 4 Pristine Sapphire.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumwindnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 5 Polished Azurium Bar and 4 Pristine Emerald.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumlightnecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 20 Polished Azurium Bar and 1 Radiant Pearl.",
                            "jewelcraftingbook");

                        return;
                    }
                    
                    case "azuriumdarknecklace":
                    {
                        Subject.Reply(
                            source,
                            $"{FauxItem.Template.Name
                            } requires 20 Polished Azurium Bar and 1 Eclipse Pearl.",
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
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Raw Bronze and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Eppe, 2 Raw Bronze, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }

                    case "saber":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Raw Bronze and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Saber, 1 Raw Bronze, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "claidheamh":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Claidheamn, 1 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "broadsword":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Broad Sword, 1 Polished Bronze, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "battlesword":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Bronze Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Battle Sword, 2 Polished Bronze Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "masquerade":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Polished Bronze Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Masquerade, 2 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "bramble":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Bramble, 1 Polished Iron Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "longsword":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Long Sword, 1 Polished Iron Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "claidhmore":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Mythril Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Claidhmore, 2 Polished Mythril Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "emeraldsword":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Mythril Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Emerald Sword, 2 Polished Mythril Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "gladius":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Polished Mythril Bar and 4 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Gladius, 3 Polished Mythril Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "kindjal":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-Brasyl Bar and 5 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Kindjal, 3 Polished Hy-Brasyl Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "dragonslayer":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Dragon Slayer, 5 Polished Hy-Brasyl Bar, and 5 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "hatchet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Hatchet, 1 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "harpoon":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Harpoon, 1 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "scimitar":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Scimitar, 2 Polished Bronze Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "club":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Club, 2 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "spikedclub":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Club, 1 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Spiked Club, 3 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "chainmace":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Iron Bar and 3 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Chain Mace, 3 Polished Iron Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "handaxe":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Mythril Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Hand Axe, 2 Polished Mythril Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "cutlass":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Cutlass, 3 Polished Mythril Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "talgoniteaxe":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Hy-brasyl Bar and 5 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Talgonite Axe, 4 Polished Hy-Brasyl Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "hybrasylbattleaxe":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Hy-Brasyl Axe, 5 Polished Hy-brasyl Bar, and 5 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "magusares":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Magus Ares, 2 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "holyhermes":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Holy Hermes, 2 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "maguszeus":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Magus Zeus, 2 Polished Iron Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "holykronos":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Holy Kronos, 2 Polished Iron Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "magusdiana":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Magus Diana, 3 Polished Mythril Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "holydiana":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Holy Diana, 3 Polished Mythril Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "stonecross":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Stone Cross, 4 Polished Hy-Brasyl Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "oakstaff":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Oak Staff, 5 Polished Hy-brasyl Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "staffofwisdom":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Staff of Wisdom, 5 Polished Hy-brasyl Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "snowdagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Raw Bronze and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Snow Dagger, 1 Raw Bronze, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "centerdagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Raw Bronze and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Center Dagger, 1 Raw Bronze, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "blossomdagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Blossom Dagger, 2 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "curveddagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.TemplateKey
                            } requires 1 Curved Dagger, 3 Polished Bronze Bar, and 1 Coal.");

                        return;
                    }
                    case "moondagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Bronze Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Moon Dagger, 3 Polished Bronze Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "lightdagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Light Dagger, 2 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "sundagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Iron Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Sun Dagger, 3 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "lotusdagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Mythril Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Lotus Dagger, 2 Polished Mythril Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "blooddagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Mythril Bar and 3 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Blood Dagger, 3 Polished Mythril Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "nagetierdagger":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Nagetier Dagger, 5 Polished Hy-brasyl Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "dullclaw":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Raw Bronze and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Dull Claw, 1 Raw Bronze, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "wolfclaw":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Wolf Claw, 2 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "eagletalon":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Iron Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Eagle Talon, 2 Polished Iron Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                        case "stonefist":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Stone Fist, 3 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "phoenixclaw":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Mythril Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Phoenix Claw, 2 Polished Mythril Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "nunchaku":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Nunchaku, 5 Polished Hy-brasyl Bar, and 4 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "woodenshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Raw Bronze and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Wooden Shield, 1 Raw Bronze, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "leathershield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Bronze Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Leather Shield, 2 Polished Bronze Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "bronzeshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Polished Bronze Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Bronze Shield, 3 Polished Bronze Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "gravelshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Gravel Shield, 2 Polished Iron Bar, and 1 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "ironshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Iron Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Iron Shield, 3 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "lightshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\n{FauxItem.Template.Name
                            } cannot be crafted.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Light Shield, 3 Polished Iron Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "mythrilshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Mythril Bar and 2 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Mythril Shield, 3 Polished Mythril Bar, and 2 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "hybrasylshield":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-brasyl Bar and 1 Coal.\nUpgrading {FauxItem.Template.Name
                            } requires 1 Hy-Brasyl Shield, 4 Polished Hy-brasyl Bar, and 3 Coal.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "enchantstone":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Strange Stone and 1 Radiant Pearl. Cannot be upgraded.",
                            "weaponsmithingbook");

                        return;
                    }
                    case "empowerstone":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Strange Stone and 1 Eclipse Pearl. Cannot be upgraded.",
                            "weaponsmithingbook");

                        return;
                    }
                    default:
                    {
                        // Handle cases where the provided recipe key doesn't match any known recipe
                        Subject.Reply(
                            source,
                            "Recipe not found, please notify a gm.",
                            "weaponsmithingbook");

                        return;
                    }
                }
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
                    case "scoutleatherpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }
                    case "dwarvishleatherpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }
                    case "palutenpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "keatonpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "bardoclepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }
                    case "gardcorppattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "journeymanpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "lorumpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "manepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "duinuasalpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "cowlpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "galuchatcoatpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mantlepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hierophantpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "dalmaticapattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "dobokpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "culottepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "earthgarbpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "windgarbpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mountaingarbpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "leathertunicpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "loricapattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "kasmaniumarmorpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "ipletmailpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hybrasylplatepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "cottepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "brigandinepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "corsettepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "pebblerosepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "kagumpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "magiskirtpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "benustapattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "stollerpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "clymouthpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "clamythpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "gorgetgownpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mysticgownpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "ellepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "dolmanpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "bansagartpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "earthbodicepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "lotusbodicepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "moonbodicepattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "lightninggarbpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "seagarbpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }

                    case "leatherbliautpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Fine Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "cuirasspattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen.",
                            "armorsmithingbook");

                        return;
                    }

                    case "kasmaniumhauberkpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Cotton.",
                            "armorsmithingbook");

                        return;
                    }

                    case "phoenixmailpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 4 Exquisite Wool.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hybrasylarmorpattern":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Exquisite Silk.",
                            "armorsmithingbook");

                        return;
                    }
                    case "leathersapphiregauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen and 1 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }

                    case "leatherrubygauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen and 1 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }

                    case "leatheremeraldgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen and 1 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }

                    case "leatherheartstonegauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen and 1 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "leatherberylgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen and 1 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    

                    case "bronzesapphiregauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Exquisite Linen, 1 Polished Bronze Bar, and 1 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }

                    case "bronzerubygauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Exquisite Linen, 1 Polished Bronze Bar, and 1 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }

                    case "bronzeemeraldgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Exquisite Linen, 1 Polished Bronze Bar, and 1 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }

                    case "bronzeheartstonegauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Exquisite Linen, 1 Polished Bronze Bar, and 1 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "bronzeberylgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Exquisite Linen, 1 Polished Bronze Bar, and 1 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "ironsapphiregauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Iron Bar, 2 Exquisite Cotton, and 1 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }

                    case "ironrubygauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Iron Bar, 2 Exquisite Cotton, and 1 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }

                    case "ironemeraldgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Iron Bar, 2 Exquisite Cotton, and 1 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }

                    case "ironheartstonegauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Iron Bar, 2 Exquisite Cotton, and 1 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "ironberylgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Iron Bar, 2 Exquisite Cotton, and 1 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mythrilsapphiregauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar, 2 Exquisite Wool, and 1 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mythrilrubygauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar, 2 Exquisite Wool, and 1 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mythrilemeraldgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar, 2 Exquisite Wool, and 1 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }

                    case "mythrilheartstonegauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar, 2 Exquisite Wool, and 1 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "mythrilberylgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 1 Polished Mythril Bar, 2 Exquisite Wool, and 1 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hybrasylsapphiregauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-brasyl Bars, 3 Exquisite Silk, and 2 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hybrasylrubygauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-brasyl Bars, 3 Exquisite Silk, and 2 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hybrasylemeraldgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-brasyl Bars, 3 Exquisite Silk, and 2 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }

                    case "hybrasylheartstonegauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-brasyl Bars, 3 Exquisite Silk, and 2 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "hybrasylberylgauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Polished Hy-brasyl Bars, 3 Exquisite Silk, and 2 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    case "jeweledseabelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Fine Linen and 1 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }
                    case "seabelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires requires 5 Linen and 1 Raw Sapphire.",
                            "armorsmithingbook");

                        return;
                    }
                    case "windbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires requires 5 Linen and 1 Raw Emerald.",
                            "armorsmithingbook");

                        return;
                    }
                    case "firebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Linen and 1 Raw Ruby.",
                            "armorsmithingbook");

                        return;
                    }
                    case "earthbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 5 Linen and 1 Raw Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "firebronzebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen, 1 Polished Bronze Bar and 2 Flawed Ruby.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "seabronzebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen, 1 Polished Bronze Bar and 2 Flawed Sapphire.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "earthbronzebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen, 1 Polished Bronze Bar and 2 Flawed Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "windbronzebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen, 1 Polished Bronze Bar and 2 Flawed Emerald.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "darkbronzebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen, 1 Polished Bronze Bar and 2 Flawed Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "lightbronzebelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Linen, 1 Polished Bronze Bar and 2 Flawed Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "fireironbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Cotton, 1 Polished Iron Bar and 2 Uncut Ruby.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "seaironbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Cotton, 1 Polished Iron Bar and 2 Uncut Sapphire.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "earthironbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Cotton, 1 Polished Iron Bar and 2 Uncut Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "windironbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Cotton, 1 Polished Iron Bar and 2 Uncut Emerald.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "darkironbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Cotton, 1 Polished Iron Bar and 2 Uncut Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "lightironbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Cotton, 1 Polished Iron Bar and 2 Uncut Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "firemythrilbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Wool, 1 Polished Mythril Bar and 2 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "seamythrilbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Wool, 1 Polished Mythril Bar and 2 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "earthmythrilbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Wool, 1 Polished Mythril Bar and 2 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "windmythrilbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Wool, 1 Polished Mythril Bar and 2 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "darkmythrilbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Wool, 1 Polished Mythril Bar and 2 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "lightmythrilbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 2 Exquisite Wool, 1 Polished Mythril Bar and 2 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "firehybrasylbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Silk, 1 Polished Hybrasyl Bar and 5 Pristine Ruby.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "seahybrasylbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Silk, 1 Polished Hybrasyl Bar and 5 Pristine Sapphire.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "earthhybrasylbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Silk, 1 Polished Hybrasyl Bar and 5 Pristine Beryl.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "windhybrasylbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Silk, 1 Polished Hybrasyl Bar and 5 Pristine Emerald.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "darkhybrasylbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Silk, 1 Polished Hybrasyl Bar and 5 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "lighthybrasylbraidbelt":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 3 Exquisite Silk, 1 Polished Hybrasyl Bar and 5 Pristine Heartstone.",
                            "armorsmithingbook");

                        return;
                    }
                    
                    case "leathergauntlet":
                    {
                        Subject.Reply(
                            source,
                            $"Level Required: {FauxItem.Level}.\nCrafting {FauxItem.Template.Name
                            } requires 8 Linen",
                            "armorsmithingbook");

                        return;
                    }
                    
                }

                break;
            }
            #endregion
        }
    }
}