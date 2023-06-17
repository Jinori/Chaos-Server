using Chaos.Collections;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class MythicDirectoryScript : MapScriptBase
{
    private readonly ISimpleCache SimpleCache;
    
    public MythicDirectoryScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;
}