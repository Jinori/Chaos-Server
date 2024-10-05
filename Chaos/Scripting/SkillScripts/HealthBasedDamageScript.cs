using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class HealthBasedDamageScript : ConfigurableSkillScriptBase,
                                       GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                                       HealthBasedDamageComponent.IHealthBasedDamageComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }
    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public int? HealthCostPct { get; init; }
    /// <inheritdoc />
    public decimal? HealthMultiplier { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public bool SingleTarget { get; init; }
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public IScript SourceScript { get; init; }
    /// <inheritdoc />
    public bool TrueDamage { get; init; }

    /// <inheritdoc />
    public HealthBasedDamageScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SourceScript = this;
        PanelEntityBase = subject;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context) =>
        new ComponentExecutor(context)
            .WithOptions(this)
            .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
            ?
            .Execute<HealthBasedDamageComponent>();

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public PanelEntityBase PanelEntityBase { get; init; }
}