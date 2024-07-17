using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CR_Bosses.SummonerKades;

public class SummonerKadesBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(KadesMoveToTargetScript)),
        GetScriptKey(typeof(SummonerKadesFleeScript)),
        GetScriptKey(typeof(SummonerKadesEnrageScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(KadesCastingScript)),
        GetScriptKey(typeof(KadesAttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(BossDefenseScript)),
        GetScriptKey(typeof(ThisIsABossScript)),
        GetScriptKey(typeof(DeathScript)),
    };

    /// <inheritdoc />
    public SummonerKadesBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}