using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class LimboLightScript : MapScriptBase
{
    /// <inheritdoc />
    public LimboLightScript(MapInstance subject)
        : base(subject) =>
        Subject.CurrentLightLevel = LightLevel.Dark_A;
}