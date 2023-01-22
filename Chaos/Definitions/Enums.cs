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
    DetectTraps = 1 << 12
    //add more statuses here, double each time
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
    DeadOnly = 1 << 3
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
    StartedGreatBatsWing = 6,
    StartedWhiteBatsWing = 7
}