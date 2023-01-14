using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Monk;

public class PoisonPunchScript : BasicSkillScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }
    protected readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public PoisonPunchScript(Skill subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();

        DamageComponentOptions = new DamageComponent.DamageComponentOptions
        {
            ApplyDamageScript = ApplyDamageScript,
            SourceScript = this,
            BaseDamage = BaseDamage,
            DamageMultiplier = DamageMultiplier,
            DamageStat = DamageStat
        };
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
        var effect = EffectFactory.Create("Poison");
        foreach (var target in targets.TargetEntities) 
            target.Effects.Apply(context.Source, effect);
    }

    #region ScriptVars
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageMultiplier { get; init; }
    #endregion
}