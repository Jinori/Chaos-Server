using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Damage;

public class ManaBasedDamageScript : ConfigurableSpellScriptBase,
                                     SpellComponent<Creature>.ISpellComponentOptions,
                                     ManaBasedDamageAbilityComponent.IManaBasedDamageComponentOptions
{
    public List<string>? EffectKeysToBreak { get; set; }

    public int? HealthCost { get; init; }
    public decimal PctHealthCost { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }

    /// <inheritdoc />
    public ManaBasedDamageScript(Spell subject)
        : base(subject)
        => ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<SpellComponent<Creature>>()
                                         ?.Execute<ManaBasedDamageAbilityComponent>();

    #region ScriptVars
    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public bool IgnoreMagicResistance { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public decimal? BaseDamageMultiplier { get; init; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />
    public decimal? FinalMultiplier { get; init; }

    /// <inheritdoc />
    public decimal? PctOfMana { get; init; }

    /// <inheritdoc />
    public decimal? PctOfManaMultiplier { get; init; }
    #endregion
}