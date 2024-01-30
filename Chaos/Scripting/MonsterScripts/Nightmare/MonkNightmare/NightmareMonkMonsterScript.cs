using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Nightmare.MonkNightmare;

namespace Chaos.Scripting.MonsterScripts.Nightmare.RogueNightmare;

public class NightmareMonkMonsterScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(MoveToTargetScript)),
        GetScriptKey(typeof(NightmareMonkAggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(DeathScript))
    };

    //If you are not using BossMoveToTargetScript, you need: MoveToTargetScript.
    /// <inheritdoc />
    public NightmareMonkMonsterScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript
            compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}