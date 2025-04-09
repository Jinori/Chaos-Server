using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Functional;

public class NoItemUsageScript : MapScriptBase
{
    /// <inheritdoc />
    public NoItemUsageScript(MapInstance subject)
        : base(subject) { }
    
    //This map script is tied to the Restriction Behaviors. If a map has this script, players won't be able to use items on this map.
}