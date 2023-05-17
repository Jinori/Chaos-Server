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
    LastStand = 1 << 23
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
    MonstersOnly = 1 << 5
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

public enum MeatsStage
{
    None = 0,
    beefslices = 1,
    beef = 2,
    chicken = 3,
    clam = 4,
    trout = 5,
    bass = 6,
    egg = 7,
    liver = 8,
    lobstertail = 9,
    rawmeat = 10
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
    greenGrapes = 5,
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

