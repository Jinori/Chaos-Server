using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Elements;

public class OffenseElementScript : ConfigurableMonsterScriptBase
{
    #region ScriptVars
    public Element Element { get; init; }
    #endregion

    /// <inheritdoc />
    public OffenseElementScript(Monster subject)
        : base(subject) =>
        Subject.StatSheet.SetOffenseElement(Element);
}