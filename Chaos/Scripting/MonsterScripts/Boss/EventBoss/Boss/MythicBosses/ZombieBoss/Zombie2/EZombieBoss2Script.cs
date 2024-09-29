using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss.MythicBosses.ZombieBoss.Zombie2;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.MythicBosses.ZombieBoss.Zombie2;

public class EZombieBoss2Script : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(BossDefenseScript)),
        GetScriptKey(typeof(EZombieBoss2EnrageScript)),
        GetScriptKey(typeof(EventMonsterScalingScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(MoveToTargetScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(ArenaDeathScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(ThisIsABossScript))
    };

    /// <inheritdoc />
    public EZombieBoss2Script(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}