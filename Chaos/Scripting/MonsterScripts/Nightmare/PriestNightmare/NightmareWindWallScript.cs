using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;

public class NightmareWindWallScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(DefaultBehaviorsScript)),
        GetScriptKey(typeof(DisplayNameScript))
    };

    //If you are not using BossMoveToTargetScript, you need: MoveToTargetScript.
    /// <inheritdoc />
    public NightmareWindWallScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}