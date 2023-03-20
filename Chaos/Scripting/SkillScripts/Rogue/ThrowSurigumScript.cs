using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class ThrowSurigumScript : DamageScript
{
    protected new IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public ThrowSurigumScript(Skill subject)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        if (context.SourceAisling?.Equipment[EquipmentSlot.Weapon]?.Slot == null) 
            return;

        if (context.SourceAisling?.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey.Contains("dagger") != true) 
            return;
        
        var targets = AbilityComponent.Activate<Creature>(context, this);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
    }
}