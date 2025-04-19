using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class RelearnCraftScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public RelearnCraftScript(
        Dialog subject,
        ISpellFactory spellFactory,
        ISkillFactory skillFactory,
        IItemFactory itemFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
        ItemFactory = itemFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "riona_relearnrecipes":
            {
                if (!source.Bank.Contains("Mount")
                    && !source.Inventory.Contains("Mount")
                    && source.Trackers.Enums.HasValue(RionaTutorialQuestStage.CompletedTutorialQuest))
                {
                    var mount = ItemFactory.Create("Mount");
                    source.GiveItemOrSendToBank(mount);
                    source.SendOrangeBarMessage("Riona hands you replacement mount.");
                }

                if (source.Trackers.Flags.HasFlag(MainstoryFlags.CreantRewards))
                {
                    source.Trackers.Flags.RemoveFlag(CreantEnums.CompletedMedusa);
                    source.Trackers.Flags.RemoveFlag(CreantEnums.CompletedSham);
                    source.Trackers.Flags.RemoveFlag(CreantEnums.CompletedTauren);
                    source.Trackers.Flags.RemoveFlag(CreantEnums.CompletedPhoenix);
                }

                if ((source.Name == "Star") && !source.Legend.ContainsKey("frostyrunnerup"))
                    source.Legend.AddUnique(
                        new LegendMark(
                            "Frosty Challenge Runner-Up: So Close So Cool!",
                            "frostyrunnerup",
                            MarkIcon.Heart,
                            MarkColor.Cyan,
                            1,
                            GameTime.Now));

                if ((source.Name == "Dood") && !source.Legend.ContainsKey("frostyfirstplace"))
                    source.Legend.AddUnique(
                        new LegendMark(
                            "Frosty Challenge First Place: The best of the best!",
                            "frostyfirstplace",
                            MarkIcon.Heart,
                            MarkColor.Cyan,
                            1,
                            GameTime.Now));

                if ((source.Name == "Path") && !source.Legend.ContainsKey("frostythirdplace"))
                    source.Legend.AddUnique(
                        new LegendMark(
                            "Frosty Challenge Third Place: Just made it!",
                            "frostythirdplace",
                            MarkIcon.Heart,
                            MarkColor.Cyan,
                            1,
                            GameTime.Now));

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.CompletedCreants)
                    && !source.Trackers.Flags.HasFlag(MainstoryFlags.CreantRewards))
                {
                    source.Trackers.Enums.Set(ClassStatBracket.Grandmaster);
                    source.Trackers.Flags.AddFlag(MainstoryFlags.CreantRewards);
                    source.Trackers.Flags.RemoveFlag(MainstoryFlags.ReceivedRewards);
                    ExperienceDistributionScript.GiveExp(source, 100000000);
                    source.TryGiveGamePoints(25);
                    var godsstar = ItemFactory.Create("godsstar");
                    source.GiveItemOrSendToBank(godsstar);
                    source.SendOrangeBarMessage("Riona hands you a Star from the gods.");
                }

                if (source.Legend.ContainsKey("darkpriest") && !source.SpellBook.ContainsByTemplateKey("auraoftorment"))
                {
                    var auraoftorment = SpellFactory.Create("auraoftorment");
                    source.SpellBook.TryAddToNextSlot(auraoftorment);
                }
                
                if (source.Legend.ContainsKey("dedicated") && source.Legend.ContainsKey("rogueClass"))
                {
                    if (!source.SkillBook.ContainsByTemplateKey("peek"))
                    {
                        var peek = SkillFactory.Create("peek");
                        source.SkillBook.TryAddToNextSlot(PageType.Page3, peek);
                    }

                    if (!source.SkillBook.ContainsByTemplateKey("sense"))
                    {
                        var sense = SkillFactory.Create("sense");
                        source.SkillBook.TryAddToNextSlot(PageType.Page3, sense);
                    }

                    if (!source.SkillBook.ContainsByTemplateKey("trip"))
                    {
                        var trip = SkillFactory.Create("trip");
                        source.SkillBook.TryAddToNextSlot(trip);
                    }
                }

                if (source.SpellBook.ContainsByTemplateKey("morcradh")
                    && !source.SpellBook.ContainsByTemplateKey("ardcradh")
                    && (source.UserStatSheet.BaseClass != BaseClass.Wizard))
                {
                    var ardcradh = SpellFactory.Create("ardcradh");
                    source.SpellBook.TryAddToNextSlot(ardcradh);
                }

                if (!source.SpellBook.ContainsByTemplateKey("phoenixgrasp")
                    && source.UserStatSheet is { BaseClass: BaseClass.Monk, Master: true })
                {
                    var phoenixgrasp = SpellFactory.Create("phoenixgrasp");
                    source.SpellBook.TryAddToNextSlot(phoenixgrasp);
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact1)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedAssemble)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedArtifactsHunt)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedFirstTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedFirstTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedSecondTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedSecondTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedThirdTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedThirdTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedFourthTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedFourthTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedTrials)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.RetryServant)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedServant)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.KilledSummoner)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
                    source.Trackers.Flags.AddFlag(MainstoryFlags.AccessGodsRealm);

                if (source.Trackers.Counters.TryGetValue("CryptSlayerLegend", out var legend) && (legend >= 10))
                {
                    source.Trackers.Flags.AddFlag(LanternSizes.None);
                    source.Trackers.Flags.AddFlag(LanternSizes.LargeLantern);
                    source.Trackers.Flags.AddFlag(LanternSizes.SmallLantern);
                } else if (legend is < 10 and > 0)
                    source.Trackers.Flags.AddFlag(LanternSizes.SmallLantern);

                if (source.Trackers.Enums.HasValue(Crafts.Weaponsmithing))
                {
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Eppe);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Saber);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DullClaw);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SnowDagger);
                    source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CenterDagger);

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicSwords))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidheamh);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BroadSword);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BattleSword);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Masquerade);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateSwords))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Bramble);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Longsword);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanSwords))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Claidhmore);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EmeraldSword);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Gladius);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptSwords))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Kindjal);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.DragonSlayer);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicWeapons))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Hatchet);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Harpoon);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Scimitar);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateWeapons))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Club);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SpikedClub);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.ChainMace);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanWeapons))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HandAxe);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Cutlass);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptWeapons))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.TalgoniteAxe);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HybrasylBattleAxe);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicStaves))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusAres);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyHermes);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateStaves))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusZeus);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyKronos);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanStaves))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MagusDiana);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HolyDiana);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptStaves))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StoneCross);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.OakStaff);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StaffOfWisdom);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicDaggers))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BlossomDagger);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.CurvedDagger);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MoonDagger);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateDaggers))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightDagger);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.SunDagger);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanDaggers))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LotusDagger);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BloodDagger);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptDaggers))
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.NagetierDagger);

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicClaws))
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.WolfClaw);

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateClaws))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.EagleTalon);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.StoneFist);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanClaws))
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.PhoenixClaw);

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.AdeptClaws))
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.Nunchaku);

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.BasicShields))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LeatherShield);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.BronzeShield);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.InitiateShields))
                    {
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.IronShield);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.GravelShield);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.LightShield);
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.MythrilShield);
                    }

                    if (source.Trackers.Flags.HasFlag(WeaponSmithingCategories.ArtisanShields))
                        source.Trackers.Flags.AddFlag(WeaponSmithingRecipes.HybrasylShield);
                    source.SendOrangeBarMessage("You have learned all your Weapon Smithing recipes.");
                }

                if (source.Trackers.Enums.HasValue(Crafts.Armorsmithing))
                {
                    source.Trackers.Flags.AddFlag(ArmorSmithCategories.BeginnerArmors);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaBelt);
                    source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LeatherGauntlet);
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

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicArmors))
                    {
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
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.InitiateArmors))
                    {
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
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.ArtisanArmors))
                    {
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
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptArmors))
                    {
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
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicGauntlets))
                    {
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
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.InitiateGauntlets))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronEmeraldGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronRubyGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronHeartstoneGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronSapphireGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.IronBerylGauntlet);
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.ArtisanGauntlets))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilEmeraldGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilRubyGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilHeartstoneGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilSapphireGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.MythrilBerylGauntlet);
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptGauntlets))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylEmeraldGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylRubyGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylHeartstoneGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylSapphireGauntlet);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.HybrasylBerylGauntlet);
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.BasicBelts))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireBronzeBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaBronzeBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthBronzeBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindBronzeBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkBronzeBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightBronzeBelt);
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.InitiateBelts))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireIronBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaIronBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthIronBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindIronBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkIronBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightIronBelt);
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.ArtisanBelts))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireMythrilBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaMythrilBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthMythrilBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindMythrilBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkMythrilBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightMythrilBraidBelt);
                    }

                    if (source.Trackers.Flags.HasFlag(ArmorSmithCategories.AdeptBelts))
                    {
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.FireHybrasylBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.SeaHybrasylBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.EarthHybrasylBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.WindHybrasylBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.DarkHybrasylBraidBelt);
                        source.Trackers.Flags.AddFlag(ArmorsmithingRecipes.LightHybrasylBraidBelt);
                    }

                    source.SendOrangeBarMessage("You have learned all your Armor Smithing recipes.");
                }

                if (source.Trackers.Enums.HasValue(Crafts.Enchanting))
                {
                    source.Trackers.Flags.AddFlag(EnchantingRecipes.GeolithGratitude);
                    source.Trackers.Flags.AddFlag(EnchantingRecipes.IgnatarEnvy);
                    source.Trackers.Flags.AddFlag(EnchantingRecipes.MiraelisSerenity);
                    source.Trackers.Flags.AddFlag(EnchantingRecipes.TheseleneElusion);
                    source.Trackers.Flags.AddFlag(EnchantingRecipes.AquaedonClarity);

                    source.SendOrangeBarMessage("You have learned all your Enchanting recipes.");
                }

                if (source.Trackers.Enums.HasValue(Crafts.Alchemy))
                {
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHealthPotion);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallManaPotion);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallHasteBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallPowerBrew);
                    source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallAccuracyPotion);

                    if (source.Trackers.Flags.HasFlag(AlchemyCategories.AttackTonics))
                    {
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.FirestormTonic);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.StunTonic);
                    }

                    if (source.Trackers.Flags.HasFlag(AlchemyCategories.BasicAlchemyBook))
                    {
                        source.Trackers.Flags.AddFlag(AlchemyCategories.BasicAlchemyBook);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.JuggernautBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.AstralBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.AntidotePotion);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallFirestormTonic);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.SmallStunTonic);
                    }

                    if (source.Trackers.Flags.HasFlag(AlchemyCategories.InitiateAlchemyBook))
                    {
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.HealthPotion);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.ManaPotion);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.RejuvenationPotion);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.HasteBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.PowerBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.AccuracyPotion);
                    }

                    if (source.Trackers.Flags.HasFlag(AlchemyCategories.StrongVitalityBrew))
                    {
                        source.Trackers.Flags.AddFlag(AlchemyCategories.StrongVitalityBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongJuggernautBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.StrongAstralBrew);
                    }

                    if (source.Trackers.Flags.HasFlag(AlchemyCategories.PotentVitalityBrew))
                    {
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentJuggernautBrew);
                        source.Trackers.Flags.AddFlag(AlchemyRecipes.PotentAstralBrew);
                    }

                    source.SendOrangeBarMessage("You have learned all your Alchemy Recipes.");
                }

                if (source.Trackers.Enums.HasValue(Crafts.Jewelcrafting))
                {
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.SmallRubyRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BerylRing);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.EarthNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.SeaNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.FireNecklace);
                    source.Trackers.Flags.AddFlag(JewelcraftingRecipes.WindNecklace);

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.BasicRings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeRubyRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeSapphireRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeHeartstoneRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeEmeraldRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BronzeBerylRing);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.InitiateRings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylRing);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.ArtisanRings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylRing);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.AdeptRings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldRing);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylRing);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.BasicEarrings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicBerylEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicEmeraldEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicHeartstoneEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicSapphireEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BasicRubyEarrings);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.InitiateEarrings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronBerylEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronEmeraldEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronHeartstoneEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronRubyEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.IronSapphireEarrings);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.ArtisanEarrings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilBerylEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilEmeraldEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilHeartstoneEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilRubyEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.MythrilSapphireEarrings);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.AdeptEarrings))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylBerylEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylEmeraldEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylHeartstoneEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylRubyEarrings);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.HybrasylSapphireEarrings);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.BasicNecklaces))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneEarthNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneFireNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneSeaNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.BoneWindNecklace);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.InitiateNecklaces))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaEarthNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaFireNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaSeaNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.KannaWindNecklace);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.ArtisanNecklaces))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedWindNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedSeaNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedFireNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.PolishedEarthNecklace);
                    }

                    if (source.Trackers.Flags.HasFlag(JewelcraftingCategories.AdeptNecklaces))
                    {
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarEarthNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarFireNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarSeaNecklace);
                        source.Trackers.Flags.AddFlag(JewelcraftingRecipes.StarWindNecklace);
                    }

                    source.SendOrangeBarMessage("You have learned all your Jewelcrafting Recipes.");
                }

                break;
            }
        }
    }
}