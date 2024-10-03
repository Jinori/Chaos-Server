using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class CascadingSkillDamageScript : ConfigurableSkillScriptBase,
                                     SpellComponent<Creature>.ISpellComponentOptions,
                                     DamageAbilityComponent.IDamageComponentOptions,
                                     CascadingComponent<CascadingDamageTileScript>.ICascadingComponentOptions
{
    /// <inheritdoc />
    public CascadingSkillDamageScript(Skill subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SourceScript = this;
        ReactorTileFactory = reactorTileFactory;
        CascadeScriptVars ??= Subject.Template.ScriptVars;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context) =>
        new ComponentExecutor(context)
            .WithOptions(this)
            .ExecuteAndCheck<SpellComponent<Creature>>()
            ?
            .Execute<DamageAbilityComponent>()
            .Execute<CascadingComponent<CascadingDamageTileScript>>();

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

    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
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

    /// <inheritdoc />
    public IScript SourceScript { get; init; }

    public bool? SurroundingTargets { get; init; }
    public decimal? DamageMultiplierPerTarget { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }

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
    /// <inheritdoc />
    public bool IgnoreMagicResistance { get; init; }
    #endregion

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
}