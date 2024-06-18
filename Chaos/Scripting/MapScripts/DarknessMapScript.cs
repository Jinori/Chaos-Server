using System.Reactive.Subjects;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class DarknessMapScript : MapScriptBase
{
    public DarknessMapScript(MapInstance subject) : base(subject)
    {
        Subject.Flags |= MapFlags.Darkness;
        Subject.CurrentLightLevel = LightLevel.Darkest_A;
    }
}