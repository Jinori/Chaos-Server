using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Elements;

public class DefenseElementScript : ConfigurableMonsterScriptBase
{
    #region ScriptVars
    public Element Element { get; init; }
    #endregion

    /// <inheritdoc />
    public DefenseElementScript(Monster subject)
        : base(subject) =>
        Subject.StatSheet.SetDefenseElement(Element);
}