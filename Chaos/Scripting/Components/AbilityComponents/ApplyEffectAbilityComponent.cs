using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Services.Factories.Abstractions;

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

            //bosses cannot be beagsuained by creag spells
            if (options.EffectKey is "beagsuain" && target.Script.Is<ThisIsABossScript>())
                continue;
            
            var effect = options.EffectFactory.Create(options.EffectKey);

            if (options.EffectDurationOverride.HasValue)
                effect.SetDuration(options.EffectDurationOverride.Value);

            target.Effects.Apply(context.Source, effect);
        }
    }

    public interface IApplyEffectComponentOptions
    {
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
        int? EffectApplyChance { get; init; }
    }
}