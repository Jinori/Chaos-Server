using Chaos.Collections;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Arena_Modules;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modes;

public sealed class ColorClashScript : CompositeMapScript
{
    
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(AnnounceMatchScript)),
        GetScriptKey(typeof(ColorClashDetermineWinnerScript)),
        GetScriptKey(typeof(RespawnHandlerScript))
    };

    /// <inheritdoc />
    public ColorClashScript(IScriptProvider scriptProvider, MapInstance subject)
    {
        if (scriptProvider.CreateScript<IMapScript, MapInstance>(ScriptKeys, subject) is not CompositeMapScript script)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var component in script)
            Add(component);
    }
}