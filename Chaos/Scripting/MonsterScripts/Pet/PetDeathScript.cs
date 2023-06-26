using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public sealed class PetDeathScript : MonsterScriptBase
{
    public override void OnDeath()
    {
        Subject.Say("*slosh*");
        Map.RemoveObject(Subject);
        
        if (Subject.TryDropGold(Subject, Subject.Gold, out var money)) 
            Subject.PetOwner?.SendActiveMessage($"Your pet died and dropped {money?.Amount} gold!");
    }
    
    /// <inheritdoc />
    public PetDeathScript(Monster subject)
        : base(subject) { }
}