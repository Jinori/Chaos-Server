using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
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
                            Subject.Reply(source, "This recipe requires 15 fruit of any type, 5 vegetable of any type, and 2 flour.", "cookbook");

                            return;
                        }
                        case "fruitbasket":
                        {
                            Subject.Reply(source, "This recipe requires 15 fruit of any type, 10 fruit of any type, and 5 fruit of any type. Must be all different fruits.", "cookbook");

                            return;
                        }
                        case "lobsterdinner":
                        {
                            Subject.Reply(source, "This recipe requires a lobster tail, 25 cherries, 5 vegetables, 5 tomatos, and 1 salt.", "cookbook");

                            return;
                        }
                        case "pie":
                        {
                            Subject.Reply(source, "This recipe requires 50 fruit of any type. The type of pie made depends on the fruit used.", "cookbook");

                            return;
                        }
                        case "salad":
                        {
                            Subject.Reply(source, "This recipe requires 1 meat of any type, 15 vegetables of any type, 15 vegetables of any type, and 1 cheese. The two vegetables must be different.", "cookbook");

                            return;
                        }
                        case "sandwich":
                        {
                            Subject.Reply(source, "This recipe requires 1 meat of any type, 5 vegetable, 5 tomato, 2 bread, and 1 cheese.", "cookbook");

                            return;
                        }
                        case "soup":
                        {
                            Subject.Reply(source, "This recipe requires 1 meat of any type, 5 vegetable of any type, 1 flour, and 1 salt.", "cookbook");

                            return;
                        }
                        case "steakmeal":
                        {
                            Subject.Reply(source, "This recipe requires 1 raw meat, 10 fruit of any type, 5 vegetable of any type, and 1 marinade.", "cookbook");
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
                
                #region Jewelcrafting Book
                case "jewelcraftingbook":
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
                        case "bronzeberylring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed beryl.", "jewelcraftingbook");
                            return;
                        }
                        case "bronzerubyring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed ruby.", "jewelcraftingbook");
                            return;
                        }
                        case "bronzesapphirering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed sapphire.", "jewelcraftingbook");
                            return;
                        }
                        case "bronzeheartstonering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed heartstone.", "jewelcraftingbook");
                            return;
                        }
                        case "bronzeemeraldring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed emerald.", "jewelcraftingbook");
                            return;
                        }
                        case "ironberylring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut beryl.", "jewelcraftingbook");
                            return;
                        }
                        case "ironrubyring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut ruby.", "jewelcraftingbook");
                            return;
                        }
                        case "ironsapphirering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut sapphire.", "jewelcraftingbook");
                            return;
                        }
                        case "ironheartstonering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut heartstone.", "jewelcraftingbook");
                            return;
                        }
                        case "ironemeraldring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut emerald.", "jewelcraftingbook");
                            return;
                        }
                        case "mythrilberylring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished beryl.", "jewelcraftingbook");
                            return;
                        }
                        case "mythrilrubyring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished ruby.", "jewelcraftingbook");
                            return;
                        }
                        case "mythrilsapphirering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished sapphire.", "jewelcraftingbook");
                            return;
                        }
                        case "mythrilheartstonering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished heartstone.", "jewelcraftingbook");
                            return;
                        }
                        case "mythrilemeraldring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished emerald.", "jewelcraftingbook");
                            return;
                        }
                        case "hybrasylberylring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished beryl.", "jewelcraftingbook");
                            return;
                        }
                        case "hybrasylrubyring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished ruby.", "jewelcraftingbook");
                            return;
                        }
                        case "hybrasylsapphirering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished sapphire.", "jewelcraftingbook");
                            return;
                        }
                        case "hybrasylheartstonering":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished heartstone.", "jewelcraftingbook");
                            return;
                        }
                        case "hybrasylemeraldring":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished emerald.", "jewelcraftingbook");
                            return;
                        }
                        case "basicberylearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "basicrubyearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "basicsapphireearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "basicheartstoneearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed heartstone.", "jewelcraftingbook");

                            return;
                        }
                        case "basicemeraldearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "ironberylearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron, and 1 uncut beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "ironrubyearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "ironsapphireearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "ironheartstoneearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut heartstone.", "jewelcraftingbook");

                            return;
                        }
                        case "ironemeraldearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "mythrilberylearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "mythrilrubyearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "mythrilsapphireearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "mythrilheartstoneearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished heartstone.", "jewelcraftingbook");

                            return;
                        }
                        case "mythrilemeraldearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "hybrasylberylearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "hybrasylrubyearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "hybrasylsapphireearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "hybrasylheartstoneearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished heartstone.", "jewelcraftingbook");

                            return;
                        }
                        case "hybrasylemeraldearrings":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "basicearthnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "basicfirenecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "basicseanecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "basicwindnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished bronze and 1 flawed emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "apprenticeearthnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "apprenticefirenecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "apprenticeseanecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "apprenticewindnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished iron and 1 uncut emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "polishedearthnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished beryl.", "jewelcraftingbook");

                            return;
                        }
                        case "polishedfirenecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "polishedseanecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "polishedwindnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished mythril and 1 finished emerald.", "jewelcraftingbook");

                            return;
                        }
                        case "starearthnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished beryl.", "jewelcraftingbook");

                            return;
                            
                        }
                        case "starfirenecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished ruby.", "jewelcraftingbook");

                            return;
                        }
                        case "starseanecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished sapphire.", "jewelcraftingbook");

                            return;
                        }
                        case "starwindnecklace":
                        {
                            Subject.Reply(source, "This recipe requires 1 polished hybrasyl and 3 finished emerald.", "jewelcraftingbook");

                            return;
                        }
                        
                        
                        
                    }

                    break;
                }

                #endregion
                
                
            }
        }
    }
}
        