using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.MonsterScripts.Pet;

namespace Chaos.Scripting.Components;

public class SapNeedleComponent : IComponent
{
    private readonly Animation Sap = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 61
    };

    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ISapNeedleComponentOptions>();
        var targets = vars.GetTargets<Creature>();
    
        foreach (var target in targets)
        {
            var baseReplenish = options.ManaReplenish ?? 0;
            var percentageBasedReplenish = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, options.PctManaReplenish);

            if (options.ReplenishGroup)
            {
                IEnumerable<Aisling>? group;
                if (context.Source is Monster monster && context.Source.Script.Is<PetScript>())
                    group = monster.PetOwner?.Group?.Where(x => x.WithinRange(target));
                else
                    group = context.SourceAisling?.Group?.Where(x => x.WithinRange(target));
                
                if (group != null)
                    foreach (var member in group)
                        ApplyManaReplenish(member, baseReplenish, percentageBasedReplenish, context.Source);
                else
                    ApplyManaReplenish(context.Source, baseReplenish, percentageBasedReplenish, context.Source);

                return;
            }
        
            ApplyManaReplenish(context.Source, baseReplenish, percentageBasedReplenish, context.Source);
        }
    }

    private void ApplyManaReplenish(Creature creature, int baseReplenish, int percentageBasedReplenish, Creature source)
    {
        var maxReplenishAllowed = (int)(creature.StatSheet.EffectiveMaximumMp * 0.20);
        var finalReplenish = Math.Min(baseReplenish + percentageBasedReplenish, maxReplenishAllowed);
        creature.StatSheet.AddMp(finalReplenish);

        if (creature is Aisling aisling)
        {
            aisling.Client.SendAttributes(StatUpdateType.Vitality);
            aisling.SendOrangeBarMessage($"{source.Name} repenished {finalReplenish} mana through Sap Needle.");
        }
        
        creature.Animate(Sap);
    }


    public interface ISapNeedleComponentOptions
    {
        int? ManaReplenish { get; init; }
        decimal PctManaReplenish { get; init; }
        bool ReplenishGroup { get; init; }
    }
}