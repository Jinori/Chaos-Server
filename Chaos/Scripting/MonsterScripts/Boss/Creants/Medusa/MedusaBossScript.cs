using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Medusa;

public class MedusaBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(ThisIsABossScript)),
        GetScriptKey(typeof(CreantDeathScript)),
        GetScriptKey(typeof(MedusaPhaseScript)),
        GetScriptKey(typeof(MedusaMoveToTargetScript)),
        GetScriptKey(typeof(MedusaCastingScript)),
        GetScriptKey(typeof(MedusaAttackingScript))
    };

    /// <inheritdoc />
    public MedusaBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}