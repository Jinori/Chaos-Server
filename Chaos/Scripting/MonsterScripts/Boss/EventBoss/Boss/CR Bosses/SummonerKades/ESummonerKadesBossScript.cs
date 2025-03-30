using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CR_Bosses.SummonerKades;

public class ESummonerKadesBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(EKadesMoveToTargetScript)),
        GetScriptKey(typeof(EventMonsterScalingScript)),
        GetScriptKey(typeof(ESummonerKadesFleeScript)),
        GetScriptKey(typeof(ESummonerKadesEnrageScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(EKadesCastingScript)),
        GetScriptKey(typeof(EKadesAttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(ThisIsABossScript)),
        GetScriptKey(typeof(ArenaDeathScript))
    };

    /// <inheritdoc />
    public ESummonerKadesBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}