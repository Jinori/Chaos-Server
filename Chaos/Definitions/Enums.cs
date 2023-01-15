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

public enum TargetFilter
{
    None,
    FriendlyOnly,
    HostileOnly
}

[Flags]
public enum TutorialQuestFlag
{
    None = 0,
    GaveStickAndArmor = 1,
    GaveAssailAndSpell = 1 << 1,
    StartedFloppy = 1 << 2,
    CompletedFloppy = 1 << 3,
    GotEquipment = 1 << 4,
    SoldCarrots = 1 << 5,
    LearnedWorld = 1 << 6,
    GiantFloppy = 1 << 7,
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
}