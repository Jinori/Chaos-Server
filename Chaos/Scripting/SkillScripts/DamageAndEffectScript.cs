using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class DamageAndEffectScript : ConfigurableSkillScriptBase,
                                     AbilityComponent<Creature>.IAbilityComponentOptions,
                                     DamageComponent.IDamageComponentOptions,
                                     ApplyEffectComponent.IApplyEffectComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }
    /// <inheritdoc />
    public int? BaseDamage { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public Stat? DamageStat { get; init; }
    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }
    /// <inheritdoc />
    public IEffectFactory EffectFactory { get; init; }
    /// <inheritdoc />
    public string? EffectKey { get; init; }
    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public IScript SourceScript { get; init; }

    /// <inheritdoc />
    public DamageAndEffectScript(Skill subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context) =>
        new ComponentExecutor(context).WithOptions(this)
                                      .ExecuteAndCheck<AbilityComponent<Creature>>()
                                      ?.Execute<DamageComponent>()
                                      .Execute<ApplyEffectComponent>();
}