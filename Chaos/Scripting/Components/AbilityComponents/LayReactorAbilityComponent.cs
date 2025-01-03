#region
using Chaos.Models.Data;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct LayReactorAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ILayReactorComponentOptions>();
        var sourceScript = vars.GetSourceScript();
        var map = context.TargetMap;

        if (string.IsNullOrEmpty(options.ReactorTileTemplateKey))
            return;

        var points = vars.GetPoints();

        foreach (var point in points)
        {
            var trap = options.ReactorTileFactory.Create(
                options.ReactorTileTemplateKey,
                map,
                point,
                owner: context.Source,
                sourceScript: sourceScript);

            map.SimpleAdd(trap);
        }
    }

    public interface ILayReactorComponentOptions
    {
        IReactorTileFactory ReactorTileFactory { get; init; }
        string? ReactorTileTemplateKey { get; init; }
    }
}