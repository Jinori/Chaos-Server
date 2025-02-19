using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class SpellComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public virtual bool Execute(ActivationContext context, ComponentVars vars)
        => new ComponentExecutor(context, vars).ExecuteAndCheck<ManaCostAbilityComponent>()
                                               ?.Execute<BreaksSpecificEffectsAbilityComponent>()
                                               .ExecuteAndCheck<GetTargetsAbilityComponent<TEntity>>()
                                               ?.ExecuteAndCheck<SplashComponent<TEntity>>()
                                               ?.Execute<MagicResistanceComponent>()
                                               .Execute<CooldownComponent>()
                                               .Execute<BodyAnimationAbilityComponent>()
                                               .Execute<AnimationAbilityComponent>()
                                               .Execute<SoundAbilityComponent>()
                                               .Execute<RemoveShamBurningGroundComponent>()
           != null;

    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface ISpellComponentOptions : GetTargetsAbilityComponent<TEntity>.IGetTargetsComponentOptions,
                                              SplashComponent<TEntity>.ISplashComponentOptions,
                                              MagicResistanceComponent.IMagicResistanceComponentOptions,
                                              SoundAbilityComponent.ISoundComponentOptions,
                                              BodyAnimationAbilityComponent.IBodyAnimationComponentOptions,
                                              AnimationAbilityComponent.IAnimationComponentOptions,
                                              ManaCostAbilityComponent.IManaCostComponentOptions,
                                              BreaksSpecificEffectsAbilityComponent.IBreaksSpecificEffectsComponentOptions { }
}