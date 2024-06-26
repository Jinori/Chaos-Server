using Chaos.Models.Data;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.Abstractions;

public interface IConditionalComponent
{
    bool Execute(ActivationContext context, ComponentVars vars);
}