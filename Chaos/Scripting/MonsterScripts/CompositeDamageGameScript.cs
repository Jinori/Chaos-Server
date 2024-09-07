using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;

namespace Chaos.Scripting.MonsterScripts;

public class CompositeDamageGameScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys =
    [
        GetScriptKey(typeof(DamageGameScript)),
        GetScriptKey(typeof(ThisIsABossScript))
    ];

    /// <inheritdoc />
    public CompositeDamageGameScript(IScriptProvider scriptProvider, Monster subject)
    {
        if (scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) is not CompositeMonsterScript compositeScript)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var script in compositeScript)
            Add(script);
    }
}