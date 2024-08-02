using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

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

            if (options.IsPetSap && context.Source is Monster monster1 && context.Source.Script.Is<PetScript>())
            {
                if (monster1.PetOwner != null)
                    ApplyManaReplenish(
                        monster1.PetOwner,
                        baseReplenish,
                        percentageBasedReplenish,
                        context.Source,
                        options);

                return;
            }
            
            if (options.ReplenishGroup)
            {
                IEnumerable<Aisling>? group;
                if (context.Source is Monster monster && context.Source.Script.Is<PetScript>())
                    group = monster.PetOwner?.Group?.Where(x => x.WithinRange(target));
                else
                    group = context.SourceAisling?.Group?.Where(x => x.WithinRange(target));
                
                if (group != null)
                    foreach (var member in group)
                        ApplyManaReplenish(member, baseReplenish, percentageBasedReplenish, context.Source, options);
                else
                    ApplyManaReplenish(context.Source, baseReplenish, percentageBasedReplenish, context.Source, options);

                return;
            }
        
            ApplyManaReplenish(context.Source, baseReplenish, percentageBasedReplenish, context.Source, options);
        }
    }

    private void ApplyManaReplenish(Creature creature, int baseReplenish, int percentageBasedReplenish, Creature source, ISapNeedleComponentOptions options)
    {
        var maxReplenishAllowed = (int)(creature.StatSheet.EffectiveMaximumMp * 0.20);
        var finalReplenish = Math.Min(baseReplenish + percentageBasedReplenish, maxReplenishAllowed);
        creature.StatSheet.AddMp(finalReplenish);
        
        if (creature is Aisling aisling)
        {
            aisling.Client.SendAttributes(StatUpdateType.Vitality);

            aisling.SendOrangeBarMessage(
                options.IsPetSap
                    ? $"Pet licked {finalReplenish} mana."
                    : $"{source.Name} replenished {finalReplenish} mana.");
        }
        
        creature.Animate(Sap);
    }


    public interface ISapNeedleComponentOptions
    {
        bool IsPetSap { get; init; }
        int? ManaReplenish { get; init; }
        decimal PctManaReplenish { get; init; }
        bool ReplenishGroup { get; init; }
    }
}