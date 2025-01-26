using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class DevourScript : ConfigurableSkillScriptBase,
                            GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                            MissingHealthDamageAbilityComponent.IDamageComponentOptions,
                            CleaveHealComponent.ICleaveHealComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public IApplyHealScript ApplyHealScript { get; init; }

    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public decimal DmgHealthPct { get; init; }

    public Element? Element { get; init; }

    /// <inheritdoc />

    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    public decimal HealPercentMissingHealth { get; init; }
    public int? ManaCost { get; init; }
    public bool? MoreDmgLowTargetHp { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    public decimal? PctHpDamage { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    public decimal? PctMptoHpHeal { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }

    /// <inheritdoc />
    public DevourScript(Skill subject)
        : base(subject)
    {
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                         ?.Execute<MissingHealthDamageAbilityComponent>()
                                         .Execute<CleaveHealComponent>();
}