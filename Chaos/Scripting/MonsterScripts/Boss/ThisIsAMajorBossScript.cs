using Chaos.Models.World;

namespace Chaos.Scripting.MonsterScripts.Boss;

public class ThisIsAMajorBossScript : ThisIsABossScript
{
    /// <inheritdoc />
    public ThisIsAMajorBossScript(Monster subject)
        : base(subject) { }

    //This Script only serves the purpose to tag all creatures in game via code as bosses
}