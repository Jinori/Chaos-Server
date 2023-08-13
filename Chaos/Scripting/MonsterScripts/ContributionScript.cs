using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class ContributionScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ContributionScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        if (!Subject.Contribution.TryGetValue(source.Id, out var currentValue))
            currentValue = 0;

        if (source is Monster { PetOwner: not null } monster)
            Subject.Contribution[monster.PetOwner.Id] = currentValue + damage;
        else
            Subject.Contribution[source.Id] = currentValue + damage;
    }
}