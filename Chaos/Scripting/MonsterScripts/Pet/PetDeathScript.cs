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
        if (Subject.PetOwner != null)
        {
            Subject.PetOwner.SkillBook.TryGetObjectByTemplateKey("summonPet", out var obj);

            if (obj != null)
                obj.Elapsed = TimeSpan.FromMinutes(5);
        }
        
        Subject.PetOwner = null;
        Map.RemoveEntity(Subject);
    }
}