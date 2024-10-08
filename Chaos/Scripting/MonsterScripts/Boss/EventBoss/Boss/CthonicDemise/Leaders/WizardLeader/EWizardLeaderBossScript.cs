using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss.CthonicDemise.Leaders.PriestLeader;
using Chaos.Scripting.MonsterScripts.Boss.CthonicDemise.Leaders.WizardLeader;
using Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CthonicDemise.Leaders.PriestLeader;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CthonicDemise.Leaders.WizardLeader;

public class EWizardLeaderBossScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(EWizardLeaderDefenseMoveToTargetScript)),
        GetScriptKey(typeof(EPriestEnrageSummonScript)),
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(EventMonsterScalingScript)),
        GetScriptKey(typeof(ContributionScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(ArenaDeathScript)),
        GetScriptKey(typeof(DisplayNameScript)),
        GetScriptKey(typeof(ThisIsABossScript))
    };

    //If you are not using BossMoveToTargetScript, you need: MoveToTargetScript.
    /// <inheritdoc />
    public EWizardLeaderBossScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}