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
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts.Monk;

public class ThrowDamageScript : ConfigurableSkillScriptBase,
                                 GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                                 DamageAbilityComponent.IDamageComponentOptions,
                                 ThrowCreatureComponent.IDamageComponentOptions,
                                 ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    public int? EffectApplyChance { get; init; }
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public int? ThrowRange { get; init; }

    /// <inheritdoc />
    public ThrowDamageScript(Skill subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                         ?.Execute<DamageAbilityComponent>()
                                         .Execute<ThrowCreatureComponent>()
                                         .Execute<ApplyEffectAbilityComponent>();

    #region ScriptVars
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
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }

    /// <inheritdoc />
    public Stat? DamageStat { get; init; }

    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    public decimal? PctOfHealthMultiplier { get; init; }
    public decimal? PctOfHealth { get; init; }

    public bool? SurroundingTargets { get; init; }
    public decimal? DamageMultiplierPerTarget { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    #endregion
}