using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class RecipeItemScript : ItemScriptBase
{

    public RecipeItemScript(Item subject)
        : base(subject) { }


    public override void OnUse(Aisling source)
    {
        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 21
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region Cooking Recipes
            case "recipe_dinnerplate":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.dinnerplate))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.dinnerplate,
                        "Dinner Plate",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_sweetbuns":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.sweetbuns))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.sweetbuns,
                        "Sweet Buns",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_fruitbasket":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.fruitbasket))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.fruitbasket,
                        "Fruit Basket",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_salad":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.salad))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.salad,
                        "Salad",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_lobsterdinner":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.lobsterdinner))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.lobsterdinner,
                        "Lobster Dinner",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_pie":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.pie))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.pie,
                        "Pie",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_sandwich":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.sandwich))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.sandwich,
                        "Sandwich",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_soup":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.soup))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.soup,
                        "Soup",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_steakmeal":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.steakmeal))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.steakmeal,
                        "Steak Meal",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion

            #region Armor Smithing Recipes
            case "recipe_basicarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.basicarmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.basicarmors,
                        "Basic Armors",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_apprenticearmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.apprenticearmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.apprenticearmors,
                        "Apprentice Armors",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.journeymanarmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.journeymanarmors,
                        "Journeyman Armors",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.adeptarmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.adeptarmors,
                        "Adept Armors",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_advancedarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.advancedarmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.advancedarmors,
                        "Advanced Armors",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.basicgauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.basicgauntlets,
                        "Basic Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticegauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.apprenticegauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.apprenticegauntlets,
                        "Apprentice Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymangauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.journeymangauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.journeymangauntlets,
                        "Journeyman Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.adeptgauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.adeptgauntlets,
                        "Adept Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.basicbelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.basicbelts,
                        "Basic Belts",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticebelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.apprenticebelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.apprenticebelts,
                        "Apprentice Belts",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithRecipes.journeymanbelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithRecipes.journeymanbelts,
                        "Journeyman Belts",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion
        }
    }

    public static void CookingRecipeLearn(
        Aisling source,
        Animation ani,
        CookingRecipes recipe,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(recipe);
        source.SendOrangeBarMessage($"You've learned {serverMessage}.");
        source.Inventory.RemoveQuantity(templatekey, 1);
    }
    
    public static void ArmorSmithRecipeLearn(
        Aisling source,
        Animation ani,
        ArmorSmithRecipes recipe,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(recipe);
        source.SendOrangeBarMessage($"You've learned {serverMessage}.");
        source.Inventory.RemoveQuantity(templatekey, 1);
    }
}