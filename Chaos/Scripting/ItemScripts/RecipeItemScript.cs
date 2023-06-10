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
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicArmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.BasicArmors,
            "Basic Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedScoutLeather);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGardcorp);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCowl);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCotte);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMagiSkirt);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGorgetGown);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDobok);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLeatherTunic);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedEarthBodice);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLeatherBliaut);

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
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDwarvishLeather);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedJourneyman);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGaluchatCoat);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBrigandine);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBenusta);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMysticGown);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLotusBodice);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCuirass);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCulotte);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLorica);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedPaluten);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLorum);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMantle);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCorsette);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedStoller);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedElle);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMoonBodice);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKasmaniumHauberk);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedEarthGarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKasmaniumArmor);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKeaton);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMane);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedHierophant);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedPebbleRose);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedClymouth);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDolman);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLightningGarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedPhoenixMail);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedWindGarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedIpletMail);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKagum);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedClamyth);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBansagart);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBardocle);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDuinUasal);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDalmatica);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedSeaGarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedHybrasylArmor);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMountainGarb);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedHybrasylPlate);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedScoutLeather);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGardcorp);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCowl);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCotte);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMagiSkirt);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGorgetGown);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDobok);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLeatherTunic);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedEarthBodice);
                source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLeatherBliaut);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledSeaBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledEarthBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledWindBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledFireBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledMetalBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledNatureBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledDarkBelt);
                source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledLightBelt);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eppe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Saber);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SnowDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CenterDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DullClaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.WoodenShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidheamh);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BroadSword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BattleSword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Masquerade);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bramble);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LongSword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidhmore);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EmeraldSword);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gladius);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Kindjal);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DragonSlayer);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hatchet);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Harpoon);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Scimitar);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Club);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SpikedClub);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.ChainMace);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HandAxe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Cutlass);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.TalgoniteAxe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HybrasylBattleAxe);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusAres);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyHermes);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusZeus);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyKronos);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusDiana);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyDiana);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StoneCross);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.OakStaff);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StaffOfWisdom);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BlossomDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CurvedDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MoonDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SunDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LotusDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BloodDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.NagetierDagger);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.WolfClaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EagleTalon);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.PhoenixClaw);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Nunchaku);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LeatherShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BronzeShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.IronShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.GravelShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MythrilShield);
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HybrasylShield);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarEnvy);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithGratitude);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisSerenity);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneElusion);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonClarity);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SerendaelLuck);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SkandaraMight);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.ZephyraSpirit);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarGrief);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithPride);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisBlessing);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneShadow);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonCalming);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SerendaelMagic);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SkandaraTriumph);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.ZephyraMist);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarRegret);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithConstitution);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisIntellect);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneDexterity);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonWisdom);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SerendaelChance);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SkandaraStrength);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.ZephyraWind);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarJealousy);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithObsession);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisHarmony);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneBalance);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonWill);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SerendaelRoll);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SkandaraDrive);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.ZephyraVortex);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarDestruction);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithFortitude);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisNurturing);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneRisk);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonResolve);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SerendaelAddiction);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SkandaraPierce);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.ZephyraGust);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeBerylRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeRubyRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeSapphireRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeEmeraldRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeHeartstoneRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneRing);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeBerylNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeRubyNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeSapphireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeEmeraldNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeBerylEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeRubyEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeSapphireEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeEmeraldEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeHeartstoneEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldEarring);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneEarring);




                source.Animate(ani);
                source.SendOrangeBarMessage($"You've learned all recipes.");
                source.Inventory.RemoveQuantity("recipe_allcrafts", 1);

                return;
            }

            #endregion
            case "recipe_apprenticearmors":
{
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.ApprenticeArmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.ApprenticeArmors,
            "Apprentice Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDwarvishLeather);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedJourneyman);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGaluchatCoat);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBrigandine);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBenusta);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMysticGown);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLotusBodice);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCuirass);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCulotte);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLorica);

        return;
    }

    source.SendOrangeBarMessage("You already know this recipe");

    return;
}
case "recipe_journeymanarmors":
{
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.JourneymanArmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.JourneymanArmors,
            "Journeyman Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedPaluten);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLorum);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMantle);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCorsette);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedStoller);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedElle);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMoonBodice);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKasmaniumHauberk);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedEarthGarb);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKasmaniumArmor);

        return;
    }

    source.SendOrangeBarMessage("You already know this recipe.");

    return;
}
case "recipe_adeptarmors":
{
    if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptArmors))
    {
        ArmorSmithRecipeLearn(
            source,
            ani,
            ArmorSmithCategories.AdeptArmors,
            "Adept Armors",
            $"{Subject.Template.TemplateKey}");

        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKeaton);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMane);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedHierophant);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedPebbleRose);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedClymouth);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDolman);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLightningGarb);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedPhoenixMail);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedWindGarb);
        source.Trackers.Flags.AddFlag(CraftedArmors.RefinedIpletMail);
        

        return;
    }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_advancedarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdvancedArmors))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.AdvancedArmors,
                        "Advanced Armors",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedKagum);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedClamyth);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBansagart);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBardocle);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDuinUasal);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDalmatica);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedSeaGarb);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedHybrasylArmor);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMountainGarb);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedHybrasylPlate);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicGauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.BasicGauntlets,
                        "Basic Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherSapphireGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.LeatherHeartstoneGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticegauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.ApprenticeGauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.ApprenticeGauntlets,
                        "Apprentice Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.IronSapphireGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymangauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.JourneymanGauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.JourneymanGauntlets,
                        "Journeyman Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.MythrilSapphireGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptGauntlets))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.AdeptGauntlets,
                        "Adept Gauntlets",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.HybrasylSapphireGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicBelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.BasicBelts,
                        "Basic Belts",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledSeaBelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledEarthBelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledWindBelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledFireBelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticebelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.ApprenticeBelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.ApprenticeBelts,
                        "Apprentice Belts",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledMetalBelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledNatureBelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.JourneymanBelts))
                {
                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.JourneymanBelts,
                        "Journeyman Belts",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledDarkBelt);
                    source.Trackers.Flags.AddFlag(ArmorSmithRecipes.JeweledLightBelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion

            #region Weapon Smithing Recipes
            case "recipe_basicswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicSwords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.BasicSwords,
                        "Basic Swords",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidheamh);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BroadSword);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BattleSword);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Masquerade);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ApprenticeSwords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ApprenticeSwords,
                        "Apprentice Swords",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bramble);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LongSword);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.JourneymanSwords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.JourneymanSwords,
                        "Journeyman Swords",
                        $"{Subject.Template.TemplateKey}");
                        
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidhmore);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EmeraldSword);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gladius);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptSwords))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.AdeptSwords,
                        "Adept Swords",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Kindjal);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DragonSlayer);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicWeapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.BasicWeapons,
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
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ApprenticeWeapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ApprenticeWeapons,
                        "Apprentice Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Club);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SpikedClub);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.ChainMace);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.JourneymanWeapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.JourneymanWeapons,
                        "Journeyman Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HandAxe);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Cutlass);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptWeapons))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.AdeptWeapons,
                        "Adept Weapons",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.TalgoniteAxe);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HybrasylBattleAxe);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicStaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.BasicStaves,
                        "Basic Staves",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusAres);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyHermes);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticestaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ApprenticeStaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ApprenticeStaves,
                        "Apprentice Staves",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusZeus);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyKronos);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.JourneymanStaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.JourneymanStaves,
                        "Journeyman Staves",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusDiana);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyDiana);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptStaves))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.AdeptStaves,
                        "Adept Staves",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StoneCross);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.OakStaff);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StaffOfWisdom);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicdaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicDaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.BasicDaggers,
                        "Basic Daggers",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BlossomDagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CurvedDagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MoonDagger);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticedaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ApprenticeDaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ApprenticeDaggers,
                        "Apprentice Daggers",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightDagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SunDagger);
                    
                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymandaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.JourneymanDaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.JourneymanDaggers,
                        "Journeyman Daggers",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LotusDagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BloodDagger);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptdaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptDaggers))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.AdeptDaggers,
                        "Adept Daggers",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.NagetierDagger);
                  
                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicClaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.BasicClaws,
                        "Basic Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.WolfClaw);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ApprenticeClaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ApprenticeClaws,
                        "Apprentice Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EagleTalon);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.JourneymanClaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.JourneymanClaws,
                        "Journeyman Claws",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.PhoenixClaw);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_adeptclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptClaws))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.AdeptClaws,
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
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicShields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.BasicShields,
                        "Basic Shields",
                        $"{Subject.Template.TemplateKey}");
                    
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LeatherShield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BronzeShield);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_apprenticeshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ApprenticeShields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ApprenticeShields,
                        "Apprentice Shields",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.IronShield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.GravelShield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightShield);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MythrilShield);
                    
                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_journeymanshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.JourneymanShields))
                {
                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.JourneymanShields,
                        "Journeyman Shields",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HybrasylShield);
                    
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
            
               #region Enchanting Recipes
            
            case "recipe_ignatarenvy":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarEnvy))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarEnvy,
                        "Ignatar's Envy",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_geolithgratitude":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithGratitude))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithGratitude,
                        "Geolith's Gratitude",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_miraelisserenity":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisSerenity))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisSerenity,
                        "Miraelis' Serenity",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_theseleneelusion":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.TheseleneElusion))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.TheseleneElusion,
                        "Theseleme's Elusion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_aquaedonclarity":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.AquaedonClarity))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.AquaedonClarity,
                        "Aquaedon's Clarity",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_serendaelluck":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SerendaelLuck))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SerendaelLuck,
                        "Serendael's Luck",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_skandaramight":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SkandaraMight))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SkandaraMight,
                        "Skandara's Might",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_zephyraspirit":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.ZephyraSpirit))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.ZephyraSpirit,
                        "Zephyra's Spirit",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_ignatargrief":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarGrief))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarGrief,
                        "Ignatar's Grief",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_geolithpride":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithPride))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithPride,
                        "Geolith's Pride",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "miraelisblessing":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisBlessing))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisBlessing,
                        "Miraelis' Blessing",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_theseleneshadow":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.TheseleneShadow))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.TheseleneShadow,
                        "Theseleme's Shadow",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_aquaedoncalming":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.AquaedonCalming))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.AquaedonCalming,
                        "Aquaedon's Calming",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_serendaelmagic":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SerendaelMagic))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SerendaelMagic,
                        "Serendael's Magic",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_skandaratriumph":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SkandaraTriumph))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SkandaraTriumph,
                        "Skandara's Triumph",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_zephyramist":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.ZephyraMist))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.ZephyraMist,
                        "Zephyra's Mist",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            
            case "recipe_ignatarregret":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarRegret))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarRegret,
                        "Ignatar's Regret",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_geolithconstitution":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithConstitution))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithConstitution,
                        "Geolith's Constitution",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_miraelisintellect":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisIntellect))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisIntellect,
                        "Miraelis' Intellect",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_theselenedexterity":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.TheseleneDexterity))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.TheseleneDexterity,
                        "Theselene's Dexterity",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_aquaedonwisdom":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.AquaedonWisdom))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.AquaedonWisdom,
                        "Aquaedon's Wisdom",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_serendaelchance":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SerendaelChance))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SerendaelChance,
                        "Serendael's Chance",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_skandarastrength":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SkandaraStrength))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SkandaraStrength,
                        "Skandara's Strength",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_zephyrawind":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.ZephyraWind))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.ZephyraWind,
                        "Zephyra's Wind",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_ignatarjealousy":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarJealousy))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarJealousy,
                        "Ignatar's Jealousy",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_geolithobsession":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithObsession))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithObsession,
                        "Geolith's Obsession",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_miraelisharmony":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisHarmony))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisHarmony,
                        "Miraelis' Harmony",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_theselenebalance":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.TheseleneBalance))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.TheseleneBalance,
                        "Theselene's Balance",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_aquaedonwill":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.AquaedonWill))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.AquaedonWill,
                        "Aquaedon's Will",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_serendaelroll":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SerendaelRoll))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SerendaelRoll,
                        "Serendael's Roll",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_skandaradrive":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SkandaraDrive))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SkandaraDrive,
                        "Skandara's Drive",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_zephyravortex":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.ZephyraVortex))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.ZephyraVortex,
                        "Zephyra's Vortex",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_ignatardestruction":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarDestruction))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarDestruction,
                        "Ignatar's Destruction",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_geolithfortitude":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithFortitude))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithFortitude,
                        "Geolith's Fortitude",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_miraelisnurturing":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisNurturing))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisNurturing,
                        "Miraelis' Nurturing",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_theselenerisk":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.TheseleneRisk))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.TheseleneRisk,
                        "Theselene's Risk",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_aquaedonresolve":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.AquaedonResolve))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.AquaedonResolve,
                        "Aquaedon's Resolve",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_serendaeladdiction":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SerendaelAddiction))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SerendaelAddiction,
                        "Serendael's Addiction",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_skandarapierce":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SkandaraPierce))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SkandaraPierce,
                        "Skandara's Pierce",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_zephyragust":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.ZephyraGust))
                {
                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.ZephyraGust,
                        "Zephyra's Gust",
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