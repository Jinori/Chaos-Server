
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
        source.Inventory.RemoveQuantityByTemplateKey(templatekey, 1);
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
        source.Inventory.RemoveQuantityByTemplateKey(templatekey, 1);
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
        source.Inventory.RemoveQuantityByTemplateKey(templatekey, 1);
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
        source.Inventory.RemoveQuantityByTemplateKey(templatekey, 1);
    }

    public static void JewelcraftingRecipeLearn(
        Aisling source,
        Animation ani,
        JewelcraftingCategories recipe,
        string serverMessage,
        string templatekey
    )
    {
        source.Animate(ani);
        source.Trackers.Flags.AddFlag(recipe);
        source.SendOrangeBarMessage($"You've learned {serverMessage}.");
        source.Inventory.RemoveQuantityByTemplateKey(templatekey, 1);
    }

    public override void OnUse(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out Crafts craft);

        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 21
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region All Mounts
            case "recipe_allmounts":
            {
                source.Trackers.Flags.AddFlag(AvailableMounts.Ant);
                source.Trackers.Flags.AddFlag(AvailableMounts.Bee);
                source.Trackers.Flags.AddFlag(AvailableMounts.Dunan);
                source.Trackers.Flags.AddFlag(AvailableMounts.Wolf);
                source.Trackers.Flags.AddFlag(AvailableMounts.Horse);
                source.Trackers.Flags.AddFlag(AvailableMounts.Kelberoth);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Blue);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Black);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Red);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Green);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Purple);
                source.Trackers.Flags.AddFlag(LanternSizes.SmallLantern);
                source.Trackers.Flags.AddFlag(LanternSizes.LargeLantern);

                source.Animate(ani);
                source.SendOrangeBarMessage("You now have all mounts and cloaks.");
                source.Inventory.RemoveQuantityByTemplateKey("recipe_allmounts", 1);

                return;
            }
            #endregion
            
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
                source.Trackers.Flags.AddFlag(CookingRecipes.Popsicle);
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
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherBerylGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeBerylGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronBerylGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilBerylGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylEmeraldGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylRubyGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylHeartstoneGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylSapphireGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylBerylGauntlet);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireBronzeBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaBronzeBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthBronzeBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindBronzeBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkBronzeBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightBronzeBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireIronBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaIronBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthIronBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindIronBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkIronBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightIronBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireMythrilBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaMythrilBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthMythrilBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindMythrilBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkMythrilBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightMythrilBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireHybrasylBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaHybrasylBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthHybrasylBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindHybrasylBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkHybrasylBraidBelt);
                source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightHybrasylBraidBelt);
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
                source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Longsword);
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
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarDominance);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithTestimony);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisBrace);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonCommitment);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SerendaelSuccess);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneSorrow);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.SkandaraOffense);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.ZephyraPower);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarDeficit);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithBarrier);
                source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisRoots);
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
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneEarthNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneFireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneSeaNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneWindNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaEarthNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaFireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaSeaNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaWindNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedEarthNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedFireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedSeaNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedWindNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarEarthNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarFireNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarSeaNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarWindNecklace);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicBerylEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicRubyEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicSapphireEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicEmeraldEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicHeartstoneEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldEarrings);
                source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneEarrings);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHealthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallManaPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallRejuvenationPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHasteBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallPowerBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallAccuracyPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.JuggernautBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.AstralBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.AntidotePotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallFirestormTonic);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallStunTonic);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.HealthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.ManaPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.RejuvenationPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.HasteBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PowerBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.Hemloch);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.AccuracyPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.RevivePotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongJuggernautBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongAstralBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.CleansingBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.FirestormTonic);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StunTonic);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.WarmthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.AmnesiaBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongHealthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongManaPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongHasteBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongPowerBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongAccuracyPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StatBoostElixir);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.KnowledgeElixir);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentHealthPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentManaPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentRejuvenationPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentHasteBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentPowerBrew);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentAccuracyPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.InvisibilityPotion);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PoisonImmunityElixir);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotionOfStrength);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotionOfConstitution);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotionOfDexterity);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotionOfIntellect);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.PotionOfWisdom);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongStatBoostElixir);
                source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongKnowledgeElixir);
                source.Trackers.Flags.AddFlag(CookingRecipes.Popsicle);

                source.Animate(ani);
                source.SendOrangeBarMessage("You've learned all recipes.");
                source.Inventory.RemoveQuantityByTemplateKey("recipe_allcrafts", 1);

                return;
            }

            #endregion

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
            case "recipe_popsicle":
            {
                if (!source.Trackers.Flags.HasFlag(CookingRecipes.Popsicle))
                {
                    CookingRecipeLearn(
                        source,
                        ani,
                        CookingRecipes.Popsicle,
                        "Popsicle",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion

            #region Armor Smithing Recipes
            case "recipe_beginnerarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BeginnerArmors))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.BeginnerArmors,
                        "Beginner Armors",
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
            
            case "recipe_basicarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicArmors))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.BasicArmors,
                        "Basic Armors",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedJourneyman);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedGaluchatCoat);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCulotte);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBenusta);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedMysticGown);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLotusBodice);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedDwarvishLeather);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedBrigandine);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedLorica);
                    source.Trackers.Flags.AddFlag(CraftedArmors.RefinedCuirass);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe");

                return;
            }

            case "recipe_initiatearmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.InitiateArmors))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.InitiateArmors,
                        "Initiate Armors",
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

                source.SendOrangeBarMessage("You already know this recipe");

                return;
            }
            case "recipe_artisanarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.ArtisanArmors))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.ArtisanArmors,
                        "Artisan Armors",
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
            case "recipe_adeptarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptArmors))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.AdeptArmors,
                        "Adept Armors",
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
            case "recipe_advancedarmors":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdvancedArmors))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("Not implemented yet.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.AdvancedArmors,
                        "Advanced Armors",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicGauntlets))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.BasicGauntlets,
                        "Basic Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherSapphireGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherBerylGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeSapphireGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.BronzeBerylGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_initiategauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.InitiateGauntlets))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.InitiateGauntlets,
                        "Initiate Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronSapphireGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronBerylGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisangauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.ArtisanGauntlets))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.ArtisanGauntlets,
                        "Artisan Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilSapphireGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilBerylGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_adeptgauntlets":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptGauntlets))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.AdeptGauntlets,
                        "Adept Gauntlets",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylEmeraldGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylRubyGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylHeartstoneGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylSapphireGauntlet);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylBerylGauntlet);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_basicbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicBelts))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.BasicBelts,
                        "Basic Belts",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireBronzeBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaBronzeBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthBronzeBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindBronzeBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkBronzeBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightBronzeBelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_initiatebelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.InitiateBelts))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.InitiateBelts,
                        "Initiate Belts",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireIronBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaIronBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthIronBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindIronBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkIronBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightIronBelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisanbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.ArtisanBelts))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.ArtisanBelts,
                        "Artisan Belts",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireMythrilBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaMythrilBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthMythrilBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindMythrilBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkMythrilBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightMythrilBraidBelt);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_adeptbelts":
            {
                if (!source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptBelts))
                {
                    if (craft != Crafts.Armorsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Armorsmith to learn this recipe.");

                        return;
                    }

                    ArmorSmithRecipeLearn(
                        source,
                        ani,
                        ArmorSmithCategories.AdeptBelts,
                        "Adept Belts",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireHybrasylBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaHybrasylBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthHybrasylBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindHybrasylBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkHybrasylBraidBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightHybrasylBraidBelt);

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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
            case "recipe_initiateswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateSwords))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.InitiateSwords,
                        "Initiate Swords",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bramble);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Longsword);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisanswords":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanSwords))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ArtisanSwords,
                        "Artisan Swords",
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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
            case "recipe_initiateweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateWeapons))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.InitiateWeapons,
                        "Initiate Weapons",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Club);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SpikedClub);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.ChainMace);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisanweapons":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanWeapons))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ArtisanWeapons,
                        "Artisan Weapons",
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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
            case "recipe_initiatestaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateStaves))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.InitiateStaves,
                        "Initiate Staves",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusZeus);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyKronos);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisanstaves":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanStaves))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ArtisanStaves,
                        "Artisan Staves",
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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
            case "recipe_initiatedaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateDaggers))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.InitiateDaggers,
                        "Initiate Daggers",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightDagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SunDagger);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisandaggers":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanDaggers))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ArtisanDaggers,
                        "Artisan Daggers",
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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
            case "recipe_initiateclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateClaws))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.InitiateClaws,
                        "Initiate Claws",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EagleTalon);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StoneFist);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_artisanclaws":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanClaws))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ArtisanClaws,
                        "Artisan Claws",
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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

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
            case "recipe_initiateshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateShields))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.InitiateShields,
                        "Initiate Shields",
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
            case "recipe_artisanshields":
            {
                if (!source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanShields))
                {
                    if (craft != Crafts.Weaponsmithing)
                    {
                        source.SendOrangeBarMessage("You must be an Weaponsmith to learn this recipe.");

                        return;
                    }

                    WeaponSmithRecipeLearn(
                        source,
                        ani,
                        WeaponSmithingCategories.ArtisanShields,
                        "Artisan Shields",
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
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

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

            case "recipe_amnesiabrew":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.AmnesiaBrew))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.AmnesiaBrew,
                        "Amnesia Brew",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_attacktonics":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyCategories.AttackTonics))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    source.Inventory.RemoveQuantityByTemplateKey(Subject.Template.TemplateKey, 1);
                    source.Trackers.Flags.AddFlag(AlchemyCategories.AttackTonics);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.FirestormTonic);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.StunTonic);

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_initiatealchemybook":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyCategories.InitiateAlchemyBook))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    source.Inventory.RemoveQuantityByTemplateKey(Subject.Template.TemplateKey, 1);
                    source.Trackers.Flags.AddFlag(AlchemyCategories.InitiateAlchemyBook);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.HealthPotion);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.ManaPotion);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.RejuvenationPotion);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.HasteBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.PowerBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.AccuracyPotion);
                    source.SendOrangeBarMessage("You have learned the Initiate Alchemy Book.");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_basicalchemybook":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyCategories.BasicAlchemyBook))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    source.Inventory.RemoveQuantityByTemplateKey(Subject.Template.TemplateKey, 1);
                    source.Trackers.Flags.AddFlag(AlchemyCategories.BasicAlchemyBook);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.JuggernautBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.AstralBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.AntidotePotion);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallFirestormTonic);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallStunTonic);
                    source.SendOrangeBarMessage("You have learned the Basic Alchemy Book.");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_cleansingbrew":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.CleansingBrew))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.CleansingBrew,
                        "Cleansing Brew",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_knowledgeelixir":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.KnowledgeElixir))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.KnowledgeElixir,
                        "Knowledge Elixir",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_statboostelixir":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StatBoostElixir))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StatBoostElixir,
                        "Stat Boost Elixir",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_strongaccuracypotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StrongAccuracyPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StrongAccuracyPotion,
                        "Strong Accuracy Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_stronghastebrew":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StrongHasteBrew))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StrongHasteBrew,
                        "Strong Haste Brew",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_stronghealthpotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StrongHealthPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StrongHealthPotion,
                        "Strong Health Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_strongmanapotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StrongManaPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StrongManaPotion,
                        "Strong Mana Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_strongrejuvenationpotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StrongRejuvenationPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StrongRejuvenationPotion,
                        "Strong Rejuvenation Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_strongpowerbrew":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.StrongPowerBrew))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.StrongPowerBrew,
                        "Strong Power Brew",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_warmthpotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.WarmthPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.WarmthPotion,
                        "Warmth Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_revivepotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.RevivePotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.RevivePotion,
                        "Revive Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }

            case "recipe_strongvitalitybook":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyCategories.StrongVitalityBrew))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    source.Inventory.RemoveQuantityByTemplateKey(Subject.Template.TemplateKey, 1);
                    source.Trackers.Flags.AddFlag(AlchemyCategories.StrongVitalityBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongJuggernautBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongAstralBrew);

                    return;
                }
                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_potenthealthpotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.PotentHealthPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.PotentHealthPotion,
                        "Potent Health Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_potentmanapotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.PotentManaPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.PotentManaPotion,
                        "Potent Mana Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_potentrejuvenationpotion":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.PotentRejuvenationPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.PotentRejuvenationPotion,
                        "Potent Rejuvenation Potion",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_potenthastebrew":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.PotentHasteBrew))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.PotentHasteBrew,
                        "Potent Haste Brew",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_potentpowerbrew":
            {
                if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.PotentAccuracyPotion))
                {
                    if (craft != Crafts.Alchemy)
                    {
                        source.SendOrangeBarMessage("You must be an Alchemist to learn this recipe.");

                        return;
                    }

                    AlchemyRecipeLearn(
                        source,
                        ani,
                        AlchemyRecipes.PotentHealthPotion,
                        "Potent Health Potion",
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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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

            case "recipe_miraelisblessing":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisBlessing))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

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
            
            case "recipe_ignatardominance":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarDominance))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarDominance,
                        "Ignatar's Dominance",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_geolithtestimony":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithTestimony))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithTestimony,
                        "Geolith's Testimony",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_miraelisbrace":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisBrace))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisBrace,
                        "Miraelis' Brace",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_aquaedoncommitment":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.AquaedonCommitment))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.AquaedonCommitment,
                        "Aquaedon's Commitment",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_serendaelsuccess":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SerendaelSuccess))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SerendaelSuccess,
                        "Serendael's Success",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_theselenesorrow":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.TheseleneSorrow))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.TheseleneSorrow,
                        "Theselene's Sorrow",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_skandaraoffense":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.SkandaraOffense))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.SkandaraOffense,
                        "Skandara's Offense",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_zephyrapower":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.ZephyraPower))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.ZephyraPower,
                        "Zephyra's Power",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_ignatardeficit":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.IgnatarDeficit))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.IgnatarDeficit,
                        "Ignatar's Deficit",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            
            case "recipe_geolithbarrier":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.GeolithBarrier))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.GeolithBarrier,
                        "Geolith's Barrier",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            case "recipe_miraelisroots":
            {
                if (!source.Trackers.Flags.HasFlag(EnchantingRecipes.MiraelisRoots))
                {
                    if (craft != Crafts.Enchanting)
                    {
                        source.SendOrangeBarMessage("You must be an Enchanter to learn this recipe.");

                        return;
                    }

                    EnchantingRecipeLearn(
                        source,
                        ani,
                        EnchantingRecipes.MiraelisRoots,
                        "Miraelis' Roots",
                        $"{Subject.Template.TemplateKey}");

                    return;
                }

                source.SendOrangeBarMessage("You already know this recipe.");

                return;
            }
            #endregion

            #region Jewelcrafting Recipes
            case "recipe_basicrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.BasicRings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.BasicRings,
                        "Basic Rings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeRubyRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeSapphireRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeHeartstoneRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeEmeraldRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeBerylRing);
                }

                break;
            }
            case "recipe_initiaterings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.InitiateRings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.InitiateRings,
                        "Initiate Rings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylRing);
                }

                break;
            }
            case "recipe_artisanrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.ArtisanRings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.ArtisanRings,
                        "Artisan Rings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylRing);
                }

                break;
            }
            case "recipe_adeptrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.AdeptRings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.AdeptRings,
                        "Adept Rings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylRing);
                }

                break;
            }

            case "recipe_basicearrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.BasicEarrings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.BasicEarrings,
                        "Basic Earrings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicBerylEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicEmeraldEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicHeartstoneEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicSapphireEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicRubyEarrings);
                }

                break;
            }

            case "recipe_initiateearrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.InitiateEarrings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.InitiateEarrings,
                        "Initiate Earrings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireEarrings);
                }

                break;
            }
            case "recipe_artisanearrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.ArtisanEarrings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.ArtisanEarrings,
                        "Artisan Earrings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireEarrings);
                }

                break;
            }
            case "recipe_adeptearrings":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.AdeptEarrings))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.AdeptEarrings,
                        "Adept Earrings",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyEarrings);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireEarrings);
                }

                break;
            }

            case "recipe_basicnecklaces":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.BasicNecklaces))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.BasicNecklaces,
                        "Basic Necklaces",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneEarthNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneFireNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneSeaNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneWindNecklace);
                }

                break;
            }

            case "recipe_initiatenecklaces":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.InitiateNecklaces))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.InitiateNecklaces,
                        "Initiate Necklaces",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaEarthNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaFireNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaSeaNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaWindNecklace);
                }

                break;
            }
            case "recipe_artisannecklaces":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.ArtisanNecklaces))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.ArtisanNecklaces,
                        "Artisan Necklaces",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedWindNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedSeaNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedFireNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedEarthNecklace);
                }

                break;
            }
            case "recipe_adeptnecklaces":
            {
                if (!source.Trackers.Flags.HasFlag(JewelcraftingCategories.AdeptNecklaces))
                {
                    if (craft != Crafts.Jewelcrafting)
                    {
                        source.SendOrangeBarMessage("You must be a Jeweler to learn this recipe.");

                        return;
                    }

                    JewelcraftingRecipeLearn(
                        source,
                        ani,
                        JewelcraftingCategories.AdeptNecklaces,
                        "Adept Necklaces",
                        $"{Subject.Template.TemplateKey}");

                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarEarthNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarFireNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarSeaNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarWindNecklace);
                }

                break;
            }
            #endregion
        }
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
        source.Inventory.RemoveQuantityByTemplateKey(templatekey, 1);
    }
}