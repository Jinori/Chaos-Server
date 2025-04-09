using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Functional;

public class NoSkillSpellUsageScript : MapScriptBase
{
    /// <inheritdoc />
    public NoSkillSpellUsageScript(MapInstance subject)
        : base(subject) { }
    
    //This map script is tied to the Restriction Behaviors. If a map has this script, players won't be able to use skills or spells on this map.
}