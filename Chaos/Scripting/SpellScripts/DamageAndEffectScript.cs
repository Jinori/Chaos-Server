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
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class DamageAndEffectScript : ConfigurableSpellScriptBase,
                                     SpellComponent<Creature>.ISpellComponentOptions,
                                     ApplyEffectAbilityComponent.IApplyEffectComponentOptions,
                                     DamageAbilityComponent.IDamageComponentOptions
{

    /// <inheritdoc />
    public DamageAndEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context) =>
        new ComponentExecutor(context).WithOptions(this)
                                      .ExecuteAndCheck<SpellComponent<Creature>>()
                                      ?.Execute<DamageAbilityComponent>()
                                      .Execute<ApplyEffectAbilityComponent>();

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool IgnoreMagicResistance { get; init; }
    public bool ExcludeSourcePoint { get; init; }
    public TargetFilter Filter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public byte? Sound { get; init; }
    public ushort? AnimationSpeed { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    public bool ShouldNotBreakHide { get; init; }
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }
    public int? EffectApplyChance { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public Element? Element { get; init; }
    public bool? MoreDmgLowTargetHp { get; init; }
    public decimal? PctHpDamage { get; init; }
    public IScript SourceScript { get; init; }
    public bool? SurroundingTargets { get; init; }
    public decimal? DamageMultiplierPerTarget { get; init; }
}