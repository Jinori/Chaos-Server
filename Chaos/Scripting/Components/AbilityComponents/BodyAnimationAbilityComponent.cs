using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct BodyAnimationAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IBodyAnimationComponentOptions>();
        var animationSpeed = options.AnimationSpeed ?? 25;
        var sourceScript = vars.GetSourceScript();

        if (options.ScaleBodyAnimationSpeedByAttackSpeed is true)
        {
            var modifier = context.Source.StatSheet.EffectiveAttackSpeedPct / 100m;

            if (modifier < 0)
                animationSpeed = (ushort)(animationSpeed * Math.Abs(modifier - 1));
            else
                animationSpeed = (ushort)(animationSpeed / (1 + modifier));
        }

        var aisling = context.SourceAisling;

        if ((aisling != null) && aisling.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var item))
            if (item.Template.Category == "2H")
                if (sourceScript is SubjectiveScriptBase<Skill> { Subject.Template.IsAssail: true })
                {
                    context.Source.AnimateBody(BodyAnimation.TwoHandAtk, animationSpeed);

                    return;
                }
        
        if ((aisling != null) && aisling.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var item2))
            if (item2.Template.Name.ContainsI("Nunchaku") && (options.BodyAnimation == BodyAnimation.Punch))
                if (sourceScript is SubjectiveScriptBase<Skill> { Subject.Template.IsAssail: true })
                {
                    context.Source.AnimateBody(BodyAnimation.Assail, animationSpeed);

                    return;
                }

        context.Source.AnimateBody(options.BodyAnimation, animationSpeed);
    }

    public interface IBodyAnimationComponentOptions
    {
        ushort? AnimationSpeed { get; init; }
        BodyAnimation BodyAnimation { get; init; }
        bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }
    }
}