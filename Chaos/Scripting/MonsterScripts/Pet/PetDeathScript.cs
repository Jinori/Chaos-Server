using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public sealed class PetDeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PetDeathScript(Monster subject)
        : base(subject) { }

    public override void OnDeath()
    {
        Subject.PetOwner = null;
        Map.RemoveObject(Subject);

        if (Subject.TryDropGold(Subject, Subject.Gold, out var money))
            Subject.PetOwner?.SendActiveMessage($"Your pet died and dropped {money?.Amount} gold!");
    }
}