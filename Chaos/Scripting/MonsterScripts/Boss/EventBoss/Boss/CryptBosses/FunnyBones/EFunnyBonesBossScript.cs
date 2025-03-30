using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CryptBosses.FunnyBones;

public class EFunnyBonesBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(BossMoveToTargetScript)),
        GetScriptKey(typeof(EFunnyBonesEnrageScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(EventMonsterScalingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DeathScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(ThisIsABossScript))
    };

    /// <inheritdoc />
    public EFunnyBonesBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}