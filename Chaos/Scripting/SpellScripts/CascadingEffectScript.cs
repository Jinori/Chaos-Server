using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class CascadingEffectScript : ConfigurableSpellScriptBase,
                                     GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                                     ApplyEffectAbilityComponent.IApplyEffectComponentOptions,
                                     CascadingComponent<CascadingEffectTileScript>.ICascadingComponentOptions
{
    /// <inheritdoc />
    public int? EffectApplyChance { get; init; }

    /// <inheritdoc />
    public TimeSpan? EffectDurationOverride { get; init; }

    /// <inheritdoc />
    public IEffectFactory EffectFactory { get; init; }

    /// <inheritdoc />
    public string? EffectKey { get; init; }

    /// <inheritdoc />
    public CascadingEffectScript(Spell subject, IReactorTileFactory reactorTileFactory, IEffectFactory effectFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        EffectFactory = effectFactory;
        CascadeScriptVars ??= Subject.Template.ScriptVars;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                         ?.Execute<ApplyEffectAbilityComponent>()
                                         .Execute<CascadingComponent<CascadingEffectTileScript>>();

    #region ScriptVars
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    
    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public bool CascadeOnlyFromEntities { get; init; }

    /// <inheritdoc />
    public IReactorTileFactory ReactorTileFactory { get; init; }

    /// <inheritdoc />
    public IDictionary<string, IScriptVars> CascadeScriptVars { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    #endregion
}