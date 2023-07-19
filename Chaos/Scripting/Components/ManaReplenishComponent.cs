using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ManaReplenishComponent : IComponent
{
    private readonly Animation Sap = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 61
    };
    
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IManaReplenishComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        
        var replenish = options.ManaReplenish ?? 0;

        foreach (var target in targets)
        {
            var finalReplenish = replenish + MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, options.PctManaReplenish);
            
            if (options.ReplenishGroup)
            {
                var group = context.SourceAisling?.Group;

                if (group != null)
                    foreach (var member in group)
                    {
                        member.StatSheet.AddMp(finalReplenish);
                        member.Client.SendAttributes(StatUpdateType.Vitality);
                        member.Animate(Sap);
                    }

                return;
            }
            
            context.Source.StatSheet.AddMp(finalReplenish);
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
        }
    }

    public interface IManaReplenishComponentOptions
    {
        int? ManaReplenish { get; init; }
        decimal PctManaReplenish { get; init; }
        bool ReplenishGroup { get; init; }
    }
}