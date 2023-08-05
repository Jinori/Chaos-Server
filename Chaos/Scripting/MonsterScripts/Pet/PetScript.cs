using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public class PetScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(PetFollowScript)),
        GetScriptKey(typeof(PetAggroTargetingScript)),
        GetScriptKey(typeof(PetMoveToTargetScript)),
        GetScriptKey(typeof(PetAttackingScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(PetDeathScript))
    };

    /// <inheritdoc />
    public PetScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript script)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var component in script)
            Add(component);
    }
}