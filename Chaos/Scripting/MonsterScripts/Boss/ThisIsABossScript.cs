using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public class ThisIsABossScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ThisIsABossScript(Monster subject)
        : base(subject) { }

    //This Script only serves the purpose to tag all creatures in game via code as bosses
}