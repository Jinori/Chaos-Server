using Chaos.Data;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Formulae.LevelUp;

public class DefaultLevelUpFormula : ILevelUpFormula
{
    /// <inheritdoc />
    public Attributes CalculateAttributesIncrease(Aisling aisling) =>
        new()
        {
            //each level, add (Level * 0.3) + 10 hp
            MaximumHp = Convert.ToInt32((aisling.UserStatSheet.Con * 35 + aisling.UserStatSheet.Str * 20) / (aisling.UserStatSheet.Level)) + 50,
            //each level, add (Level * 0.15) + 5 mp
            MaximumMp = Convert.ToInt32((aisling.UserStatSheet.Wis * 35 + aisling.UserStatSheet.Int * 20) / (aisling.UserStatSheet.Level)) + 30,
            //every 3 levels, subtract 1 ac
            Ac = aisling.StatSheet.Level % 3 == 0 ? -1 : 0
        };

    /// <inheritdoc />
    public int CalculateMaxWeight(Aisling aisling) => 40 + aisling.UserStatSheet.Level / 2 + aisling.UserStatSheet.Str;

    /// <inheritdoc />
    public int CalculateTnl(Aisling aisling) => Convert.ToInt32(Math.Pow(aisling.UserStatSheet.Level, 3.4) * 450 / 140 + 2500 * aisling.UserStatSheet.Level);
}