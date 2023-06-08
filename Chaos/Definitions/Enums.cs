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
    Amensia = 1 << 24,
    //add more statuses here, double each time
}

public enum GainExp
{
    Yes,
    No = 1
}

public enum MonkElementForm
{
    Water,
    Earth = 1,
    Air = 2,
    Fire = 3,
}

[Flags]
public enum TargetFilter
{
    None,
    FriendlyOnly = 1,
    HostileOnly = 1 << 1,
    AliveOnly = 1 << 2,
    DeadOnly = 1 << 3,
    AislingsOnly = 1 << 4,
    MonstersOnly = 1 << 5,
    SelfOnly = 1 << 6,
    GroupOnly = 1 << 7
}

public enum VisibilityType
{
    Normal,
    Hidden,
    TrueHidden,
    GmHidden
}

public enum Craft
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
    GaveStickAndArmor = 1,
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
    ObtainedNecklace = 2,
    ReturnedNecklace = 3,
    KeptNecklace = 4,
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

public enum RionaRatQuestStage
{
    None = 0,
    StartedRatQuest = 1,
    CompletedRatQuest = 2
}
public enum PFQuestStage
{
    None = 0,
    StartedPFQuest = 1,
    TurnedInRoots = 2,
    WolfManes = 3,
    WolfManesTurnedIn = 4,
    FoundPendant = 5,
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
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicHorse
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicGargoyle
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicZombie
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicFrog
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicWolf
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicMantis
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicBee
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicKobold
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
}

public enum MythicGrimlock
{
    None = 0,
    Lower = 1,
    LowerComplete = 2,
    Higher = 3,
    HigherComplete = 4,
    Item = 5,
    ItemComplete = 6,
    Allied = 7,
    EnemyAllied = 8,
    BossStarted = 9,
    BossDefeated = 10,
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
    sweetbuns = 9
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
    salt = 5
}
public enum ExtraIngredientsStage2
{
    none = 0,
    bread = 1,
    cheese = 2,
    flour = 3,
    marinade = 4,
    salt = 5
}
public enum ExtraIngredientsStage3
{
    none = 0,
    bread = 1,
    cheese = 2,
    flour = 3,
    marinade = 4,
    salt = 5
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
    SweetBuns = 1 << 8
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
    CollectedBouquet = 11,
    DeliveredBouquet = 12,
    CompletedFiskSecret = 13,
    AngeredThulin = 14,
    AngeredThulinReturn = 15,
    ExposedFisk = 16,
    WontTell = 17,
    WontTell2 = 18,
    MushroomStart = 19,
    Started4 = 20,
    Started5 = 21,
    DeliverBouquet = 22,
    Started6 = 23,
    BrandyTurnin = 24
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
    None,
    WhiteHorse = 1,
    WhiteWolf = 2
}

public enum CurrentMount
{
    None,
    WhiteHorse = 1,
    WhiteWolf = 2,
}

#region  Armor Smithing

[Flags]
public enum ArmorSmithCategories
{
    None = 0,
    Basicgauntlets = 1,
    Apprenticegauntlets = 1 << 2,
    Journeymangauntlets = 1 << 3,
    Adeptgauntlets = 1 << 4,
    Basicbelts = 1 << 5,
    Apprenticebelts = 1 << 6,
    Journeymanbelts = 1 << 7,
    Basicarmors = 1 << 8,
    Apprenticearmors = 1 << 9,
    Journeymanarmors = 1 << 10,
    Adeptarmors = 1 << 11,
    Advancedarmors = 1 << 12,
}

[Flags]
public enum ArmorSmithRecipes : ulong
{
    None = 0,
    Leathersapphiregauntlet = 1L,
    Leatherrubygauntlet = 1L << 2,
    Leatheremeraldgauntlet = 1L << 3,
    Leatherheartstonegauntlet = 1L << 4,
    Ironsapphiregauntlet = 1L << 5,
    Ironrubygauntlet = 1L << 6,
    Ironemeraldgauntlet = 1L << 7,
    Ironheartstonegauntlet = 1L << 8,
    Mythrilsapphiregauntlet = 1L << 9,
    Mythrilrubygauntlet = 1L << 10,
    Mythrilemeraldgauntlet = 1L << 11,
    Mythrilheartstonegauntlet = 1L << 12,
    Hybrasylsapphiregauntlet = 1L << 13,
    Hybrasylrubygauntlet = 1L << 14,
    Hybrasylemeraldgauntlet = 1L << 15,
    Hybrasylheartstonegauntlet = 1L << 16,
    Jeweledseabelt = 1L << 17,
    Jeweledfirebelt = 1L << 18,
    Jeweledwindbelt = 1L << 19,
    Jeweledearthbelt = 1L << 20,
    Jewelednaturebelt = 1L << 21,
    Jeweledmetalbelt = 1L << 22,
    Jeweleddarkbelt = 1L << 23,
    Jeweledlightbelt = 1L << 24
}

[Flags]
public enum CraftedArmors : ulong
{
    None = 0,
    Refinedscoutleather = 1L,
    Refineddwarvishleather = 1L << 2,
    Refinedpaluten = 1L << 3,
    Refinedkeaton = 1L << 4,
    Refinedbardocle = 1L << 5,
    Refinedgardcorp = 1L << 6,
    Refinedjourneyman = 1L << 7,
    Refinedlorum = 1L << 8,
    Refinedmane = 1L << 9,
    Refinedduinuasal = 1L << 10,
    Refinedcowl = 1L << 11,
    Refinedgaluchatcoat = 1L << 12,
    Refinedmantle = 1L << 13,
    Refinedhierophant = 1L << 14,
    Refineddalmatica = 1L << 15,
    Refinedcotte = 1L << 16,
    Refinedbrigandine = 1L << 17,
    Refinedcorsette = 1L << 18,
    Refinedpebblerose = 1L << 19,
    Refinedkagum = 1L << 20,
    Refinedmagiskirt = 1L << 21,
    Refinedbenusta = 1L << 22,
    Refinedstoller = 1L << 23,
    Refinedclymouth = 1L << 24,
    Refinedclamyth = 1L << 25,
    Refinedgorgetgown = 1L << 26,
    Refinedmysticgown = 1L << 27,
    Refinedelle = 1L << 28,
    Refineddolman = 1L << 29,
    Refinedbansagart = 1L << 30,
    Refineddobok = 1L << 31,
    Refinedculotte = 1L << 32,
    Refinedearthgarb = 1L << 33,
    Refinedwindgarb = 1L << 34,
    Refinedmountaingarb = 1L << 35,
    Refinedleathertunic = 1L << 36,
    Refinedlorica = 1L << 37,
    Refinedkasmaniumarmor = 1L << 38,
    Refinedipletmail = 1L << 39,
    Refinedhybrasylplate = 1L << 40,
    Refinedearthbodice = 1L << 41,
    Refinedlotusbodice = 1L << 42,
    Refinedmoonbodice = 1L << 43,
    Refinedlightninggarb = 1L << 44,
    Refinedseagarb = 1L << 45,
    Refinedleatherbliaut = 1L << 46,
    Refinedcuirass = 1L << 47,
    Refinedkasmaniumhauberk = 1L << 48,
    Refinedphoenixmail = 1L << 49,
    Refinedhybrasylarmor = 1L << 50
}
#endregion

#region Blacksmithing
[Flags]
public enum WeaponSmithingCategories
{
    None = 0,
    Basicswords = 1,
    Apprenticeswords = 1 << 2,
    Journeymanswords = 1 << 3,
    Adeptswords = 1 << 4,
    Basicweapons = 1 << 5,
    Apprenticeweapons = 1 << 6,
    Journeymanweapons = 1 << 7,
    Adeptweapons = 1 << 8,
    Basicstaves = 1 << 9,
    Apprenticestaves = 1 << 10,
    Journeymanstaves = 1 << 11,
    Adeptstaves = 1 << 12,
    Basicdaggers = 1 << 13,
    Apprenticedaggers = 1 << 14,
    Journeymandaggers = 1 << 15,
    Adeptdaggers = 1 << 16,
    Basicclaws = 1 << 17,
    Apprenticeclaws = 1 << 18,
    Journeymanclaws = 1 << 19,
    Adeptclaws = 1 << 20,
    Basicshields = 1 << 21,
    Apprenticeshields = 1 << 22,
    Journeymanshields = 1 << 23
}

[Flags]
public enum WeaponSmithingRecipes : ulong
{
    None = 0,
    Eppe = 1L,
    Saber = 1L << 2,
    Claidheamh = 1L << 3,
    Broadsword = 1L << 4,
    Battlesword = 1L << 5,
    Masquerade = 1L << 6,
    Bramble = 1L << 7,
    Longsword = 1L << 8,
    Claidhmore = 1L << 9,
    Emeraldsword = 1L << 10,
    Gladius = 1L << 11,
    Kindjal = 1L << 12,
    Dragonslayer = 1L << 13,
    Hatchet = 1L << 14,
    Harpoon = 1L << 15,
    Scimitar = 1L << 16,
    Club = 1L << 17,
    Spikedclub = 1L << 18,
    Chainmace = 1L << 19,
    Handaxe = 1L << 20,
    Cutlass = 1L << 21,
    Talgoniteaxe = 1L << 22,
    Hybrasylaxe = 1L << 23,
    Magusares = 1L << 24,
    Holyhermes = 1L << 25,
    Maguszeus = 1L << 26,
    Holykronos = 1L << 27,
    Magusdiana = 1L << 28,
    Holydiana = 1L << 29,
    Stonecross = 1L << 30,
    Oakstaff = 1L << 31,
    Staffofwisdom = 1L << 32,
    Snowdagger = 1L << 33,
    Centerdagger = 1L << 34,
    Blossomdagger = 1L << 35,
    Curveddagger = 1L << 36,
    Moondagger = 1L << 37,
    Lightdagger = 1L << 38,
    Sundagger = 1L << 39,
    Lotusdagger = 1L << 40,
    Blooddagger = 1L << 41,
    Nagetierdagger = 1L << 42,
    Dullclaw = 1L << 43,
    Wolfclaw = 1L << 44,
    Eagletalon = 1L << 45,
    Phoenixclaw = 1L << 46,
    Nunchaku = 1L << 47,
    Woodenshield = 1L << 48,
    Leathershield = 1L << 49,
    Bronzeshield = 1L << 50,
    Gravelshield = 1L << 51,
    Ironshield = 1L << 52,
    Lightshield = 1L << 53,
    Mythrilshield = 1L << 54,
    Hybrasylshield = 1L << 55
}
#endregion
[Flags]
public enum EnchantingRecipes
{
    None = 0,
    MiraelisEmbrace = 1,
    SkandaraResolve = 2,
}

[Flags]
public enum JewelcraftingRecipes
{
    None = 0,
    
}

[Flags]
public enum AlchemyRecipes
{
    None = 0,
    Hemloch = 1,
    BetonyDeum = 1 << 2,
}