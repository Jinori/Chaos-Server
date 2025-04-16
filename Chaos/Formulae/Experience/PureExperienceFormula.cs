using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Formulae.Experience;

/// <inheritdoc />
public sealed class PureExperienceFormula : IExperienceFormula
{
    /// <inheritdoc />
    public long Calculate(Creature killedCreature, params ICollection<Aisling> aislings)
        => killedCreature is Monster monster ? monster.Experience : 0;
}