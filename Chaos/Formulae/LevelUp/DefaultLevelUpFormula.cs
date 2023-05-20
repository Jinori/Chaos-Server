using Chaos.Formulae.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;

namespace Chaos.Formulae.LevelUp;

public class DefaultLevelUpFormula : ILevelUpFormula
{
    /// <inheritdoc />
    public Attributes CalculateAttributesIncrease(Aisling aisling) =>
        new()
        {
            //each level, add (Level * 0.3) + 10 hp
            MaximumHp = Convert.ToInt32(50),
            //each level, add (Level * 0.15) + 5 mp
            MaximumMp = Convert.ToInt32(40),
            //every 3 levels, subtract 1 ac
            Ac = aisling.StatSheet.Level % 3 == 0 ? -1 : 0
        };

    /// <inheritdoc />
    public int CalculateMaxWeight(Aisling aisling) => 40 + aisling.UserStatSheet.Level / 2 + aisling.UserStatSheet.Str;

    /// <inheritdoc />
    public int CalculateTnl(Aisling aisling) => Convert.ToInt32(Math.Pow(aisling.UserStatSheet.Level, 3.4) * 450 / 140 + 2500 * aisling.UserStatSheet.Level);
}