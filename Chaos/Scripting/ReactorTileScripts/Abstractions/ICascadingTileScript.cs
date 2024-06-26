using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.ReactorTileScripts.Abstractions;

public interface ICascadingTileScript
{
    ComponentExecutor Executor { get; init; }
}