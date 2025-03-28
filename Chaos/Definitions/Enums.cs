namespace Chaos.Definitions;

public enum AoeShape
{
    None,
    Front,
    AllAround,
    FrontalCone,
    FrontalDiamond,
    Circle,
    Square,
    CircleOutline,
    SquareOutline,
    Cleave
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

public enum StartEvent
{
    Off,
    StartEvent
}

public enum MonkElementForm
{
    Water,
    Earth = 1,
    Air = 2,
    Fire = 3
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
    GroupOnly = 1 << 16,
    PetOnly = 1 << 17,
    PetOwnerOnly = 1 << 18,
    PetOwnerGroupOnly = 1 << 19,
}

public enum VisibilityType
{
    Normal,
    Hidden,
    TrueHidden,
    GmHidden
}

public enum VisionType
{
    Normal,
    Blind,
    TrueBlind
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
    Foraging = 1 << 3
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
    KeptNecklace = 5
}

public enum ManorLouegieStage
{
    None = 0,
    AcceptedQuestBanshee = 1,
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
    StartedWhiteBatsWing = 7,
    StartedMarauderSpine = 8,
    StartedKardiFur = 9,
    StartedMimicTeeth = 10,
    StartedSuccubusHair = 11
}

public enum ALittleBitofThatStage
{
    None = 0,
    StartedApple = 1,
    StartedTomato = 2,
    StartedCherry = 3,
    StartedGrapes = 4,
    StartedMold = 5,
    StartedBaguette = 6
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
    Crafting = 1 << 2
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
    TurnedInTristar = 8
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
    BossBunnyDefeated = 10
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
    BossHorseDefeated = 10
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
    BossGargoyleDefeated = 10
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
    BossZombieDefeated = 10
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
    BossFrogDefeated = 10
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
    BossWolfDefeated = 10
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
    BossMantisDefeated = 10
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
    BossBeeDefeated = 10
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
    BossKoboldDefeated = 10
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
    BossGrimlockDefeated = 10
}
#endregion

public enum WolfProblemStage
{
    None = 0,
    Start = 1,
    Complete = 2
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
    SickChildKilled = 10
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
    SpokeToMaria = 5,
    QueenSpawning = 6,
    QueenSpawned = 7,
    QueenKilled = 8,
    Complete = 9
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
    popsicle = 10,
    hotchocolate = 11
}

[Flags]
public enum CookFoodProgression
{
    None = 0,
    NotenoughIngredients = 1
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
    tangerines = 7
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
    HotChocolate = 1 << 10
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
    Ant = 1 << 6
}

[Flags]
public enum AvailableCloaks
{
    Green = 1,
    Red = 1 << 2,
    Blue = 1 << 3,
    Black = 1 << 4,
    Purple = 1 << 5
}

public enum CurrentMount
{
    Horse = 1,
    Wolf = 2,
    Dunan = 3,
    Kelberoth = 4,
    Bee = 5,
    Ant = 6
}

public enum CurrentCloak
{
    Green = 1,
    Red = 2,
    Blue = 3,
    Black = 4,
    Purple = 5
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
    AdeptBelts = 1 << 8,
    BeginnerArmors = 1 << 9,
    BasicArmors = 1 << 10,
    InitiateArmors = 1 << 11,
    ArtisanArmors = 1 << 12,
    AdeptArmors = 1 << 13,
    RefiningKit = 1 << 14,
    AdvancedGauntlets = 1 << 15,
    AdvancedBelts = 1 << 16
}

[Flags]
public enum ArmorsmithingRecipes : ulong
{
    None = 0,
    LeatherSapphireGauntlet = 1L,
    LeatherRubyGauntlet = 1L << 2,
    LeatherEmeraldGauntlet = 1L << 3,
    LeatherHeartstoneGauntlet = 1L << 4,
    LeatherBerylGauntlet = 1L << 5,
    BronzeSapphireGauntlet = 1L << 6,
    BronzeRubyGauntlet = 1L << 7,
    BronzeEmeraldGauntlet = 1L << 8,
    BronzeHeartstoneGauntlet = 1L << 9,
    BronzeBerylGauntlet = 1L << 10,
    IronSapphireGauntlet = 1L << 11,
    IronRubyGauntlet = 1L << 12,
    IronEmeraldGauntlet = 1L << 13,
    IronHeartstoneGauntlet = 1L << 14,
    IronBerylGauntlet = 1L << 15,
    MythrilSapphireGauntlet = 1L << 16,
    MythrilRubyGauntlet = 1L << 17,
    MythrilEmeraldGauntlet = 1L << 18,
    MythrilHeartstoneGauntlet = 1L << 19,
    MythrilBerylGauntlet = 1L << 20,
    HybrasylSapphireGauntlet = 1L << 21,
    HybrasylRubyGauntlet = 1L << 22,
    HybrasylEmeraldGauntlet = 1L << 23,
    HybrasylHeartstoneGauntlet = 1L << 24,
    HybrasylBerylGauntlet = 1L << 25,
    EarthBelt = 1L << 26,
    SeaBelt = 1L << 27,
    FireBelt = 1L << 28,
    WindBelt = 1L << 29,
    FireBronzeBelt = 1L << 30,
    EarthBronzeBelt = 1L << 31,
    WindBronzeBelt = 1L << 32,
    SeaBronzeBelt = 1L << 33,
    DarkBronzeBelt = 1L << 34,
    LightBronzeBelt = 1L << 35,
    FireIronBelt = 1L << 36,
    SeaIronBelt = 1L << 37,
    WindIronBelt = 1L << 38,
    EarthIronBelt = 1L << 39,
    LightIronBelt = 1L << 40,
    DarkIronBelt = 1L << 41,
    FireMythrilBraidBelt = 1L << 42,
    EarthMythrilBraidBelt = 1L << 43,
    WindMythrilBraidBelt = 1L << 44,
    SeaMythrilBraidBelt = 1L << 45,
    DarkMythrilBraidBelt = 1L << 46,
    LightMythrilBraidBelt = 1L << 47,
    FireHybrasylBraidBelt = 1L << 48,
    EarthHybrasylBraidBelt = 1L << 49,
    WindHybrasylBraidBelt = 1L << 50,
    SeaHybrasylBraidBelt = 1L << 51,
    DarkHybrasylBraidBelt = 1L << 52,
    LightHybrasylBraidBelt = 1L << 53,
    LeatherGauntlet = 1L << 54
}

[Flags]
public enum ArmorsmithingRecipes2 : ulong
{
    None = 0,
    WindCrimsoniteBelt = 1L << 1,
    EarthCrimsoniteBelt = 1L << 2,
    FireCrimsoniteBelt = 1L << 3,
    SeaCrimsoniteBelt = 1L << 4,
    DarkCrimsoniteBelt = 1L << 5,
    LightCrimsoniteBelt = 1L << 6,
    WindAzuriumBelt = 1L << 7,
    EarthAzuriumBelt = 1L << 8,
    FireAzuriumBelt = 1L << 9,
    SeaAzuriumBelt = 1L << 10,
    DarkAzuriumBelt = 1L << 11,
    LightAzuriumBelt = 1L << 12,
    CrimsoniteRubyGauntlet = 1L << 13,
    CrimsoniteSapphireGauntlet = 1L << 14,
    CrimsoniteEmeraldGauntlet = 1L << 15,
    CrimsoniteBerylGauntlet = 1L << 16,
    CrimsoniteHeartstoneGauntlet = 1L << 17,
    AzuriumRubyGauntlet = 1L << 18,
    AzuriumSapphireGauntlet = 1L << 19,
    AzuriumEmeraldGauntlet = 1L << 20,
    AzuriumBerylGauntlet = 1L << 21,
    AzuriumHeartstoneGauntlet = 1L << 22
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

[Flags]
public enum CraftedArmors2 : ulong
{
    None = 0,
    RefiningKit = 1L
}
#endregion

#region Blacksmithing
[Flags]
public enum WeaponSmithingCategories : ulong
{
    None = 0,
    BasicSwords = 1L << 1,
    InitiateSwords = 1L << 2,
    ArtisanSwords = 1L << 3,
    AdeptSwords = 1L << 4,
    BasicWeapons = 1L << 5,
    InitiateWeapons = 1L << 6,
    ArtisanWeapons = 1L << 7,
    AdeptWeapons = 1L << 8,
    BasicStaves = 1L << 9,
    InitiateStaves = 1L << 10,
    ArtisanStaves = 1L << 11,
    AdeptStaves = 1L << 12,
    BasicDaggers = 1L << 13,
    InitiateDaggers = 1L << 14,
    ArtisanDaggers = 1L << 15,
    AdeptDaggers = 1L << 16,
    BasicClaws = 1L << 17,
    InitiateClaws = 1L << 18,
    ArtisanClaws = 1L << 19,
    AdeptClaws = 1L << 20,
    BasicShields = 1L << 21,
    InitiateShields = 1L << 22,
    ArtisanShields = 1L << 23,
    EmpowerStone = 1L << 24,
    EnchantStone = 1L << 25,
    AdvancedWeapons = 1L << 26,
    AdvancedSwords = 1L << 27,
    AdvancedStaves = 1L << 28,
    AdvancedDaggers = 1L << 29,
    AdvancedClaws = 1L << 30,
    AdvancedShields = 1L << 31,
    PolishingStone = 1L << 32,
    AdeptShields = 1L << 33
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
    StoneFist = 1L << 46,
    PhoenixClaw = 1L << 47,
    Nunchaku = 1L << 48,
    WoodenShield = 1L << 49,
    LeatherShield = 1L << 50,
    BronzeShield = 1L << 51,
    GravelShield = 1L << 52,
    IronShield = 1L << 53,
    LightShield = 1L << 54,
    MythrilShield = 1L << 55,
    HybrasylShield = 1L << 56
}

[Flags]
public enum WeaponSmithingRecipes2 : ulong
{
    None = 0,
    EmpowerStone = 1L << 1,
    EnchantStone = 1L << 2,
    ForestEscalon = 1L << 3,
    MoonEscalon = 1L << 4,
    ShadowEscalon = 1L << 5,
    RubySaber = 1L << 6,
    SapphireSaber = 1L << 7,
    BerylSaber = 1L << 8,
    StaffofZephyra = 1L << 9,
    StaffofAquaedon = 1L << 10,
    StaffofMiraelis = 1L << 11,
    StaffofIgnatar = 1L << 12,
    StaffofGeolith = 1L << 13,
    StaffofTheselene = 1L << 14,
    ScurvyDagger = 1L << 15,
    ChainWhip = 1L << 16,
    Kris = 1L << 17,
    RubyTonfa = 1L << 18,
    SapphireTonfa = 1L << 19,
    EmeraldTonfa = 1L << 20,
    TalgoniteShield = 1L << 21,
    CursedShield = 1L << 22,
    CathonicShield = 1L << 23,
    CaptainShield = 1L << 24,
    PolishingStone = 1L << 25
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
    ZephyraGust = 1L << 40,
    IgnatarDominance = 1L << 41,
    GeolithTestimony = 1L << 42,
    MiraelisBrace = 1L << 43,
    AquaedonCommitment = 1L << 44,
    SerendaelSuccess = 1L << 45,
    TheseleneSorrow = 1L << 46,
    SkandaraOffense = 1L << 47,
    ZephyraPower = 1L << 48,
    IgnatarDeficit = 1L << 49,
    GeolithBarrier = 1L << 50,
    MiraelisRoots = 1L << 51
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
    AdvancedCrimsoniteRings = 1L << 13,
    AdvancedAzuriumRings = 1L << 14,
    AdvancedCrimsoniteEarrings = 1L << 15,
    AdvancedAzuriumEarrings = 1L << 16,
    AdvancedCrimsoniteNecklaces = 1L << 17,
    AdvancedAzuriumNecklaces = 1L << 18,
    ExpertCrimsoniteNecklaces = 1L << 19,
    ExpertAzuriumNecklaces = 1L << 20
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
public enum JewelcraftingRecipes2 : ulong
{
    None = 0,
    CrimsoniteBerylRing = 1L << 1,
    CrimsoniteRubyRing = 1L << 2,
    CrimsoniteSapphireRing = 1L << 3,
    CrimsoniteEmeraldRing = 1L << 4,
    CrimsoniteHeartstoneRing = 1L << 5,
    AzuriumBerylRing = 1L << 6,
    AzuriumRubyRing = 1L << 7,
    AzuriumSapphireRing = 1L << 8,
    AzuriumEmeraldRing = 1L << 9,
    AzuriumHeartstoneRing = 1L << 10,
    CrimsoniteBerylEarrings = 1L << 11,
    CrimsoniteRubyEarrings = 1L << 12,
    CrimsoniteSapphireEarrings = 1L << 13,
    CrimsoniteEmeraldEarrings = 1L << 14,
    CrimsoniteHeartstoneEarrings = 1L << 15,
    AzuriumBerylEarrings = 1L << 16,
    AzuriumRubyEarrings = 1L << 17,
    AzuriumSapphireEarrings = 1L << 18,
    AzuriumEmeraldEarrings = 1L << 19,
    AzuriumHeartstoneEarrings = 1L << 20,
    CrimsoniteEarthNecklace = 1L << 21,
    CrimsoniteFireNecklace = 1L << 22,
    CrimsoniteSeaNecklace = 1L << 23,
    CrimsoniteWindNecklace = 1L << 24,
    CrimsoniteLightNecklace = 1L << 25,
    CrimsoniteDarkNecklace = 1L << 26,
    AzuriumEarthNecklace = 1L << 27,
    AzuriumFireNecklace = 1L << 28,
    AzuriumSeaNecklace = 1L << 29,
    AzuriumWindNecklace = 1L << 30,
    AzuriumLightNecklace = 1L << 31,
    AzuriumDarkNecklace = 1L << 32
}

[Flags]
public enum AlchemyCategories
{
    None = 0,
    BasicAlchemyBook = 1,
    InitiateAlchemyBook = 2,
    AttackTonics = 3,
    StrongVitalityBrew = 4,
    PotentVitalityBrew = 5
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
    PotentHealthPotion = 1L << 35,
    PotentManaPotion = 1L << 36,
    PotentRejuvenationPotion = 1L << 37,
    PotentHasteBrew = 1L << 38,
    PotentPowerBrew = 1L << 39,
    PotentAccuracyPotion = 1L << 40,
    InvisibilityPotion = 1L << 41,
    PoisonImmunityElixir = 1L << 42,
    PotionOfStrength = 1L << 43,
    PotionOfIntellect = 1L << 44,
    PotionOfWisdom = 1L << 45,
    PotionOfConstitution = 1L << 46,
    PotionOfDexterity = 1L << 47,
    StrongStatBoostElixir = 1L << 48,
    StrongKnowledgeElixir = 1L << 49,
    PotentJuggernautBrew = 1L << 50,
    PotentAstralBrew = 1L << 51
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

public enum PetMode
{
    Defensive = 0,
    Offensive = 1,
    Assist = 2,
    Passive = 3
}

public enum PetFollowMode
{
    AtFeet,
    Wander,
    DontMove,
    FollowAtDistance
}

public enum SummonChosenPet
{
    None = 0,
    Gloop = 1,
    Bunny = 2,
    Faerie = 3,
    Dog = 4,
    Ducklings = 5,
    Cat = 6,
    Smoldy = 7,
    Penguin = 8
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
    Level99 = 1 << 6
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
    PawStrike = 1,
    Enrage = 1 << 1,
    WindStrike = 1 << 2
}

public enum Level40PetSkills
{
    None,
    Blitz = 1,
    Slobber = 1 << 1,
    DoubleLick = 1 << 2
}

public enum Level55PetSkills
{
    None,
    Frenzy = 1,
    Spit = 1 << 1,
    Evade = 1 << 2
}

public enum Level80PetSkills
{
    None,
    ChitinChew = 1,
    SnoutStun = 1 << 1,
    EssenceLeechLick = 1 << 2
}

[Flags]
public enum PetSkillsChosen
{
    None,
    Level10 = 1,
    Level25 = 1 << 1,
    Level40 = 1 << 2,
    Level55 = 1 << 3,
    Level80 = 1 << 4,
    Level99 = 1 << 5
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

public enum ArenaSide
{
    None,
    Defender = 1,
    Offensive = 2
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
    DefeatedBoss = 10
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
    CompletedNightmareLoss2 = 12
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

public enum EastWoodlandsKillQuestStage
{
    None,
    EWViper = 1,
    EWBee1 = 2,
    EWBee2 = 3,
    EWMantis1 = 4,
    EWMantis2 = 5,
    EWWolf = 6,
    EWKobold = 7,
    EWKoboldMage = 8,
    EWGoblinGuard = 9,
    EWGoblinSoldier = 10,
    EWGoblinWarrior = 11,
    EWHobgoblin = 12
}

public enum WestWoodlandsKillQuestStage
{
    None,
    WWGoblinGuard = 1,
    WWGoblinWarrior = 2,
    WWHobGoblin = 3,
    WWShrieker = 4,
    WWWisp = 5,
    WWFaerie = 6,
    WWTwink = 7
}

public enum WestWoodlandsDungeonQuestStage
{
    None,
    Started = 1,
    Failed = 2,
    Completed = 3
}

public enum KarloposKillQuestStage
{
    None,
    KarloposCrab = 1,
    KarloposTurtle = 2,
    KarloposSlug = 3,
    KarloposSpore = 4,
    KarloposOctopus = 5,
    KarloposGog = 6,
    KarloposKraken = 7
}

public enum PietSewerKillQuestStage
{
    None,
    SewerCrab = 1,
    SewerTurtle = 2,
    SewerFrog = 3,
    SewerAnemone = 4,
    SewerBrawlfish = 5,
    SewerRockCobbler = 6,
    SewerKraken = 7,
    SewerGog = 8,
    SewerGremlin = 9,
    SewerMiniSkrull = 10,
    SewerSkrull = 11
}

public enum AbelDungeonKillQuestStage
{
    None,
    DungeonSlug = 1,
    DungeonGlupe = 2,
    DungeonLeech = 3,
    DungeonPolyp = 4,
    DungeonSpore = 5,
    DungeonDwarf = 6,
    DungeonDwarfSoldier = 7,
    DungeonBoss = 8
}

public enum DubhaimCastleKillQuestStage
{
    None,
    DubhaimDunan = 1,
    DubhaimGhast = 2,
    DubhaimCruel1 = 3,
    DubhaimCruel2 = 4,
    DubhaimGargoyle1 = 5,
    DubhaimGargoyle2 = 6,
    DubhaimGargoyle3 = 7,
    DubhaimGargoyleFiend1 = 8,
    DubhaimGargoyleFiend2 = 9
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
public enum EasterEggs
{
    None,
    LeakyBarrel = 1 << 1,
    UnlockedChest = 1 << 2,
    CrackInWall = 1 << 3,
    DepressedStatue = 1 << 4,
    GraveSite = 1 << 5
}

[Flags]
public enum SavedChild
{
    None,
    savedchild = 1
}

public enum CrudeLeather
{
    None,
    StartedQuest = 1
}

public enum RedNose
{
    None,
    StartedQuest = 1
}

public enum Rudolph
{
    None,
    StartedQuest = 1
}

public enum Erbie
{
    None,
    StartedQuest = 1
}

public enum DirtyErbie
{
    None,
    StartedErbies = 1
}

public enum GraveSite
{
    None,
    StartedQuest = 1,
    ReadTheHeadstone = 2,
    CompletedQuest = 3
}

public enum PurpleWhopper
{
    None,
    StartedQuest = 1
}

public enum PietWood
{
    None,
    StartedQuest = 1
}

public enum PrettyFlower
{
    None,
    StartedQuest = 1
}

public enum BertilPotion
{
    None,
    StartedQuest = 1
}

public enum SharpestBlade
{
    None,
    StartedQuest = 1
}

public enum FishOil
{
    None = 0,
    StartedQuest = 1
}

public enum DecoratingInn
{
    None,
    StartedPetunia = 1,
    CompletedPetunia = 2,
    StartedGoldRose = 3,
    CompletedGoldRose = 4,
    StartedPinkRose = 5,
    CompletedQuest = 6
}

public enum DragonScale
{
    None,
    StartedDragonScale = 1,
    FoundAllClues = 2,
    DroppedScale = 3,
    SpawnedDragon = 4,
    KilledDragon = 5,
    TurnedInScaleSword = 6,
    TurnedInScaleRing = 7,
    TurnedInScaleClaws = 8,
    TurnedInScaleGauntlet = 9,
    TurnedInScaleDagger = 10,
    CompletedDragonScale = 11
}

[Flags]
public enum DragonScaleFlags
{
    None,
    Callo = 1,
    Vidar = 1 << 1,
    Marcelo = 1 << 2,
    Avel = 1 << 3,
    Torbjorn = 1 << 4,
    Gunnar = 1 << 5
}

public enum HungryViveka
{
    None,
    StartedCherryPie = 1,
    CompletedCherryPie = 2
}

public enum UndineFieldDungeon
{
    None,
    StartedDungeon = 1,
    EnteredArena = 2,
    StartedCarnun = 3,
    KilledCarnun = 4
}

public enum PirateShip
{
    None,
    StartedShipAttack = 1,
    FinishedWave1 = 2,
    FinishedWave2 = 3,
    FinishedWave3 = 4,
    FinishedWave4 = 5,
    FinishedWave5 = 6,
    FinishedWave6 = 7,
    FinishedWave7 = 8,
    FinishedWave8 = 9,
    FinishedWave9 = 10,
    FinishedWave10 = 11,
    FinishedShipAttack = 12,
    CompletedShipAttack = 13
}

public enum HelpSable
{
    None,
    StartedSable = 1,
    FinishedSable = 2,
    StartedSam = 3,
    FinishedSam = 4,
    StartedRoger = 5,
    FinishedRoger = 6,
    StartedCaptain = 7,
    FinishedCaptain = 8,
    StartedDoltoo = 9,
    EscortingDoltooStart = 10,
    EscortingDoltooFailed = 11,
    CompletedEscort = 12,
    FinishedDoltoo = 13
}

[Flags]
public enum ShipAttackFlags
{
    None,
    CompletedShipAttack = 1,
    FinishedDoltoo = 2
}

[Flags]
public enum UndineFieldDungeonFlag
{
    None,
    CompletedUF = 1
}

public enum MainStoryEnums
{
    None,
    MysteriousArtifactFound = 1,
    ReceivedMA = 2,
    SpokeToZephyr = 3,
    StartedArtifact1 = 4,
    FinishedArtifact1 = 5,
    StartedArtifact2 = 6,
    FinishedArtifact2 = 7,
    StartedArtifact3 = 8,
    FinishedArtifact3 = 9,
    StartedArtifact4 = 10,
    FinishedArtifact4 = 11,
    StartedAssemble = 12,
    CompletedArtifactsHunt = 13,
    StartedFirstTrial = 14,
    FinishedFirstTrial = 15,
    StartedSecondTrial = 16,
    FinishedSecondTrial = 17,
    StartedThirdTrial = 18,
    FinishedThirdTrial = 19,
    StartedFourthTrial = 20,
    FinishedFourthTrial = 21,
    CompletedTrials = 22,
    SearchForSummoner = 23,
    Entered3rdFloor = 24,
    RetryServant = 25,
    DefeatedServant = 26,
    CompletedServant = 27,
    SearchForSummoner2 = 28,
    FoundSummoner2 = 29,
    SpawnedCreants = 30,
    StartedSummonerFight = 31,
    KilledSummoner = 32,
    CompletedPreMasterMainStory = 33
}

public enum MainstoryMasterEnums
{
    None = 0,
    StartedDungeon = 1,
    FinishedDungeon = 2,
    CompletedDungeon = 3,
    StartedCreants = 4,
    KilledCreants = 5,
    CompletedCreants = 6
}

[Flags]
public enum CdDungeonBoss
{
    None = 0,
    John = 1 << 1,
    Jane = 1 << 2,
    Roy = 1 << 3,
    Ray = 1 << 4,
    Mike = 1 << 5,
    Mary = 1 << 6,
    Phil = 1 << 7,
    Pam = 1 << 8,
    William = 1 << 9,
    Wanda = 1 << 10,
    CompletedDungeonOnce = 1 << 11
}

[Flags]
public enum CreantEnums
{
    StartedMedusa = 1 << 1,
    KilledMedusa = 1 << 2,
    CompletedMedusa = 1 << 3,
    StartedPhoenix = 1 << 4,
    KilledPhoenix = 1 << 5,
    CompletedPhoenix = 1 << 6,
    StartedTauren = 1 << 7,
    KilledTauren = 1 << 8,
    CompletedTauren = 1 << 9,
    StartedSham = 1 << 10,
    KilledSham = 1 << 11,
    CompletedSham = 1 << 12
}

public enum CreantPhases
{
    None,
    InPhase = 1
}

public enum SummonerBossFight
{
    None,
    StartedSummonerFight = 1,
    FirstStage = 2,
    FirstStage1 = 3,
    SecondStage = 4,
    SecondStage1 = 5,
    ThirdStage = 6,
    ThirdStage1 = 7,
    FourthStage = 8,
    FourthStage1 = 9,
    FifthStage = 10,
    FifthStage1 = 11,
    KilledSummoner = 12,
    CompletedSummoner = 13
}

public enum CombatTrial
{
    None,
    StartedTrial = 1,
    StartedFirst = 2,
    FinishedFirst = 3,
    StartedSecond = 4,
    FinishedSecond = 5,
    StartedThird = 6,
    FinishedThird = 7,
    StartedFourth = 8,
    FinishedFourth = 9,
    StartedFifth = 10,
    FinishedFifth = 11,
    FinishedTrial = 12
}

public enum SacrificeTrial
{
    None,
    StartedTrial = 1,
    StartedFirst = 2,
    FinishedFirst = 3,
    StartedSecond = 4,
    FinishedSecond = 5,
    StartedThird = 6,
    FinishedThird = 7,
    StartedFourth = 8,
    FinishedFourth = 9,
    StartedFifth = 10,
    FinishedFifth = 11,
    FinishedTrial = 12
}

public enum LuckTrial
{
    None,
    StartedTrial = 1,
    BadDoor = 2,
    SucceededFirst = 3,
    SucceededSecond = 4,
    SucceededThird = 5,
    CompletedTrial = 6,
    CompletedTrial2 = 7
}

public enum IntelligenceTrial
{
    None,
    StartedTrial = 1,
    CompletedTrial = 2,
    CompletedTrial2 = 3
}

[Flags]
public enum MainstoryFlags
{
    None,
    FoundMysteriousArtifact = 1 << 1,
    AccessGodsRealm = 1 << 2,
    CompletedArtifact1 = 1 << 3,
    CompletedArtifact2 = 1 << 4,
    CompletedArtifact3 = 1 << 5,
    CompletedArtifact4 = 1 << 6,
    CompletedFloor3 = 1 << 7,
    FinishedDungeon = 1 << 8,
    FinishedCreants = 1 << 9,
    ReceivedRewards = 1 << 10, //useless enum, can delete for live.
    CreantRewards = 1 << 11,
    BountyBoardEpicChampion = 1 << 12
}

[Flags]
public enum CthonicDemiseBoss
{
    None,
    John = 1 << 1,
    Jane = 1 << 2,
    Phil = 1 << 3,
    Pam = 1 << 4,
    Roy = 1 << 5,
    Ray = 1 << 6,
    William = 1 << 7,
    Wanda = 1 << 8,
    Mike = 1 << 9,
    Mary = 1 << 10
}

public enum SupplyLouresStage
{
    None,
    StartedQuest = 1,
    StartedSupply = 2,
    SawAssassin = 3,
    KilledAssassin = 4,
    KilledAssassin2 = 5,
    TurnedInSupply = 6,
    CompletedSupply = 7,
    KeptThibaultsSecret = 8,
    CompletedAssassin1 = 9,
    CompletedAssassin2 = 10,
    CompletedAssassin3 = 11
}

public enum ExpTimerStage
{
    None,
    Tracking = 1,
    Stopped = 2
}

public enum WerewolfOfPiet
{
    None = 0,
    StartedQuest = 1,
    FollowedMerchant = 2,
    KilledandGotCursed = 3,
    SpokeToWizard = 4,
    SpawnedWerewolf2 = 5,
    RetryWerewolf = 6,
    KilledWerewolf = 7,
    CollectedBlueFlower = 8,
    ReceivedCure = 9
}

[Flags]
public enum LanternSizes
{
    None = 0,
    SmallLantern = 1,
    LargeLantern = 2
}

public enum AttackedWerewolf
{
    None = 0,
    Yes = 1,
    No = 2
}

[Flags]
public enum FishingQuest
{
    None = 0,
    Reached250 = 1 << 1,
    Reached500 = 1 << 2,
    Reached800 = 1 << 3,
    Reached1500 = 1 << 4,
    Reached3000 = 1 << 5,
    Reached5000 = 1 << 6,
    Reached7500 = 1 << 7,
    Reached10000 = 1 << 8,
    Reached12500 = 1 << 9,
    Reached15000 = 1 << 10,
    Reached20000 = 1 << 11,
    Reached25000 = 1 << 12,
    Reached30000 = 1 << 13,
    Reached40000 = 1 << 14,
    Reached50000 = 1 << 15,
    CompletedFishing = 1 << 16
}

[Flags]
public enum ForagingQuest
{
    None = 0,
    Reached250 = 1 << 1,
    Reached500 = 1 << 2,
    Reached800 = 1 << 3,
    Reached1500 = 1 << 4,
    Reached3000 = 1 << 5,
    Reached5000 = 1 << 6,
    Reached7500 = 1 << 7,
    Reached10000 = 1 << 8,
    Reached12500 = 1 << 9,
    Reached15000 = 1 << 10,
    Reached20000 = 1 << 11,
    Reached25000 = 1 << 12,
    Reached30000 = 1 << 13,
    Reached40000 = 1 << 14,
    Reached50000 = 1 << 15,
    CompletedForaging = 1 << 16
}

public enum MasterPriestPath
{
    None = 0,
    Dark = 1,
    Light = 2
}

[Flags]
public enum SenaanFlagsHalloweenEvent
{
    None = 0,
    CompletedYear1 = 1 << 1,
    CompletedYear2 = 1 << 2
}

public enum ThanksgivingChallenge
{
    None = 0,
    FirstWave = 1 << 1,
    SecondWave = 1 << 2,
    ThirdWave = 1 << 3,
    FourthWave = 1 << 4,
    FifthWave = 1 << 5,
    SixthWave = 1 << 6,
    SeventhWave = 1 << 7,
    EighthWave = 1 << 8,
    NinthWave = 1 << 9,
    TenthWave = 1 << 10,
    EleventhWave = 1 << 11,
    TwelvethWave = 1 << 12,
    ThirteenthWave = 1 << 13,
    FourteenthWave = 1 << 14,
    FifteenthWave = 1 << 15,
    SixteenthWave = 1 << 16,
    SeventeenthWave = 1 << 17,
    EighteenthWave = 1 << 18,
    NinteenthWave = 1 << 19,
    TwentyEighthWave = 1 << 20
}

[Flags]
public enum BountyQuestFlags1 : ulong
{
    None = 0,
    EasyAncientBeetalic = 1L << 1,
    EasyAncientSkeleton = 1L << 2,
    EasyLosgann = 1L << 3,
    EasyRuidhtear = 1L << 4,
    EasyBrownMantis = 1L << 5,
    EasyGoldBeetalic = 1L << 6,
    EasyRedShocker = 1L << 7,
    EasyBlackShocker = 1L << 8,
    EasyGoldShocker = 1L << 9,
    EasyBlueShocker = 1L << 10,
    EasyDireWolf = 1L << 11,
    EasyIceElemental = 1L << 12,
    EasyIceSkeleton = 1L << 13,
    EasyIceSpore = 1L << 14,
    MediumAncientBeetalic = 1L << 15,
    MediumAncientSkeleton = 1L << 16,
    MediumLosgann = 1L << 17,
    MediumRuidhtear = 1L << 18,
    MediumBrownMantis = 1L << 19,
    MediumGoldBeetalic = 1L << 20,
    MediumRedShocker = 1L << 21,
    MediumBlackShocker = 1L << 22,
    MediumGoldShocker = 1L << 23,
    MediumBlueShocker = 1L << 24,
    MediumDireWolf = 1L << 25,
    MediumIceElemental = 1L << 26,
    MediumIceSkeleton = 1L << 27,
    MediumIceSpore = 1L << 28,
    HardAncientBeetalic = 1L << 29,
    HardAncientSkeleton = 1L << 30,
    HardLosgann = 1L << 31,
    HardRuidhtear = 1L << 32,
    HardBrownMantis = 1L << 33,
    HardGoldBeetalic = 1L << 34,
    HardRedShocker = 1L << 35,
    HardBlackShocker = 1L << 36,
    HardGoldShocker = 1L << 37,
    HardBlueShocker = 1L << 38,
    HardDireWolf = 1L << 39,
    HardIceElemental = 1L << 40,
    HardIceSkeleton = 1L << 41,
    HardIceSpore = 1L << 42,
    EpicKelberoth = 1L << 43,
    EpicAncientDraco = 1L << 44,
    EpicGreenMantis = 1L << 45,
    EpicHydra = 1L << 46
}

[Flags]
public enum BountyQuestFlags2 : ulong { }

[Flags]
public enum BountyQuestFlags3 : ulong { }

[Flags]
public enum AvailableQuestFlags1 : ulong
{
    None = 0,
    EasyAncientBeetalic = 1L << 1,
    EasyAncientSkeleton = 1L << 2,
    EasyLosgann = 1L << 3,
    EasyRuidhtear = 1L << 4,
    EasyBrownMantis = 1L << 5,
    EasyGoldBeetalic = 1L << 6,
    EasyRedShocker = 1L << 7,
    EasyBlackShocker = 1L << 8,
    EasyGoldShocker = 1L << 9,
    EasyBlueShocker = 1L << 10,
    EasyDireWolf = 1L << 11,
    EasyIceElemental = 1L << 12,
    EasyIceSkeleton = 1L << 13,
    EasyIceSpore = 1L << 14,
    MediumAncientBeetalic = 1L << 15,
    MediumAncientSkeleton = 1L << 16,
    MediumLosgann = 1L << 17,
    MediumRuidhtear = 1L << 18,
    MediumBrownMantis = 1L << 19,
    MediumGoldBeetalic = 1L << 20,
    MediumRedShocker = 1L << 21,
    MediumBlackShocker = 1L << 22,
    MediumGoldShocker = 1L << 23,
    MediumBlueShocker = 1L << 24,
    MediumDireWolf = 1L << 25,
    MediumIceElemental = 1L << 26,
    MediumIceSkeleton = 1L << 27,
    MediumIceSpore = 1L << 28,
    HardAncientBeetalic = 1L << 29,
    HardAncientSkeleton = 1L << 30,
    HardLosgann = 1L << 31,
    HardRuidhtear = 1L << 32,
    HardBrownMantis = 1L << 33,
    HardGoldBeetalic = 1L << 34,
    HardRedShocker = 1L << 35,
    HardBlackShocker = 1L << 36,
    HardGoldShocker = 1L << 37,
    HardBlueShocker = 1L << 38,
    HardDireWolf = 1L << 39,
    HardIceElemental = 1L << 40,
    HardIceSkeleton = 1L << 41,
    HardIceSpore = 1L << 42,
    EpicKelberoth = 1L << 43,
    EpicAncientDraco = 1L << 44,
    EpicGreenMantis = 1L << 45,
    EpicHydra = 1L << 46
}

[Flags]
public enum AvailableQuestFlags2 : ulong { }

[Flags]
public enum AvailableQuestFlags3 : ulong { }

public enum FlourentineQuest
{
    None = 0,
    SpeakWithJester = 1,
    SpeakWithSnaggles = 2,
    SpeakWithZappy = 3,
    SpeakWithFlourentineFirst = 4,
    SpeakWithFlourentineAfterZappy = 5,
    FinishedQuest = 6
}

[Flags]
public enum InvisibleGear
{
    None = 0,
    HideHelmet = 1 << 1,
    HideWeapon = 1 << 2,
    HideShield = 1 << 3,
    HideBoots = 1 << 4,
    HideArmor = 1 << 5,
    HideAccessoryOne = 1 << 6,
    HideAccessoryTwo = 1 << 7,
    HideAccessoryThree = 1 << 8,
}

public enum LuckyCharmsQuest
{
    None = 0,
    Accepted = 1,
    TurnedIn = 2
}

public enum AlchemyTrinketUsage
{
    None = 0,
    SelectedPotion = 1
}

public enum AlchemyTrinketSelection
{
    None = 0,
    Hemloch = 1,
    SmallHealthPotion = 2,
    SmallManaPotion = 3,
    SmallRejuvenationPotion = 4,
    SmallHasteBrew = 5,
    SmallPowerBrew = 6,
    SmallAccuracyPotion = 7,
    JuggernautBrew = 8,
    AstralBrew = 9,
    AntidotePotion = 10,
    SmallFirestormTonic = 11,
    SmallStunTonic = 12,
    HealthPotion = 13,
    ManaPotion = 14,
    RejuvenationPotion = 15,
    HasteBrew = 16,
    PowerBrew = 17,
    AccuracyPotion = 18,
    RevivePotion = 19,
    StrongJuggernautBrew = 20,
    StrongAstralBrew = 21,
    CleansingBrew = 22,
    FirestormTonic = 23,
    StunTonic = 24,
    WarmthPotion = 25,
    AmnesiaBrew = 26,
    StrongHealthPotion = 27,
    StrongManaPotion = 28,
    StrongHasteBrew = 29,
    StrongPowerBrew = 30,
    StrongAccuracyPotion = 31,
    StatBoostElixir = 32,
    KnowledgeElixir = 33,
    StrongRejuvenationPotion = 34,
    PotentHealthPotion = 35,
    PotentManaPotion = 36,
    PotentRejuvenationPotion = 37,
    PotentHasteBrew = 38,
    PotentPowerBrew = 39,
    PotentAccuracyPotion = 40,
    InvisibilityPotion = 41,
    PoisonImmunityElixir = 42,
    PotionOfStrength = 43,
    PotionOfIntellect = 44,
    PotionOfWisdom = 45,
    PotionOfConstitution = 46,
    PotionOfDexterity = 47,
    StrongStatBoostElixir = 48,
    StrongKnowledgeElixir = 49,
    PotentJuggernautBrew = 50,
    PotentAstralBrew = 51
}
