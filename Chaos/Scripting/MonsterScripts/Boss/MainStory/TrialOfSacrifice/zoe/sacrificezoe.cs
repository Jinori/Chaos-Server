using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.TrialOfSacrifice.zoe;

public class SacrificeZoe : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(ZoeDeathScript)),
        GetScriptKey(typeof(DisplayNameScript))
    };

    //If you are not using BossMoveToTargetScript, you need: MoveToTargetScript.
    /// <inheritdoc />
    public SacrificeZoe(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}