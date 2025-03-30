using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class CascadingSkillDamageScript : ConfigurableSkillScriptBase,
                                          GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                                          DamageAbilityComponent.IDamageComponentOptions,
                                          CascadingComponent<CascadingDamageTileScript>.ICascadingComponentOptions
{
    public List<string>? EffectKeysToBreak { get; set; }

    public int? HealthCost { get; init; }
    public decimal PctHealthCost { get; init; }

    /// <inheritdoc />
    public CascadingSkillDamageScript(Skill subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ReactorTileFactory = reactorTileFactory;
        CascadeScriptVars ??= Subject.Template.ScriptVars;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
                                         ?.Execute<DamageAbilityComponent>()
                                         .Execute<CascadingComponent<CascadingDamageTileScript>>();

    #region ScriptVars
    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    public TargetFilter Filter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public byte? Sound { get; init; }
    public ushort? AnimationSpeed { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public int? BaseDamage { get; init; }
    public decimal? DamageMultiplierPerTarget { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public Element? Element { get; init; }
    public bool? MoreDmgLowTargetHp { get; init; }
    public decimal? PctHpDamage { get; init; }
    public decimal? PctOfHealth { get; init; }
    public decimal? PctOfHealthMultiplier { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }
    public bool CascadeOnlyFromEntities { get; init; }
    public IDictionary<string, IScriptVars> CascadeScriptVars { get; init; }
    public IReactorTileFactory ReactorTileFactory { get; init; }
    public bool? SurroundingTargets { get; init; }
    #endregion
}