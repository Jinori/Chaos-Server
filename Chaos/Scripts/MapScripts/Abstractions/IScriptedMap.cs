using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MapScripts.Abstractions;

public interface IScriptedMap : IScripted
{
    IMapScript Script { get; }
}