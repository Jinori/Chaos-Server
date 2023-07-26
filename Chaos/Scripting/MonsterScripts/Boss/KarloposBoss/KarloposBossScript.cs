using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public class KarloposBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(BossThrowHazardousScript)),
        GetScriptKey(typeof(BossMoveToTargetScript)),
        GetScriptKey(typeof(BossDefenseScript)),
        GetScriptKey(typeof(KarloposBossEnrageScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DeathScript)),
        GetScriptKey(typeof(DisplayNameScript))
    };

    /// <inheritdoc />
    public KarloposBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}