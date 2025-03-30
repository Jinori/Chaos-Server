using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

/// <summary>
///     Script that scales the boss monster based on the number and average level of non-admin Aislings on the map.
/// </summary>
public sealed class EventMonsterScalingScript(Monster subject) : MonsterScriptBase(subject)
{
    private static bool MapBonusApplied;

    /// <summary>
    ///     Creates bonus attributes for the boss based on the number and average level of Aislings on the map.
    /// </summary>
    private Attributes CreateBonusAttributes(int aislingCount, double averageLevel)
        => new()
        {
            Con = ((int)averageLevel + Subject.StatSheet.Level) * aislingCount / 50,
            Dex = ((int)averageLevel + Subject.StatSheet.Level) * aislingCount / 50,
            Int = ((int)averageLevel + Subject.StatSheet.Level) * aislingCount / 50,
            Str = ((int)averageLevel + Subject.StatSheet.Level) * aislingCount / 50,
            Wis = ((int)averageLevel + Subject.StatSheet.Level) * aislingCount / 50,
            AtkSpeedPct = (aislingCount + (int)averageLevel) / 4,
            MaximumHp = (int)(averageLevel + Subject.StatSheet.Level) * aislingCount * 500,
            MaximumMp = (int)(averageLevel + Subject.StatSheet.Level) * aislingCount * 500,
            SkillDamagePct = (int)(averageLevel + Subject.StatSheet.Level) * aislingCount / 75,
            SpellDamagePct = (int)(averageLevel + Subject.StatSheet.Level) * aislingCount / 75
        };

    /// <summary>
    ///     Updates the boss' stats and applies scaling based on non-admin Aislings present on the map.
    /// </summary>
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        // If the bonus has already been applied or no valid target is found, exit early.
        if (MapBonusApplied)
            return;

        // Get all non-admin Aislings on the map.
        var nonAdminAislings = Map.GetEntities<Aisling>()
                                  .Where(aisling => !aisling.IsAdmin)
                                  .Select(aisling => aisling.StatSheet.Level)
                                  .ToList();

        if (!nonAdminAislings.Any())
            return;

        // Calculate the average level and number of non-admin Aislings.
        var averageLevel = nonAdminAislings.Average();
        var aislingCount = nonAdminAislings.Count;

        // Create bonus attributes based on the Aisling count and average level.
        var bonusAttributes = CreateBonusAttributes(aislingCount, averageLevel);

        // Apply the bonus attributes to the boss and reset health/mana to 100%.
        Subject.StatSheet.AddBonus(bonusAttributes);
        Subject.StatSheet.SetHealthPct(100);
        Subject.StatSheet.SetManaPct(100);

        // Mark that the scaling has been applied.
        MapBonusApplied = true;
    }
}