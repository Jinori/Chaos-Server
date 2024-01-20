using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

public class NightmareTeammateScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(TeammateFollowScript)),
        GetScriptKey(typeof(TeammateAggroTargetingScript)),
        GetScriptKey(typeof(TeammateMoveToTargetScript)),
        GetScriptKey(typeof(TeammateAttackingScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(TeammateDeathScript)),
        GetScriptKey(typeof(DisplayNameScript))
    };

    /// <inheritdoc />
    public NightmareTeammateScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}