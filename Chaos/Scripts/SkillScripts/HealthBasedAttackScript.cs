using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class HealthBasedAttackScript : BasicSkillScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public HealthBasedAttackScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        if (TrueDamage is true)
            ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
        var damageAmount = Convert.ToInt32(HealthMultiplier * context.Source.StatSheet.CurrentHp);

        foreach (var target in targets.TargetEntities)
        {
            ApplyDamageScript.ApplyDamage(context.Source, target, this, damageAmount);
        }
        
        if (context.Source.StatSheet.HealthPercent <= HealthCostPct) 
            context.Source.StatSheet.SetHp(1);
        else
        {
            context.Source.StatSheet.SubtractHealthPct(HealthCostPct);
        }

        context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
    }
    
    #region ScriptVars
    protected decimal? HealthMultiplier { get; init; }
    protected int HealthCostPct { get; init; }
    protected bool TrueDamage { get; init; }
    #endregion ScriptVars
}