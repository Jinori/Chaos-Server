#pragma warning disable CS1591
namespace Chaos.Common.Definitions;

[Flags]
public enum WizardElement : byte
{
    None = 0,
    Fire = 1 << 1,
    Earth = 1 << 2,
    Wind = 1 << 3,
    Water = 1 << 4,
    Removed = 1 << 5
}

[Flags]
public enum SpellSchools : byte
{
    None = 0,
    Fire = 1 << 1,
    Earth = 1 << 2,
    Wind = 1 << 3,
    Water = 1 << 4,
    Healing = 1 << 5,
    Status = 1 << 6
}


public enum WizardElementCounter : byte
{
    None = 0,
    Has2ndElement = 1 << 1,
    Has3rdElement = 1 << 2,
    Has4thElement = 1 << 3,
}

public enum RandomizationType
{
    Balanced = 0,
    Positive = 1,
    Negative = 2
}

[Flags]

public enum ChiAnkletFlags : ulong
{
    Ac1 = 0,
    Ac2 = 1 << 1,
    Ac3 = 1 << 2,
    Ac4 = 1 << 3,
    Ac5 = 1 << 4,
    Ac6 = 1 << 5,
    Ac7 = 1 << 6,
    Ac8 = 1 << 7,
    Ac9 = 1 << 8,

    Block1 = 1 << 9,
    Block2 = 1 << 10,
    Block3 = 1 << 11,
    Block4 = 1 << 12,
    Block5 = 1 << 13,

    DTKProc1 = 1 << 14,
    DTKProc2 = 1 << 15,
    DTKProc3 = 1 << 16,
    DTKProc4 = 1 << 17,
    DTKProc5 = 1 << 18,

    IncreaseRegen1 = 1 << 19,
    IncreaseRegen2 = 1 << 20,
    IncreaseRegen3 = 1 << 21,
    IncreaseRegen4 = 1 << 22,
    IncreaseRegen5 = 1 << 23
}

[Flags]
public enum QuestFlag1 : ulong
{
    None = 0,

    ChosenClass = 1 << 1
,
    Arms = 1 << 2,
    GatheringSticks = 1 << 3,
    SpareAStickComplete = 1 << 4,
    HeadedToBeautyShop = 1 << 5,
    TalkedToJosephine = 1 << 6,
    TerrorOfCryptHunt = 1 << 7,
    TerrorOfCryptComplete = 1 << 8,
    //add more quest flags here, double each time
}

/// <summary>
///     A 2nd set of quest flags, for when <see cref="Chaos.Common.Definitions.QuestFlag1" /> is filled up
/// </summary>
[Flags]
public enum QuestFlag2 : ulong { }

/// <summary>
///     A 3rd set of quest flags, for when <see cref="Chaos.Common.Definitions.QuestFlag2" /> is filled up
/// </summary>
[Flags]
public enum QuestFlag3 : ulong { }

public enum LootTableMode
{
    ChancePerItem,
    PickSingleOrDefault
}