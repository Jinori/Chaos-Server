namespace Chaos.Extensions;

public static class EffectsToPersistExtension
{
    private static readonly string[] CoreEffects =
    [
        "Arena Revive",
        "ValentinesCandy",
        "GMKnowledge",
        "Strong Knowledge",
        "Knowledge",
        "DropBoost",
        "Miracle"
    ];

    // All general persistent buffs (stat boosts, foods, potions, etc.)
    private static readonly string[] PersistentBuffs =
    [
        "Hot Chocolate",
        "Invulnerability",
        "Mount",
        "Werewolf",
        "Fishing",
        "Foraging",
        "Celebration",
        "Marriage",
        "DmgTrinket",
        "Prevent Recradh",
        "Strong Stat Boost",
        "Stat Boost",
        "Strength Potion",
        "Intellect Potion",
        "Wisdom Potion",
        "Constitution Potion",
        "Dexterity Potion",
        "Small Haste", "Haste", "Strong Haste", "Potent Haste",
        "Small Power", "Power", "Strong Power", "Potent Power",
        "Small Accuracy", "Accuracy", "Strong Accuracy", "Potent Accuracy",
        "Juggernaut", "Strong Juggernaut", "Potent Juggernaut",
        "Astral", "Strong Astral", "Potent Astral",
        "Poison Immunity",
        "Dinner Plate",
        "Sweet Buns",
        "Fruit Basket",
        "Lobster Dinner",
        "Pie Acorn",
        "Pie Apple",
        "Pie Cherry",
        "Pie Grape",
        "PieGreengrapes",
        "Pie Strawberry",
        "Pie Tangerines",
        "Salad",
        "Sandwich",
        "Soup",
        "Steak Meal"
    ];
    
    private static readonly string[] PowerLimiters =
    [
        "Power Limit",
        "Creant Power Limit"
    ];

    // Additional temporary enhancements protected from AoSith
    private static readonly string[] FuryEffects =
    [
        "Fury1", "Fury2", "Fury3", "Fury4", "Fury5", "Fury6",
        "Cunning1", "Cunning2", "Cunning3", "Cunning4", "Cunning5", "Cunning6",
        "Stoned" //Medusa Stoned
    ];

    public static readonly HashSet<string> EffectsToPersistOnHostedArenaDeath = [..CoreEffects];

    public static readonly HashSet<string> EffectsToPersistOnDeath =
    [
        ..CoreEffects.Concat(PersistentBuffs)
    ];

    public static readonly HashSet<string> EffectsToPersistThroughAoSith =
    [
        ..CoreEffects.Concat(PersistentBuffs).Concat(FuryEffects).Concat(PowerLimiters)
    ];
}