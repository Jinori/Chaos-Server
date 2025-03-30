using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripting.Components.AbilityComponents;

public class SapNeedleComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ISapNeedleComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var sapTarget = options.SapTarget;
        var damageScript = ApplyAttackDamageScript.Create();
        var baseReplenish = options.ManaReplenish ?? 0;
        var percentageBasedReplenish = MathEx.GetPercentOf<int>((int)sapTarget.StatSheet.EffectiveMaximumMp, options.PctManaReplenish);

        foreach (var target in targets)
            ApplyManaReplenish(
                target,
                baseReplenish,
                percentageBasedReplenish,
                context.Source,
                options);

        if (options.DealDamageEqualToSappedMana)
        {
            var maxReplenishAllowed = (int)(context.Source.StatSheet.EffectiveMaximumMp * 0.20);
            var finalReplenish = Math.Min(baseReplenish + percentageBasedReplenish, maxReplenishAllowed);

            damageScript.ApplyDamage(
                context.Source,
                sapTarget,
                vars.GetSourceScript(),
                finalReplenish);
        }
    }

    private void ApplyManaReplenish(
        Creature creature,
        int baseReplenish,
        int percentageBasedReplenish,
        Creature source,
        ISapNeedleComponentOptions options)
    {
        var maxReplenishAllowed = (int)(creature.StatSheet.EffectiveMaximumMp * 0.20);
        var finalReplenish = Math.Min(baseReplenish + percentageBasedReplenish, maxReplenishAllowed);
        creature.StatSheet.AddMp(finalReplenish);

        if (creature is Aisling aisling)
        {
            aisling.Client.SendAttributes(StatUpdateType.Vitality);

            aisling.SendOrangeBarMessage(
                options.IsPetSap ? $"Pet licked {finalReplenish} mana." : $"{source.Name} replenished {finalReplenish} mana.");
        }
    }

    public interface ISapNeedleComponentOptions
    {
        bool DealDamageEqualToSappedMana { get; init; }
        bool IsPetSap { get; init; }
        int? ManaReplenish { get; init; }
        decimal PctManaReplenish { get; init; }
        Creature SapTarget { get; set; }
    }
}