#region
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct ApplyEffectAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IApplyEffectComponentOptions>();

        if (string.IsNullOrEmpty(options.EffectKey))
            return;

        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            if (options.EffectApplyChance.HasValue && !IntegerRandomizer.RollChance(options.EffectApplyChance.Value))
                continue;

            var effect = options.EffectFactory.Create(options.EffectKey);

            SetEffectVars(effect, target, vars);

            if (options.EffectDurationOverride.HasValue)
                effect.SetDuration(options.EffectDurationOverride.Value);

            target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
        }
    }

    private void SetEffectVars(IEffect effect, Creature target, ComponentVars vars)
    {
        var finalDamage = vars.GetFinalDamage(target);
        
        switch (effect.Name.ToLower())
        {
            case "burn":
            {
                var damagePerTick = (int)(finalDamage * 0.25m);
                effect.SetVar("dmgPerTick", damagePerTick);
                
                break;
            }
            case "salsplash":
            {
                var damagePerTick = (int)(finalDamage * 0.25m);
                effect.SetVar("dmgPerTick", damagePerTick);
                
                break;
            }
        }
    }

    public interface IApplyEffectComponentOptions
    {
        int? EffectApplyChance { get; init; }
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
    }
}