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

public enum TutorialQuestFlag
{
    None = 0,
    GaveStickAndArmor = 1,
    GaveAssail = 2,
}