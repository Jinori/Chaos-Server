using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

public sealed class TeammateDeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TeammateDeathScript(Monster subject)
        : base(subject) { }

    public override void OnDeath()
    {
        Subject.PetOwner = null;
        Map.RemoveEntity(Subject);

        if (Subject.TryDropGold(Subject, Subject.Gold, out var money))
            Subject.PetOwner?.SendActiveMessage($"Your pet died and dropped {money?.Amount} gold!");
    }
}