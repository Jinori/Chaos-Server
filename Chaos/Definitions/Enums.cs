namespace Chaos.Definitions;

public enum AoeShape
{
    None,
    Front,
    AllAround,
    FrontalCone,
    FrontalDiamond,
    Cleave
}

[Flags]
public enum Status : ulong
{
    None = 0,
    Dead = 1,
    Suain = 1 << 2,
    AsgallFaileas = 1 << 3,
    PreventAffliction = 1 << 4,
    ClawFist = 1 << 5,
    BeagSuain = 1 << 6,
    BattleCry = 1 << 7,
    InnerFire = 1 << 8,
    Rumination = 1 << 9,
    ChiBlocker = 1 << 10,
    Pramh = 1 << 11,
    DetectTraps = 1 << 12,
    Blind = 1 << 13,
    EarthenStance = 1 << 14,
    MistStance = 1 << 15,
    ThunderStance = 1 << 16,
    SmokeStance = 1 << 17,
    BeagAite = 1 << 18,
    Aite = 1 << 19,
    MorAite = 1 << 20,
    ArdAite = 1 << 21,
    Hide = 1 << 22,
    LastStand = 1 << 23,
    Amnesia = 1 << 24,
    PreventRecradh = 1 << 25,
    WolfFangFist = 1 << 26
    //add more statuses here
}

public enum GainExp
{
    Yes,
    No = 1
}

public enum GodMode
{
    No,
    Yes
}

public enum MonkElementForm
{
    Water,
    Earth = 1,
    Air = 2,
    Fire = 3,
}

public enum ReligionPrayer
{
    None,
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Fifth = 5,
    End = 6
}

[Flags]
public enum TargetFilter : ulong
{
    None,
    FriendlyOnly = 1,
    HostileOnly = 1 << 1,
    NeutralOnly = 1 << 2,
    NonFriendlyOnly = 1 << 3,
    NonHostileOnly = 1 << 4,
    NonNeutralOnly = 1 << 5,
    AliveOnly = 1 << 6,
    DeadOnly = 1 << 7,
    AislingsOnly = 1 << 8,
    MonstersOnly = 1 << 9,
    MerchantsOnly = 1 << 10,
    NonAislingsOnly = 1 << 11,
    NonMonstersOnly = 1 << 12,
    NonMerchantsOnly = 1 << 13,
    SelfOnly = 1 << 14,
    OthersOnly = 1 << 15,
    GroupOnly = 1 << 16
}

public enum VisibilityType
{
    Normal,
    Hidden,
    TrueHidden,
    GmHidden
}

public enum Crafts
{
    None = 0,
    Armorsmithing = 1,
    Enchanting = 2,
    Alchemy = 3,
    Weaponsmithing = 4,
    Jewelcrafting = 5
}

[Flags]
public enum Hobbies
{
    None = 0,
    Fishing = 1,
    Cooking = 1 << 2,
}

public enum Religion
{
    None = 0,
    Cail = 1,
    Ceannlaidir = 2,
    Deoch = 3,
    Fiosachd = 4,
    Glioca = 5,
    Gramail = 6,
    Luathas = 7,
    Sgrios = 8
}

public enum ReligionRanks
{
    None = 0,
    Probate = 1,
    Worshipper = 2,
    Acolyte = 3,
    Priest = 4,
    Cleric = 5,
    Minister = 6,
    HighPriest = 7
}

public enum TutorialQuestStage
{
    None = 0,
    GaveArmor = 1,
    GaveAssailAndSpell = 2,
    LearnedWorld = 3,
    GotEquipment = 4,
    StartedFloppy = 5,
    CompletedFloppy = 6,
    GiantFloppy = 7,
    CompletedTutorial = 8
}

public enum ManorNecklaceStage
{
    None = 0,
    AcceptedQuest = 1,
    SawNecklace = 2,
    ReturningNecklace = 3,
    ReturnedNecklace = 4,
    KeptNecklace = 5,
}

public enum ManorLouegieStage
{
    None = 0,
    AcceptedQuest = 1,
    CompletedQuest = 2
}

public enum HolyResearchStage
{
    None = 0,
    StartedRawHoney = 1,
    StartedRawWax = 2,
    StartedRoyalWax = 3
}

public enum DarkThingsStage
{
    None = 0,
    StartedSpidersEye = 1,
    StartedCentipedesGland = 2,
    StartedBatsWing = 3,
    StartedSpidersSilk = 4,
    StartedScorpionSting = 5,
    StartedGiantBatsWing = 6,
    StartedWhiteBatsWing = 7
}

public enum ALittleBitofThatStage
{
    None = 0,
    StartedApple = 1,
    StartedTomato = 2,
    StartedCherry = 3,
    StartedGrapes = 4,
    StartedMold = 5,
    StartedBaguette = 6,
}

public enum RionaTutorialQuestStage
{
    None = 0,
    StartedRatQuest = 1,
    CompletedRatQuest = 2,
    StartedSpareAStick = 3,
    CompletedSpareAStick = 4,
    StartedGetGuided = 5,
    CompletedGetGuided = 6,
    StartedSkarn = 7,
    CompletedSkarn = 8,
    StartedBeautyShop = 9,
    CompletedBeautyShop = 10,
    StartedCrafting = 11,
    CompletedCrafting = 12,
    StartedLeveling = 13,
    CompletedLeveling = 14,
    CompletedTutorialQuest = 15
}

[Flags]
public enum RionaTutorialQuestFlags
{
    None = 0,
    SpareAStick = 1,
    Skarn = 1 << 1,
    Crafting = 1 << 2,
}

public enum PFQuestStage
{
    None = 0,
    StartedPFQuest = 1,
    TurnedInRoots = 2,
    WolfManes = 3,
    WolfManesTurnedIn = 4,
    KilledGiantMantis = 6,
    CompletedPFQuest = 7,
    TurnedInTristar = 8,
}

public enum CryptSlayerStage
{
    None = 0,
    Bat = 1,
    Centipede1 = 2,
    Centipede2 = 3,
    GiantBat = 4,
    Kardi = 5,
    Marauder = 6,
    Mimic = 7,
    Rat = 8,
    Scorpion = 9,
    Spider1 = 10,
    Spider2 = 11,
    Succubus = 12,
    WhiteBat = 13,
    Completed = 14
}

#region Mythic Quests
public enum MythicQuestMain
{
    None = 0,
    MythicStarted = 1,
    CompletedAll = 2,
    CompletedMythic = 3
}

public enum MythicBunny
{
    None = 0,
    LowerBunny = 1,
    LowerBunnyComplete = 2,
    HigherBunny = 3,
    HigherBunnyComplete = 4,
    ItemBunny = 5,
    ItemBunnyComplete = 6,
    AlliedBunny = 7,
    EnemyBunnyAllied = 8,
    BossBunnyStarted = 9,
    BossBunnyDefeated = 10,
}

public enum MythicHorse
{
    None = 0,
    LowerHorse = 1,
    LowerHorseComplete = 2,
    HigherHorse = 3,
    HigherHorseComplete = 4,
    ItemHorse = 5,
    ItemHorseComplete = 6,
    AlliedHorse = 7,
    EnemyHorseAllied = 8,
    BossHorseStarted = 9,
    BossHorseDefeated = 10,
}

public enum MythicGargoyle
{
    None = 0,
    LowerGargoyle = 1,
    LowerGargoyleComplete = 2,
    HigherGargoyle = 3,
    HigherGargoyleComplete = 4,
    ItemGargoyle = 5,
    ItemGargoyleComplete = 6,
    AlliedGargoyle = 7,
    EnemyGargoyleAllied = 8,
    BossGargoyleStarted = 9,
    BossGargoyleDefeated = 10,
}

public enum MythicZombie
{
    None = 0,
    LowerZombie = 1,
    LowerZombieComplete = 2,
    HigherZombie = 3,
    HigherZombieComplete = 4,
    ItemZombie = 5,
    ItemZombieComplete = 6,
    AlliedZombie = 7,
    EnemyZombieAllied = 8,
    BossZombieStarted = 9,
    BossZombieDefeated = 10,
}

public enum MythicFrog
{
    None = 0,
    LowerFrog = 1,
    LowerFrogComplete = 2,
    HigherFrog = 3,
    HigherFrogComplete = 4,
    ItemFrog = 5,
    ItemFrogComplete = 6,
    AlliedFrog = 7,
    EnemyFrogAllied = 8,
    BossFrogStarted = 9,
    BossFrogDefeated = 10,
}

public enum MythicWolf
{
    None = 0,
    LowerWolf = 1,
    LowerWolfComplete = 2,
    HigherWolf = 3,
    HigherWolfComplete = 4,
    ItemWolf = 5,
    ItemWolfComplete = 6,
    AlliedWolf = 7,
    EnemyWolfAllied = 8,
    BossWolfStarted = 9,
    BossWolfDefeated = 10,
}

public enum MythicMantis
{
    None = 0,
    LowerMantis = 1,
    LowerMantisComplete = 2,
    HigherMantis = 3,
    HigherMantisComplete = 4,
    ItemMantis = 5,
    ItemMantisComplete = 6,
    MantisAllied = 7,
    EnemyAllied = 8,
    BossMantisStarted = 9,
    BossMantisDefeated = 10,
}

public enum MythicBee
{
    None = 0,
    LowerBee = 1,
    LowerBeeComplete = 2,
    HigherBee = 3,
    HigherBeeComplete = 4,
    ItemBee = 5,
    ItemBeeComplete = 6,
    AlliedBee = 7,
    EnemyBeeAllied = 8,
    BossBeeStarted = 9,
    BossBeeDefeated = 10,
}

public enum MythicKobold
{
    None = 0,
    LowerKobold = 1,
    LowerKoboldComplete = 2,
    HigherKobold = 3,
    HigherKoboldComplete = 4,
    ItemKobold = 5,
    ItemKoboldComplete = 6,
    AlliedKobold = 7,
    EnemyKoboldAllied = 8,
    BossKoboldStarted = 9,
    BossKoboldDefeated = 10,
}

public enum MythicGrimlock
{
    None = 0,
    LowerGrimlock = 1,
    LowerGrimlockComplete = 2,
    HigherGrimlock = 3,
    HigherGrimlockComplete = 4,
    ItemGrimlock = 5,
    ItemGrimlockComplete = 6,
    AlliedGrimlock = 7,
    EnemyGrimlockAllied = 8,
    BossGrimlockStarted = 9,
    BossGrimlockDefeated = 10,
}
#endregion

public enum WolfProblemStage
{
    None = 0,
    Start = 1,
    Complete = 2,
}

public enum SickChildStage
{
    None = 0,
    WhiteRose = 1,
    WhiteRose1Turn = 2,
    WhiteRose2 = 3,
    WhiteRose2Turn = 4,
    GoldRose = 5,
    GoldRoseTurn = 6,
    BlackRose = 7,
    BlackRoseTurn = 8,
    SickChildComplete = 9,
    SickChildKilled = 10,
}

public enum CrHorror
{
    None = 0,
    Start = 1,
    Kill = 2,
    Complete = 3
}

public enum QueenOctopusQuest
{
    None = 0,
    Liver = 1,
    Pendant = 2,
    Pendant2 = 3,
    Pendant3 = 4,
    Queen = 5,
    Complete = 6
}

public enum BeggarFoodQuestStage
{
    None = 0,
    Started = 1,
    Completed = 2
}

public enum BeeProblem
{
    None = 0,
    Started = 1,
    Completed = 2
}

public enum IceWallQuest
{
    None = 0,
    Start = 1,
    SampleComplete = 2,
    KillWolves = 3,
    WolfComplete = 4,
    Charm = 5,
    KillBoss = 6,
    Complete = 7
}

#region Cooking Enums
public enum CookFoodStage
{
    None = 0,
    dinnerplate = 1,
    fruitbasket = 2,
    lobsterdinner = 3,
    pie = 4,
    salad = 5,
    sandwich = 6,
    soup = 7,
    steakmeal = 8,
    sweetbuns = 9,
    popsicle = 10
}

[Flags]
public enum CookFoodProgression
{
    None = 0,
    NotenoughIngredients = 1,
}

public enum MeatsStage
{
    None = 0,
    beefslices = 1,
    clam = 2,
    trout = 3,
    bass = 4,
    egg = 5,
    liver = 6,
    lobstertail = 7,
    rawmeat = 8
}

public enum MeatsStage2
{
    None = 0,
    beefslices = 1,
    clam = 2,
    trout = 3,
    bass = 4,
    egg = 5,
    liver = 6,
    lobstertail = 7,
    rawmeat = 8
}

public enum MeatsStage3
{
    None = 0,
    beefslices = 1,
    clam = 2,
    trout = 3,
    bass = 4,
    egg = 5,
    liver = 6,
    lobstertail = 7,
    rawmeat = 8
}

public enum FruitsStage
{
    none = 0,
    acorn = 1,
    apple = 2,
    cherry = 3,
    grape = 4,
    greengrapes = 5,
    strawberry = 6,
    tangerines = 7
}

public enum FruitsStage2
{
    none = 0,
    acorn = 1,
    apple = 2,
    cherry = 3,
    grape = 4,
    greengrapes = 5,
    strawberry = 6,
    tangerines = 7
}

public enum FruitsStage3
{
    None = 0,
    acorn = 1,
    apple = 2,
    cherry = 3,
    grape = 4,
    greengrapes = 5,
    strawberry = 6,
    tangerines = 7,
}

public enum VegetableStage
{
    none = 0,
    carrot = 1,
    rambutan = 2,
    tomato = 3,
    vegetable = 4
}

public enum VegetableStage2
{
    none = 0,
    carrot = 1,
    rambutan = 2,
    tomato = 3,
    vegetable = 4
}

public enum VegetableStage3
{
    none = 0,
    carrot = 1,
    rambutan = 2,
    tomato = 3,
    vegetable = 4
}

public enum ExtraIngredientsStage
{
    none = 0,
    bread = 1,
    cheese = 2,
    flour = 3,
    marinade = 4,
    salt = 5,
    ice = 6,
    sugar = 7
}

public enum ExtraIngredientsStage2
{
    none = 0,
    bread = 1,
    cheese = 2,
    flour = 3,
    marinade = 4,
    salt = 5,
    ice = 6,
    sugar = 7
}

public enum ExtraIngredientsStage3
{
    none = 0,
    bread = 1,
    cheese = 2,
    flour = 3,
    marinade = 4,
    salt = 5,
    ice = 6,
    sugar = 7
}

[Flags]
public enum CookingRecipes
{
    None,
    DinnerPlate = 1,
    FruitBasket = 1 << 1,
    LobsterDinner = 1 << 2,
    Pie = 1 << 3,
    Salad = 1 << 4,
    Sandwich = 1 << 5,
    Soup = 1 << 6,
    SteakMeal = 1 << 7,
    SweetBuns = 1 << 8,
    Popsicle = 1 << 9,
}
#endregion

public enum FiskSecretStage
{
    None = 0,
    Started = 1,
    StartedWaterLilies = 2,
    CollectedWaterLilies = 3,
    Started2 = 4,
    StartedPetunias = 5,
    CollectedPetunias = 6,
    Started3 = 7,
    StartedPinkRose = 8,
    CollectedPinkRose = 9,
    StartedBouquet = 10,
    StartedBouquet1 = 11,
    CollectedBouquet = 12,
    DeliveredBouquet = 13,
    CompletedFiskSecret = 14,
    AngeredThulin = 15,
    AngeredThulinReturn = 16,
    ExposedFisk = 17,
    WontTell = 18,
    WontTell2 = 19,
    MushroomStart = 20,
    Started4 = 21,
    Started5 = 22,
    DeliverBouquet = 23,
    Started6 = 24,
    BrandyTurnin = 25
}

public enum FiskRemakeBouquet
{
    None = 0,
    BouquetWait = 1,
    RemadeBouquet = 2
}

public enum SwampMazeQuest
{
    None = 0,
    Start = 1,
    Complete = 2
}

[Flags]
public enum AvailableMounts
{
    Horse = 1,
    Wolf = 1 << 2,
    Dunan = 1 << 3,
    Kelberoth = 1 << 4,
    Bee = 1 << 5,
    Ant = 1 << 6,
}

[Flags]
public enum AvailableCloaks
{
    Green = 1,
    Red = 1 << 2,
    Blue = 1 << 3,
    Black = 1 << 4,
    Purple = 1 << 5,
}

public enum CurrentMount
{
    Horse = 1,
    Wolf = 2,
    Dunan = 3,
    Kelberoth = 4,
    Bee = 5,
    Ant = 6,
}

public enum CurrentCloak
{
    Green = 1,
    Red = 2,
    Blue = 3,
    Black = 4,
    Purple = 5,
}

#region Armor Smithing
[Flags]
public enum ArmorSmithCategories
{
    None = 0,
    BasicGauntlets = 1,
    InitiateGauntlets = 1 << 2,
    ArtisanGauntlets = 1 << 3,
    AdeptGauntlets = 1 << 4,
    BasicBelts = 1 << 5,
    InitiateBelts = 1 << 6,
    ArtisanBelts = 1 << 7,
    BasicArmors = 1 << 8,
    InitiateArmors = 1 << 9,
    ArtisanArmors = 1 << 10,
    AdeptArmors = 1 << 11,
    AdvancedArmors = 1 << 12,
}

[Flags]
public enum ArmorsmithingRecipes : ulong
{
    None = 0,
    LeatherSapphireGauntlet = 1L,
    LeatherRubyGauntlet = 1L << 2,
    LeatherEmeraldGauntlet = 1L << 3,
    LeatherHeartstoneGauntlet = 1L << 4,
    IronSapphireGauntlet = 1L << 5,
    IronRubyGauntlet = 1L << 6,
    IronEmeraldGauntlet = 1L << 7,
    IronHeartstoneGauntlet = 1L << 8,
    MythrilSapphireGauntlet = 1L << 9,
    MythrilRubyGauntlet = 1L << 10,
    MythrilEmeraldGauntlet = 1L << 11,
    MythrilHeartstoneGauntlet = 1L << 12,
    HybrasylSapphireGauntlet = 1L << 13,
    HybrasylRubyGauntlet = 1L << 14,
    HybrasylEmeraldGauntlet = 1L << 15,
    HybrasylHeartstoneGauntlet = 1L << 16,
    JeweledSeaBelt = 1L << 17,
    JeweledFireBelt = 1L << 18,
    JeweledWindBelt = 1L << 19,
    JeweledEarthBelt = 1L << 20,
    JeweledNatureBelt = 1L << 21,
    JeweledMetalBelt = 1L << 22,
    JeweledDarkBelt = 1L << 23,
    JeweledLightBelt = 1L << 24,
    EarthBelt = 1L << 25,
    SeaBelt = 1L << 26,
    FireBelt = 1L << 27,
    WindBelt = 1L << 28,
    LeatherGauntlet = 1L << 29
}

[Flags]
public enum CraftedArmors : ulong
{
    None = 0,
    RefinedScoutLeather = 1L,
    RefinedDwarvishLeather = 1L << 2,
    RefinedPaluten = 1L << 3,
    RefinedKeaton = 1L << 4,
    RefinedBardocle = 1L << 5,
    RefinedGardcorp = 1L << 6,
    RefinedJourneyman = 1L << 7,
    RefinedLorum = 1L << 8,
    RefinedMane = 1L << 9,
    RefinedDuinUasal = 1L << 10,
    RefinedCowl = 1L << 11,
    RefinedGaluchatCoat = 1L << 12,
    RefinedMantle = 1L << 13,
    RefinedHierophant = 1L << 14,
    RefinedDalmatica = 1L << 15,
    RefinedCotte = 1L << 16,
    RefinedBrigandine = 1L << 17,
    RefinedCorsette = 1L << 18,
    RefinedPebbleRose = 1L << 19,
    RefinedKagum = 1L << 20,
    RefinedMagiSkirt = 1L << 21,
    RefinedBenusta = 1L << 22,
    RefinedStoller = 1L << 23,
    RefinedClymouth = 1L << 24,
    RefinedClamyth = 1L << 25,
    RefinedGorgetGown = 1L << 26,
    RefinedMysticGown = 1L << 27,
    RefinedElle = 1L << 28,
    RefinedDolman = 1L << 29,
    RefinedBansagart = 1L << 30,
    RefinedDobok = 1L << 31,
    RefinedCulotte = 1L << 32,
    RefinedEarthGarb = 1L << 33,
    RefinedWindGarb = 1L << 34,
    RefinedMountainGarb = 1L << 35,
    RefinedLeatherTunic = 1L << 36,
    RefinedLorica = 1L << 37,
    RefinedKasmaniumArmor = 1L << 38,
    RefinedIpletMail = 1L << 39,
    RefinedHybrasylPlate = 1L << 40,
    RefinedEarthBodice = 1L << 41,
    RefinedLotusBodice = 1L << 42,
    RefinedMoonBodice = 1L << 43,
    RefinedLightningGarb = 1L << 44,
    RefinedSeaGarb = 1L << 45,
    RefinedLeatherBliaut = 1L << 46,
    RefinedCuirass = 1L << 47,
    RefinedKasmaniumHauberk = 1L << 48,
    RefinedPhoenixMail = 1L << 49,
    RefinedHybrasylArmor = 1L << 50
}
#endregion

#region Blacksmithing
[Flags]
public enum WeaponSmithingCategories
{
    None = 0,
    BasicSwords = 1,
    InitiateSwords = 1 << 2,
    ArtisanSwords = 1 << 3,
    AdeptSwords = 1 << 4,
    BasicWeapons = 1 << 5,
    InitiateWeapons = 1 << 6,
    ArtisanWeapons = 1 << 7,
    AdeptWeapons = 1 << 8,
    BasicStaves = 1 << 9,
    InitiateStaves = 1 << 10,
    ArtisanStaves = 1 << 11,
    AdeptStaves = 1 << 12,
    BasicDaggers = 1 << 13,
    InitiateDaggers = 1 << 14,
    ArtisanDaggers = 1 << 15,
    AdeptDaggers = 1 << 16,
    BasicClaws = 1 << 17,
    InitiateClaws = 1 << 18,
    ArtisanClaws = 1 << 19,
    AdeptClaws = 1 << 20,
    BasicShields = 1 << 21,
    InitiateShields = 1 << 22,
    ArtisanShields = 1 << 23
}

[Flags]
public enum WeaponSmithingRecipes : ulong
{
    None = 0,
    Eppe = 1L,
    Saber = 1L << 2,
    Claidheamh = 1L << 3,
    BroadSword = 1L << 4,
    BattleSword = 1L << 5,
    Masquerade = 1L << 6,
    Bramble = 1L << 7,
    Longsword = 1L << 8,
    Claidhmore = 1L << 9,
    EmeraldSword = 1L << 10,
    Gladius = 1L << 11,
    Kindjal = 1L << 12,
    DragonSlayer = 1L << 13,
    Hatchet = 1L << 14,
    Harpoon = 1L << 15,
    Scimitar = 1L << 16,
    Club = 1L << 17,
    SpikedClub = 1L << 18,
    ChainMace = 1L << 19,
    HandAxe = 1L << 20,
    Cutlass = 1L << 21,
    TalgoniteAxe = 1L << 22,
    HybrasylBattleAxe = 1L << 23,
    MagusAres = 1L << 24,
    HolyHermes = 1L << 25,
    MagusZeus = 1L << 26,
    HolyKronos = 1L << 27,
    MagusDiana = 1L << 28,
    HolyDiana = 1L << 29,
    StoneCross = 1L << 30,
    OakStaff = 1L << 31,
    StaffOfWisdom = 1L << 32,
    SnowDagger = 1L << 33,
    CenterDagger = 1L << 34,
    BlossomDagger = 1L << 35,
    CurvedDagger = 1L << 36,
    MoonDagger = 1L << 37,
    LightDagger = 1L << 38,
    SunDagger = 1L << 39,
    LotusDagger = 1L << 40,
    BloodDagger = 1L << 41,
    NagetierDagger = 1L << 42,
    DullClaw = 1L << 43,
    WolfClaw = 1L << 44,
    EagleTalon = 1L << 45,
    PhoenixClaw = 1L << 46,
    Nunchaku = 1L << 47,
    WoodenShield = 1L << 48,
    LeatherShield = 1L << 49,
    BronzeShield = 1L << 50,
    GravelShield = 1L << 51,
    IronShield = 1L << 52,
    LightShield = 1L << 53,
    MythrilShield = 1L << 54,
    HybrasylShield = 1L << 55
}
#endregion

[Flags]
public enum EnchantingRecipes : ulong
{
    None = 0,
    IgnatarEnvy = 1L,
    GeolithGratitude = 1L << 2,
    MiraelisSerenity = 1L << 3,
    TheseleneElusion = 1L << 4,
    AquaedonClarity = 1L << 5,
    SerendaelLuck = 1L << 6,
    SkandaraMight = 1L << 7,
    ZephyraSpirit = 1L << 8,
    IgnatarGrief = 1L << 9,
    GeolithPride = 1L << 10,
    MiraelisBlessing = 1L << 11,
    TheseleneShadow = 1L << 12,
    AquaedonCalming = 1L << 13,
    SerendaelMagic = 1L << 14,
    SkandaraTriumph = 1L << 15,
    ZephyraMist = 1L << 16,
    IgnatarRegret = 1L << 17,
    GeolithConstitution = 1L << 18,
    MiraelisIntellect = 1L << 19,
    TheseleneDexterity = 1L << 20,
    AquaedonWisdom = 1L << 21,
    SerendaelChance = 1L << 22,
    SkandaraStrength = 1L << 23,
    ZephyraWind = 1L << 24,
    IgnatarJealousy = 1L << 25,
    GeolithObsession = 1L << 26,
    MiraelisHarmony = 1L << 27,
    TheseleneBalance = 1L << 28,
    AquaedonWill = 1L << 29,
    SerendaelRoll = 1L << 30,
    SkandaraDrive = 1L << 31,
    ZephyraVortex = 1L << 32,
    IgnatarDestruction = 1L << 33,
    GeolithFortitude = 1L << 34,
    MiraelisNurturing = 1L << 35,
    TheseleneRisk = 1L << 36,
    AquaedonResolve = 1L << 37,
    SerendaelAddiction = 1L << 38,
    SkandaraPierce = 1L << 39,
    ZephyraGust = 1L << 40
}

[Flags]
public enum JewelcraftingCategories : ulong
{
    None = 0,
    BasicRings = 1L,
    InitiateRings = 1L << 2,
    ArtisanRings = 1L << 3,
    AdeptRings = 1L << 4,
    BasicNecklaces = 1L << 5,
    InitiateNecklaces = 1L << 6,
    ArtisanNecklaces = 1L << 7,
    AdeptNecklaces = 1L << 8,
    BasicEarrings = 1L << 9,
    InitiateEarrings = 1L << 10,
    ArtisanEarrings = 1L << 11,
    AdeptEarrings = 1L << 12,
}

[Flags]
public enum JewelcraftingRecipes : ulong
{
    None = 0,
    BronzeBerylRing = 1,
    BronzeRubyRing = 1 << 2,
    BronzeSapphireRing = 1 << 3,
    BronzeEmeraldRing = 1 << 4,
    BronzeHeartstoneRing = 1 << 5,
    IronBerylRing = 1 << 6,
    IronRubyRing = 1 << 7,
    IronSapphireRing = 1 << 8,
    IronEmeraldRing = 1 << 9,
    IronHeartstoneRing = 1 << 10,
    MythrilBerylRing = 1 << 11,
    MythrilRubyRing = 1 << 12,
    MythrilSapphireRing = 1 << 13,
    MythrilEmeraldRing = 1 << 14,
    MythrilHeartstoneRing = 1 << 15,
    HybrasylBerylRing = 1 << 16,
    HybrasylRubyRing = 1 << 17,
    HybrasylSapphireRing = 1 << 18,
    HybrasylEmeraldRing = 1 << 19,
    HybrasylHeartstoneRing = 1 << 20,
    BoneWindNecklace = 1 << 21,
    BoneEarthNecklace = 1 << 22,
    BoneFireNecklace = 1 << 23,
    BoneSeaNecklace = 1 << 24,
    KannaEarthNecklace = 1 << 25,
    KannaFireNecklace = 1 << 26,
    KannaSeaNecklace = 1 << 27,
    KannaWindNecklace = 1 << 28,
    PolishedWindNecklace = 1 << 29,
    PolishedFireNecklace = 1 << 30,
    PolishedSeaNecklace = 1L << 31,
    PolishedEarthNecklace = 1L << 32,
    StarEarthNecklace = 1L << 33,
    StarFireNecklace = 1L << 34,
    StarSeaNecklace = 1L << 35,
    StarWindNecklace = 1L << 36,
    BasicBerylEarrings = 1L << 37,
    BasicRubyEarrings = 1L << 38,
    BasicSapphireEarrings = 1L << 39,
    BasicEmeraldEarrings = 1L << 40,
    BasicHeartstoneEarrings = 1L << 41,
    IronBerylEarrings = 1L << 42,
    IronRubyEarrings = 1L << 43,
    IronSapphireEarrings = 1L << 44,
    IronEmeraldEarrings = 1L << 45,
    IronHeartstoneEarrings = 1L << 46,
    MythrilBerylEarrings = 1L << 47,
    MythrilRubyEarrings = 1L << 48,
    MythrilSapphireEarrings = 1L << 49,
    MythrilEmeraldEarrings = 1L << 50,
    MythrilHeartstoneEarrings = 1L << 51,
    HybrasylBerylEarrings = 1L << 52,
    HybrasylRubyEarrings = 1L << 53,
    HybrasylSapphireEarrings = 1L << 54,
    HybrasylEmeraldEarrings = 1L << 55,
    HybrasylHeartstoneEarrings = 1L << 56,
    SmallRubyRing = 1L << 57,
    BerylRing = 1L << 58,
    FireNecklace = 1L << 59,
    SeaNecklace = 1L << 60,
    WindNecklace = 1L << 61,
    EarthNecklace = 1L << 62
}

[Flags]
public enum AlchemyCategories
{
    None = 0,
    BasicAlchemyBook = 1,
    InitiateAlchemyBook = 2,
    AttackTonics = 3,
    StrongVitalityBrew = 4,
}

[Flags]
public enum AlchemyRecipes : ulong
{
    None = 0,
    Hemloch = 1L,
    SmallHealthPotion = 1L << 2,
    SmallManaPotion = 1L << 3,
    SmallRejuvenationPotion = 1L << 4,
    SmallHasteBrew = 1L << 5,
    SmallPowerBrew = 1L << 6,
    SmallAccuracyPotion = 1L << 7,
    JuggernautBrew = 1L << 8,
    AstralBrew = 1L << 9,
    AntidotePotion = 1L << 10,
    SmallFirestormTonic = 1L << 11,
    SmallStunTonic = 1L << 12,
    HealthPotion = 1L << 13,
    ManaPotion = 1L << 14,
    RejuvenationPotion = 1L << 15,
    HasteBrew = 1L << 16,
    PowerBrew = 1L << 17,
    AccuracyPotion = 1L << 18,
    RevivePotion = 1L << 19,
    StrongJuggernautBrew = 1L << 20,
    StrongAstralBrew = 1L << 21,
    CleansingBrew = 1L << 22,
    FirestormTonic = 1L << 23,
    StunTonic = 1L << 24,
    WarmthPotion = 1L << 25,
    AmnesiaBrew = 1L << 26,
    StrongHealthPotion = 1L << 27,
    StrongManaPotion = 1L << 28,
    StrongHasteBrew = 1L << 29,
    StrongPowerBrew = 1L << 30,
    StrongAccuracyPotion = 1L << 31,
    StatBoostElixir = 1L << 32,
    KnowledgeElixir = 1L << 33,
    StrongRejuvenationPotion = 1L << 34,
}

public enum JoinReligionQuest
{
    None = 0,
    MiraelisQuest = 1,
    SkandaraQuest = 2,
    TheseleneQuest = 3,
    SerendaelQuest = 4,
    JoinReligionComplete = 5
}

public enum SummonChosenPet
{
    None = 0,
    Gloop = 1,
    Bunny = 2,
    Faerie = 3,
    Dog = 4,
    Ducklings = 5,
    Cat = 6
}

[Flags]
public enum PetSkillsAvailable
{
    None,
    Level10 = 1,
    Level25 = 1 << 1,
    Level40 = 1 << 2,
    Level55 = 1 << 3,
    Level70 = 1 << 4,
    Level85 = 1 << 5,
    Level99 = 1 << 6,
}

public enum Level10PetSkills
{
    None,
    RabidBite = 1,
    Growl = 1 << 1,
    QuickAttack = 1 << 2
}

public enum Level25PetSkills
{
    None,
    TailSweep = 1,
    Enrage = 1 << 1,
    WindStrike = 1 << 2,
}

[Flags]
public enum PetSkillsChosen
{
    None,
    Level10 = 1,
    Level25 = 1 << 1,
    Level40 = 1 << 2,
    Level55 = 1 << 3,
    Level70 = 1 << 4,
    Level85 = 1 << 5,
    Level99 = 1 << 6,
}

public enum ArenaHost
{
    None,
    Host = 1,
    MasterHost = 2
}

public enum ArenaHostPlaying
{
    None,
    Yes = 1,
    No = 2
}

public enum ArenaTeam
{
    None,
    Blue = 1,
    Green = 2,
    Gold = 3,
    Red = 4
}

public enum PentagramQuestStage
{
    None,
    SignedPact = 1,
    StartedRitual = 2,
    ReceivedClue = 3,
    FoundPentagramPiece = 4,
    EmpoweringPentagramPiece = 5,
    EmpoweredPentagramPiece = 6,
    CreatedPentagram = 7,
    BossSpawning = 8,
    BossSpawned = 9,
    DefeatedBoss = 10,
}

public enum NightmareQuestStage
{
    None,
    Started = 1,
    MetRequirementsToEnter1 = 2,
    MetRequirementsToEnter2 = 3,
    MetRequirementsToEnter3 = 4,
    EnteredDream = 5,
    SpawningNightmare = 6,
    SpawnedNightmare = 7,
    DefeatedorLossNightmare = 8,
    CompletedNightmareWin1 = 9,
    CompletedNightmareWin2 = 10,
    CompletedNightmareLoss1 = 11,
    CompletedNightmareLoss2 = 12,
}


public enum ClassStatBracket
{
    None,
    PreMaster = 1,
    Master = 2,
    Grandmaster = 3
}

public enum TheSacrificeQuestStage
{
    None,
    Reconaissance = 1,
    AttackCaptors = 2,
    RescueChildren = 3
}

public enum AstridKillQuestStage
{
    None,
    AstridWolf = 1,
    AstridKobold = 2,
    AstridGoblinWarrior = 3,
    AstridGoblinGuard = 4,
    AstridGoblinSoldier = 5
}
[Flags]
public enum ReconPoints
{
    None,
    Reconpoint1 = 1,
    Reconpoint2 = 1 << 1,
    Reconpoint3 = 1 << 2,
    Reconpoint4 = 1 << 3, 
    Reconpoint5 = 1 << 4,
    Reconpoint6 = 1 << 5,
    Reconpoint7 = 1 << 6
}

[Flags]
public enum SavedChild
{
    None,
    savedchild = 1
}