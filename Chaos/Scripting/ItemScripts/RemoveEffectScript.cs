using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class RemoveEffectScript : ConfigurableItemScriptBase,
                                  GenericAbilityComponent<Aisling>.IAbilityComponentOptions,
                                  RemoveEffectComponent.IRemoveEffectComponentOptions,
                                  ConsumableAbilityComponent.IConsumableComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public string? EffectKey { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    public string ItemName { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    public bool Message { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public bool? RemoveAllEffects { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public bool SingleTarget { get; init; }
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public RemoveEffectScript(Item subject)
        : base(subject) => ItemName = Subject.DisplayName;

    public override void OnUse(Aisling source) => new ComponentExecutor(source, source).WithOptions(this)
                                                                                       .ExecuteAndCheck<GenericAbilityComponent<Aisling>>()
                                                                                       ?.Execute<RemoveEffectComponent>()
                                                                                       .Execute<ConsumableAbilityComponent>();

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
}