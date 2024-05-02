using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class SplashComponent<TEntity> : IConditionalComponent where TEntity : MapEntity
{
    /// <inheritdoc />
    public virtual bool Execute(ActivationContext context, ComponentVars vars)
    {
        // Getting necessary options
        var options = vars.GetOptions<ISplashComponentOptions>();

        // Getting targets
        var targets = vars.GetTargets<TEntity>().ToList();

        // Check each target for nearby creatures
        foreach (var target in targets.ToList())
        {
            var localSplashTargets = context.TargetMap.GetEntitiesWithinRange<TEntity>(target, options.SplashDistance)
                .WithFilter(context.Source, options.SplashFilter);

            foreach(var localSplashTarget in localSplashTargets)
                if (IntegerRandomizer.RollChance(options.SplashChance))
                    targets.Add(localSplashTarget);
        }

        vars.SetTargets(targets);

        // If no targets were found, return false
        return !options.MustHaveTargets || targets.Count != 0;
    }

    public interface ISplashComponentOptions
    {
        int SplashChance { get; init; }
        int SplashDistance { get; init; }
        TargetFilter SplashFilter { get; init; }
        bool MustHaveTargets { get; init; }
    }
}