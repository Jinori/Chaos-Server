using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

public class TaurenBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(TaurenMoveToTargetScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(TaurenCastingScript)),
        GetScriptKey(typeof(TaurenAttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(BossDefenseScript)),
        GetScriptKey(typeof(ThisIsABossScript)),
        GetScriptKey(typeof(CreantDeathScript)),
        GetScriptKey(typeof(TaurenPhaseScript))
    };

    /// <inheritdoc />
    public TaurenBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}