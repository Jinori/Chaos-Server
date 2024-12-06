#region
using Chaos.Common.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

/// <summary>
///     A component that creates a cascade of effects
/// </summary>
public class CascadingComponent<TTileScript> : IComponent where TTileScript: ICascadingTileScript
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ICascadingComponentOptions>();
        var targets = vars.GetTargets<MapEntity>();
        var points = vars.GetPoints();

        var cascadePoints = options.CascadeOnlyFromEntities ? targets.Select(Point.From) : points;
        var scriptKey = ScriptBase.GetScriptKey(typeof(TTileScript));

        foreach (var point in cascadePoints)
        {
            if (!context.TargetMap.IsWithinMap(point))
                continue;
            
            var tile = options.ReactorTileFactory.Create(
                context.TargetMap,
                point,
                false,
                [scriptKey],
                options.CascadeScriptVars,
                context.Source,
                options.SourceScript);

            context.TargetMap.SimpleAdd(tile);
        }
    }

    public interface ICascadingComponentOptions
    {
        bool CascadeOnlyFromEntities { get; init; }
        IDictionary<string, IScriptVars> CascadeScriptVars { get; init; }
        IReactorTileFactory ReactorTileFactory { get; init; }
        
        public IScript SourceScript { get; init; }
    }
}