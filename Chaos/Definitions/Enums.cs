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

public enum ProfessionCount
{
    None,
    One = 1,
    Two = 2
}

public enum Professions
{
    None,
    Fishing = 1,
    Tailoring = 2,
    Cooking = 3,
    Enchanting = 4,
    Alchemy = 5,
    Blacksmithing = 6
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
    dinnerplate = 1,
    fruitbasket = 1 << 1,
    lobsterdinner = 1 << 2,
    pie = 1 << 3,
    salad = 1 << 4,
    sandwich = 1 << 5,
    soup = 1 << 6,
    steakmeal = 1 << 7,
    sweetbuns = 1 << 8
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

#region  Armor Smithing Recipes

#region Armor Smith Categories

[Flags]
public enum ArmorSmithRecipes
{
    None = 0,
    basicgauntlets = 1,
    apprenticegauntlets = 1 << 2,
    journeymangauntlets = 1 << 3,
    adeptgauntlets = 1 << 4,
    basicbelts = 1 << 5,
    apprenticebelts = 1 << 6,
    journeymanbelts = 1 << 7,
    basicarmors = 1 << 8,
    apprenticearmors = 1 << 9,
    journeymanarmors = 1 << 10,
    adeptarmors = 1 << 11,
    advancedarmors = 1 << 12,
}

#endregion

#region Gauntlets

[Flags]
public enum BasicGauntletRecipes
{
    None = 0,
    leathersapphiregauntlet = 1,
    leatherrubygauntlet = 1 << 2,
    leatheremeraldgauntlet = 1 << 3,
    leatherheartstonegauntlet = 1 << 4,
}

[Flags]
public enum ApprenticeGauntletRecipes
{
    None = 0,
    ironsapphiregauntlet = 1,
    ironrubygauntlet = 2,
    ironemeraldgauntlet = 3,
    ironheartstonegauntlet = 4,
}

[Flags]
public enum JourneymanGauntletRecipes
{
    None = 0,
    mythrilsapphiregauntlet = 1,
    mythrilrubygauntlet = 2,
    mythrilemeraldgauntlet = 3,
    mythrilheartstonegauntlet = 4,
}
[Flags]
public enum AdeptGauntletRecipes
{
    None = 0,
    hybrasylsapphiregauntlet = 1,
    hybrasylrubygauntlet = 2,
    hybrasylemeraldgauntlet = 3,
    hybrasylheartstonegauntlet = 4,
}
#endregion

#region Belts

[Flags]
public enum BasicBelts
{
    None = 0,
    jeweledseabelt = 1,
    jeweledfirebelt = 1 << 2,
    jeweledwindbelt = 1 << 3,
    jeweledearthbelt = 1 << 4
}

public enum ApprenticeBelts
 {
     None = 0,
     jewelednaturebelt = 1,
     jeweledmetalbelt = 1 << 2
 }
public enum JourneymanBelts
{
    None = 0,
    jeweleddarkbelt = 1,
    jeweledlightbelt = 1 << 2
}


    #endregion

#region CraftedArmors

[Flags]
public enum CraftedArmors
{
    None = 0,
    refinedscoutleather = 1,
    refineddwarvishleather = 1 << 2,
    refinedpaluten = 1 << 3,
    refinedkeaton = 1 << 4,
    refinedbardocle = 1 << 5,
    refinedgardcorp = 1 << 6,
    refinedjourneyman = 1 << 7,
    refinedlorum = 1 << 8,
    refinedmane = 1 << 9,
    refinedduinuasal = 1 << 10,
    refinedcowl = 1 << 11,
    refinedgaluchatcoat = 1 << 12,
    refinedmantle = 1 << 13,
    refinedhierophant = 1 << 14,
    refineddalmatica = 1 << 15,
    refinedcotte = 1 << 16,
    refinedbrigadine = 1 << 17,
    refinedcorsette = 1 << 18,
    refinedpebblerose = 1 << 19,
    refinedkagum = 1 << 20,
    refinedmagiskirt = 1 << 21,
    refinedbenusta = 1 << 22,
    refinedstoller = 1 << 23,
    refinedclymouth = 1 << 24,
    refinedclamyth = 1 << 25,
    refinedgorgetgown = 1 << 26,
    refinedmysticgown = 1 << 27,
    refinedelle = 1 << 28,
    refineddolman = 1 << 29,
    refinedbansagart = 1 << 30
}

    #endregion

#endregion

#region Weapon Smithing Recipes

#region Weapon Smith Categories
[Flags]
public enum WeaponSmithRecipes
{
    None = 0,
    basicswords = 1,
    apprenticeswords = 1 << 2,
    journeymanswords = 1 << 3,
    adeptswords = 1 << 4,
    basicweapons = 1 << 5,
    apprenticeweapons = 1 << 6,
    journeymanweapons = 1 << 7,
    adeptweapons = 1 << 8,
    basicstaves = 1 << 9,
    apprenticestaves = 1 << 10,
    journeymanstaves = 1 << 11,
    adeptstaves = 1 << 12,
    basicdaggers = 1 << 13,
    apprenticedaggers = 1 << 14,
    journeymandaggers = 1 << 15,
    adeptdaggers = 1 << 16,
    basicclaws = 1 << 17,
    apprenticeclaws = 1 << 18,
    journeymanclaws = 1 << 19,
    basicshields = 1 << 20,
    apprenticeshields = 1 << 21,
    journeymanshields = 1 << 22,
}
#endregion

#region Swords

[Flags]
public enum Swords
{
    None = 0,
    eppe = 1,
    saber = 1 << 2,
    claidheamh = 1 << 3,
    broadsword = 1 << 4,
    battlesword = 1 << 5,
    masquerade = 1 << 6,
    bramble = 1 << 7,
    longsword = 1 << 8,
    claidhmore = 1 << 9,
    emeraldsword = 1 << 10,
    gladius = 1 << 11,
    kindjal = 1 << 12,
    dragonslayer = 1 << 13
}

    #endregion

#region Weapons

[Flags]
public enum Weapons
{
    None = 0,
    hatchet = 1,
    harpoon = 1 << 2,
    scimitar = 1 << 3,
    club = 1 << 4,
    spikedclub = 1 << 5,
    chainmace = 1 << 6,
    handaxe = 1 << 7,
    cutlass = 1 << 8,
    talgoniteaxe = 1 << 9,
    hybrasylaxe = 1 << 10
}
#endregion

#region Staves

[Flags]
public enum Staves
{
    None = 0,
    magusares = 1,
    holyhermes = 1 << 2,
    maguszeus = 1 << 3,
    holykronos = 1 << 4,
    magusdiana = 1 << 5,
    holydiana = 1 << 6,
    stonecross = 1 << 7,
    oakstaff = 1 << 8,
    staffofwisdom = 1 << 9
}


    #endregion

#region Daggers

[Flags]
public enum Daggers
{
    None = 0, 
    snowdagger = 1,
    centerdagger = 1 << 2,
    blossomdagger = 1 << 3,
    curveddagger = 1 << 4,
    moondagger = 1 << 5,
    lightdagger = 1 << 6,
    sundagger = 1 << 7,
    lotusdagger = 1 << 8,
    blooddagger = 1 << 9,
    nagetierdagger = 1 << 10
}
    #endregion

#region Claws

[Flags]
public enum Claws
{
    None = 0,
    dullclaw = 1,
    wolfclaw = 1 << 2,
    eagletalon = 1 << 3,
    phoenixclaw = 1 << 4,
    nunchaku = 1 << 5,
}

    #endregion

#region Shields

[Flags]
public enum Shields
{
    None = 0,
    woodenshield = 1,
    leathershield = 1 << 2,
    bronzeshield = 1 << 3,
    gravelshield = 1 << 4,
    ironshield = 1 << 5,
    lightshield = 1 << 6,
    mythrilshield = 1 << 7,
    hybrasylshield = 1 << 8,
}

    #endregion

    #endregion
[Flags]
public enum EnchantingRecipes
{
    None = 0,
    MiraelisEmbrace = 1,
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