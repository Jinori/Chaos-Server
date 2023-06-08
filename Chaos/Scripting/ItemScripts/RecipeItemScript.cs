using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

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
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.DinnerPlate))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.DinnerPlate,
                        "Dinner Plate",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_sweetbuns":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.SweetBuns))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.SweetBuns,
                        "Sweet Buns",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_fruitbasket":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.FruitBasket))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.FruitBasket,
                        "Fruit Basket",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_salad":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.Salad))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.Salad,
                        "Salad",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_lobsterdinner":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.LobsterDinner))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.LobsterDinner,
                        "Lobster Dinner",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_pie":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.Pie))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.Pie,
                        "Pie",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_sandwich":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.Sandwich))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.Sandwich,
                        "Sandwich",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_soup":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.Soup))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.Soup,
                        "Soup",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_steakmeal":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.SteakMeal))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.SteakMeal,
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
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Basicarmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.Basicarmors,
            "Basic Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedscoutleather);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedgardcorp);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcowl);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcotte);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmagiskirt);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedgorgetgown);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refineddobok);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedleathertunic);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedearthbodice);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedleatherbliaut);

        return;
    }

    source.SendOrangeBarMessage("You already know this recipe");

    return;
}

                #region All Recipes
            case "recipe_allcrafts":
            {
                source.Trackers.Flags.AddFlag(CookingRecipes.DinnerPlate);
                source.Trackers.Flags.AddFlag(CookingRecipes.FruitBasket);
                source.Trackers.Flags.AddFlag(CookingRecipes.LobsterDinner);
                source.Trackers.Flags.AddFlag(CookingRecipes.Sandwich);
                source.Trackers.Flags.AddFlag(CookingRecipes.Pie);
                source.Trackers.Flags.AddFlag(CookingRecipes.Salad);
                source.Trackers.Flags.AddFlag(CookingRecipes.Soup);
                source.Trackers.Flags.AddFlag(CookingRecipes.SteakMeal);
                source.Trackers.Flags.AddFlag(CookingRecipes.SweetBuns);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refineddwarvishleather);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedjourneyman);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedgaluchatcoat);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbrigandine);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbenusta);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmysticgown);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlotusbodice);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcuirass);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedculotte);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlorica);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedpaluten);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlorum);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmantle);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcorsette);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedstoller);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedelle);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmoonbodice);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkasmaniumhauberk);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedearthgarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkasmaniumarmor);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkeaton);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmane);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedhierophant);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedpebblerose);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedclymouth);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refineddolman);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlightninggarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedphoenixmail);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedwindgarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedipletmail);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkagum);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedclamyth);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbansagart);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbardocle);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedduinuasal);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refineddalmatica);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedseagarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedhybrasylarmor);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmountaingarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedhybrasylplate);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedscoutleather);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedgardcorp);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcowl);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcotte);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmagiskirt);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedgorgetgown);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refineddobok);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedleathertunic);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedearthbodice);
                source.Trackers.Flags.AddFlag(CraftedArmors.Refinedleatherbliaut);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leathersapphiregauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leatherrubygauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leatheremeraldgauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leatherheartstonegauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironemeraldgauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironrubygauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironheartstonegauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironsapphiregauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilemeraldgauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilrubygauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilheartstonegauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilsapphiregauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylemeraldgauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylrubygauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylheartstonegauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylsapphiregauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledseabelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledearthbelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledwindbelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledfirebelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledmetalbelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jewelednaturebelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweleddarkbelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledlightbelt);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eppe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Saber);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Snowdagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Centerdagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Dullclaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Woodenshield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidheamh);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Broadsword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Battlesword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Masquerade);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bramble);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Longsword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidhmore);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Emeraldsword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gladius);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Kindjal);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Dragonslayer);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hatchet);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Harpoon);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Scimitar);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Club);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Spikedclub);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Chainmace);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Handaxe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Cutlass);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Talgoniteaxe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hybrasylaxe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Magusares);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Holyhermes);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Maguszeus);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Holykronos);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Magusdiana);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Holydiana);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Stonecross);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Oakstaff);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Staffofwisdom);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Blossomdagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Curveddagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Moondagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Lightdagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Sundagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Lotusdagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Blooddagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Nagetierdagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Wolfclaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eagletalon);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Phoenixclaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Nunchaku);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Leathershield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bronzeshield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Ironshield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gravelshield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Lightshield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Mythrilshield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hybrasylshield);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisEmbrace);
                
                source.Animate(ani);
                source.SendOrangeBarMessage($"You've learned all recipes.");
                source.Inventory.RemoveQuantity("recipe_allcrafts", 1);

                return;
            }
                
            #endregion
            case "recipe_apprenticearmors":
{
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Apprenticearmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.Apprenticearmors,
            "Apprentice Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.Refineddwarvishleather);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedjourneyman);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedgaluchatcoat);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbrigandine);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbenusta);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmysticgown);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlotusbodice);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcuirass);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedculotte);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlorica);

        return;
    }

    source.SendOrangeBarMessage("You already know this recipe");

    return;
}
case "recipe_journeymanarmors":
{
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Journeymanarmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.Journeymanarmors,
            "Journeyman Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedpaluten);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlorum);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmantle);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedcorsette);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedstoller);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedelle);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmoonbodice);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkasmaniumhauberk);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedearthgarb);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkasmaniumarmor);

        return;
    }

    source.SendOrangeBarMessage("You already know this recipe.");

    return;
}
case "recipe_adeptarmors":
{
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Adeptarmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.Adeptarmors,
            "Adept Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkeaton);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmane);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedhierophant);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedpebblerose);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedclymouth);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refineddolman);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedlightninggarb);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedphoenixmail);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedwindgarb);
        source.Trackers.Flags.AddFlag(CraftedArmors.Refinedipletmail);
        

        return;
    }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_advancedarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Advancedarmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Advancedarmors,
                        "Advanced Armors",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedkagum);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedclamyth);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbansagart);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedbardocle);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedduinuasal);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refineddalmatica);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedseagarb);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedhybrasylarmor);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedmountaingarb);
                    source.Trackers.Flags.AddFlag(CraftedArmors.Refinedhybrasylplate);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Basicgauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Basicgauntlets,
                        "Basic Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leathersapphiregauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leatherrubygauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leatheremeraldgauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Leatherheartstonegauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticegauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Apprenticegauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Apprenticegauntlets,
                        "Apprentice Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironemeraldgauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironrubygauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironheartstonegauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Ironsapphiregauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymangauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Journeymangauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Journeymangauntlets,
                        "Journeyman Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilemeraldgauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilrubygauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilheartstonegauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Mythrilsapphiregauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Adeptgauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Adeptgauntlets,
                        "Adept Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylemeraldgauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylrubygauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylheartstonegauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Hybrasylsapphiregauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_Basicbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Basicbelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Basicbelts,
                        "Basic Belts",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledseabelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledearthbelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledwindbelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledfirebelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticebelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Apprenticebelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Apprenticebelts,
                        "Apprentice Belts",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledmetalbelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jewelednaturebelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.Journeymanbelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.Journeymanbelts,
                        "Journeyman Belts",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweleddarkbelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.Jeweledlightbelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion

            #region Weapon Smithing Recipes
            case "recipe_basicswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Basicswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Basicswords,
                        "Basic Swords",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidheamh);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Broadsword);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Battlesword);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Masquerade);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Apprenticeswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Apprenticeswords,
                        "Apprentice Swords",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bramble);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Longsword);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Journeymanswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Journeymanswords,
                        "Journeyman Swords",
                        $"{Subject.Template.TemplateKey}");
                        
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidhmore);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Emeraldsword);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gladius);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Adeptswords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Adeptswords,
                        "Adept Swords",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Kindjal);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Dragonslayer);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Basicweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Basicweapons,
                        "Basic Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hatchet);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Harpoon);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Scimitar);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Apprenticeweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Apprenticeweapons,
                        "Apprentice Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Club);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Spikedclub);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Chainmace);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Journeymanweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Journeymanweapons,
                        "Journeyman Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Handaxe);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Cutlass);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Adeptweapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Adeptweapons,
                        "Adept Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Talgoniteaxe);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hybrasylaxe);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Basicstaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Basicstaves,
                        "Basic Staves",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Magusares);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Holyhermes);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticestaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Apprenticestaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Apprenticestaves,
                        "Apprentice Staves",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Maguszeus);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Holykronos);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Journeymanstaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Journeymanstaves,
                        "Journeyman Staves",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Magusdiana);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Holydiana);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Adeptstaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Adeptstaves,
                        "Adept Staves",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Stonecross);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Oakstaff);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Staffofwisdom);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicdaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Basicdaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Basicdaggers,
                        "Basic Daggers",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Blossomdagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Curveddagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Moondagger);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticedaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Apprenticedaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Apprenticedaggers,
                        "Apprentice Daggers",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Lightdagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Sundagger);
                    
                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymandaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Journeymandaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Journeymandaggers,
                        "Journeyman Daggers",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Lotusdagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Blooddagger);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptdaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Adeptdaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Adeptdaggers,
                        "Adept Daggers",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Nagetierdagger);
                  
                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Basicclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Basicclaws,
                        "Basic Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Wolfclaw);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Apprenticeclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Apprenticeclaws,
                        "Apprentice Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eagletalon);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Journeymanclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Journeymanclaws,
                        "Journeyman Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Phoenixclaw);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_adeptclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Adeptclaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Adeptclaws,
                        "Adept Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Nunchaku);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Basicshields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Basicshields,
                        "Basic Shields",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Leathershield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bronzeshield);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Apprenticeshields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Apprenticeshields,
                        "Apprentice Shields",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Ironshield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gravelshield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Lightshield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Mythrilshield);
                    
                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.Journeymanshields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.Journeymanshields,
                        "Journeyman Shields",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hybrasylshield);
                    
                    return;
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
        ArmorSmithCategories recipe,
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
        WeaponSmithingCategories category,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(category);
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