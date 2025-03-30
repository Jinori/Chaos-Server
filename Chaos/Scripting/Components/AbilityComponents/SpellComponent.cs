using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class SpellComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public virtual bool Execute(ActivationContext context, ComponentVars vars)
        => new ComponentExecutor(context, vars).ExecuteAndCheck<HealthCostAbilityComponent>()
                                               ?.ExecuteAndCheck<ManaCostAbilityComponent>()
                                               ?.Execute<BreaksSpecificEffectsAbilityComponent>()
                                               .ExecuteAndCheck<GetTargetsAbilityComponent<TEntity>>()
                                               ?.Execute<RemoveShamBurningGroundComponent>()
                                               .ExecuteAndCheck<SplashComponent<TEntity>>()
                                               ?.Execute<CooldownComponent>()
                                               .ExecuteAndCheck<MagicResistanceComponent>()
                                               ?.Execute<BodyAnimationAbilityComponent>()
                                               .Execute<AnimationAbilityComponent>()
                                               .Execute<SoundAbilityComponent>()
           != null;

    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface ISpellComponentOptions : GetTargetsAbilityComponent<TEntity>.IGetTargetsComponentOptions,
                                              SplashComponent<TEntity>.ISplashComponentOptions,
                                              MagicResistanceComponent.IMagicResistanceComponentOptions,
                                              SoundAbilityComponent.ISoundComponentOptions,
                                              BodyAnimationAbilityComponent.IBodyAnimationComponentOptions,
                                              AnimationAbilityComponent.IAnimationComponentOptions,
                                              HealthCostAbilityComponent.IHealthCostComponentOptions,
                                              ManaCostAbilityComponent.IManaCostComponentOptions,
                                              BreaksSpecificEffectsAbilityComponent.IBreaksSpecificEffectsComponentOptions { }
}