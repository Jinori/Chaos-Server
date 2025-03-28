using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Halloween;

public class CountessBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(MoveToTargetScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(CountessEnrageScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(CountDeathScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(ThisIsAWorldBossScript))
    };

    //If you are not using BossMoveToTargetScript, you need: MoveToTargetScript.
    /// <inheritdoc />
    public CountessBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}