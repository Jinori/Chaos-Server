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


