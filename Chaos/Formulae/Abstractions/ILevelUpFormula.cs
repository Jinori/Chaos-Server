using Chaos.Objects.World;

namespace Chaos.Formulae.Abstractions;

public interface ILevelUpFormula
{
    void LevelUp(Aisling aisling);
    int GetNewTnl(Aisling aisling);
}