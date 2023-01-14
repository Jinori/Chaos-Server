using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Monk;

public class PoisonPunchScript : BasicSkillScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

    /// <inheritdoc />
    public DamageScript(Spell subject)
        : base(subject)
    {
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
    public override void OnUse(SkillContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageMultiplier { get; init; }
    #endregion
}