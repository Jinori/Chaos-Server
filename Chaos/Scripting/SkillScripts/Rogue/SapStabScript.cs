using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class SapStabScript : ConfigurableSkillScriptBase,
                             GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                             DamageAbilityComponent.IDamageComponentOptions,
                             SapNeedleComponent.ISapNeedleComponentOptions

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
    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public decimal? DamageMultiplierPerTarget { get; init; }

    /// <inheritdoc />
    public Stat? DamageStat { get; init; }

    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />

    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public bool IsPetSap { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public int? ManaReplenish { get; init; }

    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaReplenish { get; init; }

    public decimal? PctOfHealth { get; init; }

    public decimal? PctOfHealthMultiplier { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public bool ReplenishGroup { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }

    public bool? SurroundingTargets { get; init; }

    /// <inheritdoc />
    public SapStabScript(Skill subject)
        : base(subject)
        => ApplyDamageScript = ApplyAttackDamageScript.Create();

    public override void OnUse(ActivationContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                         ?.Execute<DamageAbilityComponent>()
                                         .Execute<SapNeedleComponent>();
}