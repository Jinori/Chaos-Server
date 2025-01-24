using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct MultistrikeAbilityComponent<TEntity> : IConditionalComponent where TEntity: Creature
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
        => new ComponentExecutor(context, vars).ExecuteAndCheck<ManaCostAbilityComponent>()
                                               ?.Execute<BreaksHideAbilityComponent>()
                                               .ExecuteAndCheck<GetMultistrikeTargetsAbilityComponent<TEntity>>()
                                               ?.Execute<BodyAnimationAbilityComponent>()
                                               .Execute<AnimationAbilityComponent>()
                                               .Execute<SoundAbilityComponent>()
                                               .Execute<CooldownComponent>()
           != null;

    public interface IAbilityComponentOptions : GetMultistrikeTargetsAbilityComponent<TEntity>.IGetMultistrikeTargetsOptions,
                                                SoundAbilityComponent.ISoundComponentOptions,
                                                BodyAnimationAbilityComponent.IBodyAnimationComponentOptions,
                                                AnimationAbilityComponent.IAnimationComponentOptions,
                                                ManaCostAbilityComponent.IManaCostComponentOptions,
                                                BreaksHideAbilityComponent.IBreaksHideComponentOptions { }
}