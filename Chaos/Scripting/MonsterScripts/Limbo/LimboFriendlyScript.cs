using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo;

//marker script to make limbo units friendly to eachother
public class LimboFriendlyScript : MonsterScriptBase
{
    /// <inheritdoc />
    public LimboFriendlyScript(Monster subject)
        : base(subject) { }
}