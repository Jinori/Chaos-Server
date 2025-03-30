using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class PeekScript : ConfigurableSkillScriptBase,
                          GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                          PeekAbilityComponent.IShowPeekOptions
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
    public List<string>? EffectKeysToBreak { get; set; }

    /// <inheritdoc />
    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    public int? HealthCost { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    public decimal PctHealthCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    public bool StopOnWalls { get; init; }

    /// <inheritdoc />
    public PeekScript(Skill subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnUse(ActivationContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                         ?.Execute<PeekAbilityComponent>();

    #region ScriptVars
    public string? DialogKey { get; init; }

    /// <inheritdoc />
    public IMerchantFactory MerchantFactory { get; init; }

    public IDialogFactory DialogFactory { get; init; }
    #endregion
}