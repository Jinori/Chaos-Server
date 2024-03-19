using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class ItemTargetMonsterScript : ConfigurableItemScriptBase,
                                       ConsumableComponent.IConsumableComponentOptions,
                                       AbilityComponent<Creature>.IAbilityComponentOptions,
                                       ApplyEffectComponent.IApplyEffectComponentOptions
{
    public bool AllAggro { get; set; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }

    public bool ExcludeSourcePoint { get; init; }
    public TargetFilter Filter { get; init; }
    public string ItemName { get; init; } = null!;
    public int? ManaCost { get; init; }
    public bool Message { get; init; }
    public bool MustHaveTargets { get; init; }
    public decimal PctManaCost { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool ShouldNotBreakHide { get; init; }
    public byte? Sound { get; init; }

    public ItemTargetMonsterScript(Item subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    public override void OnUse(Aisling source) =>
        new ComponentExecutor(source, source)
            .WithOptions(this)
            .ExecuteAndCheck<AbilityComponent<Creature>>()
            ?
            .Execute<ConsumableComponent>()
            .Execute<ApplyEffectComponent>();
}