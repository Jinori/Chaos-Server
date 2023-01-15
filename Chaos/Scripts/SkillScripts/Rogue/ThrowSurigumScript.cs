using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripts.SkillScripts.Rogue;

public class ThrowSurigumScript : DamageScript
{
    protected IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public ThrowSurigumScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        //Must be wearing a soori or dagger
        if (context.SourceAisling?.Equipment[EquipmentSlot.Weapon]?.Slot is not null)
        {
            if (context.SourceAisling?.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey.ContainsI("dagger") is false)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "In an attempt to throw your weapon, you have failed.");
                return;
            }
        }
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
    }
}