using System.Text;
using System.Text.RegularExpressions;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Generic
{
    public class RecipeLibraryScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private ItemDetails? ItemDetails;
        protected HashSet<string>? ItemTemplateKeys { get; init; }
        private Item? FauxItem => ItemDetails?.Item;

        public RecipeLibraryScript(Dialog subject, IItemFactory itemFactory)
            : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnDisplaying(Aisling source)
        {
            var hasFlag1 = source.Trackers.Flags.TryGetFlag(out CookingRecipes _);
            var hasFlag2 = source.Trackers.Flags.TryGetFlag(out AlchemyRecipes _);
            var hasFlag3 = source.Trackers.Flags.TryGetFlag(out ArmorSmithRecipes _);
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
                    {
                        // Iterating through the Alchemy recipe requirements.
                        foreach (var recipe in CraftingRequirements.AlchemyRequirements)
                        {
                            // Checking if the recipe is available or not.
                            if (recipes.HasFlag(recipe.Key))
                            {
                                // Creating a faux item for the recipe.
                                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                                // Adding the recipe to the subject's dialog window.
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                            }
                        }
                    }

                    break;
                }
                #endregion
                #region Armorsmithing Book
                case "armorsmithingbook":
                {
                    if (source.Trackers.Flags.TryGetFlag(out CraftedArmors armorRecipes))
                    {
                        foreach (var recipe in CraftingRequirements.ArmorSmithingArmorRequirements)
                        {
                            if (armorRecipes.HasFlag(recipe.Key))
                            {
                                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                            }
                        }
                    }

                    if (source.Trackers.Flags.TryGetFlag(out ArmorSmithRecipes gearRecipes))
                    {
                        foreach (var recipe in CraftingRequirements.ArmorSmithingGearRequirements)
                        {
                            if (gearRecipes.HasFlag(recipe.Key))
                            {
                                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                            }
                        }
                    }

                    break;
                }
                #endregion
                
                #region Weaponsmithing Book
                case "weaponsmithingbook":
                {
                    if (source.Trackers.Flags.TryGetFlag(out WeaponSmithingRecipes recipes))
                    {
                        foreach (var recipe in CraftingRequirements.WeaponSmithingCraftRequirements)
                        {
                            if (recipes.HasFlag(recipe.Key))
                            {
                                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                            }
                        }
                    }

                    break;
                }

                #endregion
                
                #region Enchanting Book
                case "enchantingbook":
                {
                    // Checking if the Alchemy recipe is available or not.
                    if (source.Trackers.Flags.TryGetFlag(out EnchantingRecipes recipes))
                    {
                        // Iterating through the Alchemy recipe requirements.
                        foreach (var recipe in CraftingRequirements.EnchantingRequirements)
                        {
                            // Checking if the recipe is available or not.
                            if (recipes.HasFlag(recipe.Key))
                            {
                                // Creating a faux item for the recipe.
                                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                                // Adding the recipe to the subject's dialog window.
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                            }
                        }
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
                        // Iterating through the Alchemy recipe requirements.
                        foreach (var recipe in CraftingRequirements.JewelcraftingRequirements)
                        {
                            // Checking if the recipe is available or not.
                            if (recipes.HasFlag(recipe.Key))
                            {
                                // Creating a faux item for the recipe.
                                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                                // Adding the recipe to the subject's dialog window.
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                            }
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
                            Subject.Reply(source, DialogString.UnknownInput.Value);

                            return;
                        }

                        ItemDetails =
                            Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                        if (ItemDetails == null)
                        {
                            Subject.Reply(source, DialogString.UnknownInput.Value);

                            return;
                        }
                    }

                    switch (FauxItem?.Template.TemplateKey.ToLower())
                    {
                        case "dinnerplate":
                        {
                            Subject.Reply(source, "This recipe requires 1 meat of any type, 10 fruit of any type, and 5 vegetables of any type. To cook this, go to the Mileth Kitchen and go to the stove.", "cookbook");

                            return;
                        }
                        case "sweetbuns":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "fruitbasket":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "lobsterdinner":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "pie":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "salad":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "sandwich":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "soup":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

                            return;
                        }
                        case "steakmeal":
                        {
                            Subject.Reply(source, "Skip", "cooking_directory");

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
                            Subject.Reply(source, DialogString.UnknownInput.Value);
                            return;
                        }

                        ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                        if (ItemDetails == null)
                        {
                            Subject.Reply(source, DialogString.UnknownInput.Value);
                            return;
                        }
                    }

                    switch (FauxItem?.Template.TemplateKey.ToLower())
                    {
                        case "hemloch":
                            Subject.Reply(source, "This recipe requires something. To brew this, go to the Piet Alchemy Lab and go to the tables.", "alchemybook");
                            return;
                    }

                    break;
                }

                #endregion
                
                
            }
        }
    }
}
        