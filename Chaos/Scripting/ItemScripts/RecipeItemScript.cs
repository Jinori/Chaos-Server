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
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedscoutleather);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedgardcorp);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedcowl);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedcotte);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedmagiskirt);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedgorgetgown);

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
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.refineddwarvishleather);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedjourneyman);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedgaluchatcoat);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedbrigadine);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedbenusta);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedmysticgown);

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
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedpaluten);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedlorum);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedmantle);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedcorsette);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedstoller);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedelle);

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
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedkeaton);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedmane);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedhierophant);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedpebblerose);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedclymouth);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refineddolman);

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
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedkagum);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedclamyth);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedbansagart);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedbardocle);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refinedduinuasal);
                    source.Trackers.Flags.AddFlag(CraftedArmors.refineddalmatica);

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

            #region Weapon Smithing Recipes
            case "recipe_basicswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.basicswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.basicswords,
                        "Basic Swords",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.apprenticeswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.apprenticeswords,
                        "Apprentice Swords",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.journeymanswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.journeymanswords,
                        "Journeyman Swords",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.adeptswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.adeptswords,
                        "Adept Swords",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.basicweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.basicweapons,
                        "Basic Weapons",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.apprenticeweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.apprenticeweapons,
                        "Apprentice Weapons",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.journeymanweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.journeymanweapons,
                        "Journeyman Weapons",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.adeptweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.adeptweapons,
                        "Adept Weapons",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.basicstaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.basicstaves,
                        "Basic Staves",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticestaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.apprenticestaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.apprenticestaves,
                        "Apprentice Staves",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.journeymanstaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.journeymanstaves,
                        "Journeyman Staves",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.adeptstaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.adeptstaves,
                        "Adept Staves",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "basicdaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.basicdaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.basicdaggers,
                        "Basic Daggers",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "apprenticedaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.apprenticedaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.apprenticedaggers,
                        "Apprentice Daggers",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "journeymandaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.journeymandaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.journeymandaggers,
                        "Journeyman Daggers",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "adeptdaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.adeptdaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.adeptdaggers,
                        "Adept Daggers",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "basicclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.basicclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.basicclaws,
                        "Basic Claws",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "apprenticeclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.apprenticeclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.apprenticeclaws,
                        "Apprentice Claws",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "journeymanclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.journeymanclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.journeymanclaws,
                        "Journeyman Claws",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "basicshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.basicshields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.basicshields,
                        "Basic Shields",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "apprenticeshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.apprenticeshields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.apprenticeshields,
                        "Apprentice Shields",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "journeymanshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithRecipes.journeymanshields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithRecipes.journeymanshields,
                        "Journeyman Shields",
                        $"{Subject.Template.TemplateKey}");
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion
            
            #region Alchemy Recipes
            
            case "recipe_hemloch":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.Hemloch))
                {
                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.Hemloch,
                        "Hemloch",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_betonydeum":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.BetonyDeum))
                {
                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.BetonyDeum,
                        "Betony Deum",
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
    public static void WeaponSmithRecipeLearn(
        Aisling source,
        Animation ani,
        WeaponSmithRecipes recipe,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(recipe);
        source.SendOrangeBarMessage($"You've learned {serverMessage}.");
        source.Inventory.RemoveQuantity(templatekey, 1);
    }
    public static void AlchemyRecipeLearn(
        Aisling source,
        Animation ani,
        AlchemyRecipes recipe,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(recipe);
        source.SendOrangeBarMessage($"You've learned {serverMessage}.");
        source.Inventory.RemoveQuantity(templatekey, 1);
    }
    public static void EnchantingRecipeLearn(
        Aisling source,
        Animation ani,
        EnchantingRecipes recipe,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(recipe);
        source.SendOrangeBarMessage($"You've learned {serverMessage}.");
        source.Inventory.RemoveQuantity(templatekey, 1);
    }
    public static void JewelcraftingRecipeLearn(
        Aisling source,
        Animation ani,
        JewelcraftingRecipes recipe,
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