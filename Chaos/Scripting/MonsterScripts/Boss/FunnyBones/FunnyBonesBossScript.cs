using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.FunnyBones
{
    public class FunnyBonesBossScript : CompositeMonsterScript
    {
        private static readonly ICollection<string> ScriptKeys = new[]
        {
            GetScriptKey(typeof(BossMoveToTargetScript)),
            GetScriptKey(typeof(FunnyBonesEnrageScript)),
            GetScriptKey(typeof(AggroTargetingScript)),
            GetScriptKey(typeof(ContributionScript)),
            GetScriptKey(typeof(CastingScript)),
            GetScriptKey(typeof(AttackingScript)),
            GetScriptKey(typeof(WanderingScript)),
            GetScriptKey(typeof(DeathScript))
        };

        /// <inheritdoc />
        public FunnyBonesBossScript(IScriptProvider scriptProvider, Monster subject)
        {
            if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
                throw new InvalidOperationException("Unable to create componentized script");

            foreach (var script in compositeScript)
                Add(script);
        }
    }
}