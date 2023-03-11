using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components;

public class ManaBasedDamageComponent
{

    protected virtual int CalculateDamage(
        ActivationContext context,
        IManaBasedDamageComponentOptions options
    )
    {
        var baseDamage = options.BaseDamage ?? 0;
        var manaDamage = 0;

        if (options.BaseDamageMultiplier.HasValue)
            baseDamage = Convert.ToInt32(baseDamage * options.BaseDamageMultiplier.Value);

        if (options.PctOfMana.HasValue)
            manaDamage = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, options.PctOfMana.Value);

        if (options.PctOfManaMultiplier.HasValue)
            manaDamage = Convert.ToInt32(manaDamage * options.PctOfManaMultiplier.Value);

        var finalDamage = baseDamage + manaDamage;

        if (options.FinalMultiplier.HasValue)
            finalDamage = Convert.ToInt32(finalDamage * options.FinalMultiplier.Value);

        return finalDamage;
    }

    public virtual void ApplyDamage(
        ActivationContext context,
        IReadOnlyCollection<Creature> targetEntities,
        IManaBasedDamageComponentOptions options
    )
    {
        var damage = CalculateDamage(context, options);

        if (damage <= 0)
            return;

        foreach (var target in targetEntities)
            options.ApplyDamageScript.ApplyDamage(
                context.Source,
                target,
                options.SourceScript,
                damage,
                options.Element);
    }

    public interface IManaBasedDamageComponentOptions
    {
        
        IScript SourceScript { get; }
        IApplyDamageScript ApplyDamageScript { get; }
        int? BaseDamage { get; }
        decimal? BaseDamageMultiplier { get; }
        decimal? PctOfMana { get; }
        decimal? PctOfManaMultiplier { get; }
        decimal? FinalMultiplier { get; }
        
        Element? Element { get; }
        
    }
}