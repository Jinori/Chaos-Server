using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class CooldownComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var subject = vars.GetSubject<PanelEntityBase>();

        if ((context.SourceAisling != null) && !context.SourceAisling.SpellBook.ContainsByTemplateKey(subject.Template.TemplateKey))
            return;

        var aisling = context.SourceAisling;

        if (aisling == null)
            return;

        // Get the player's cooldown reduction percentage (assuming it comes in as an integer like 10 for 10%)
        var cooldownReductionPct = aisling?.StatSheet.EffectiveCooldownReductionPct ?? 0;
        
        if (cooldownReductionPct > 80)
            cooldownReductionPct = 80;

        var cooldownReduction = aisling?.StatSheet.EffectiveCooldownReduction ?? 0;
        
        if (cooldownReduction > 3)
            cooldownReduction = 3;

        // Convert cooldownReductionPct to a percentage (divide by 100)
        var reductionPct = cooldownReductionPct / 100.0;

        var flatCooldown = cooldownReduction;

        // Ensure that PanelEntityBase and its cooldown value are valid
        if (subject.Cooldown.HasValue)
        {
            // Apply the cooldown reduction to the original cooldown value
            var reducedCooldownSecs = (subject.Cooldown.Value.TotalSeconds - flatCooldown) * (1 - reductionPct);

            if (reducedCooldownSecs < 0)
                reducedCooldownSecs = 0;

            // Set the new reduced cooldown (in milliseconds) for the ability
            subject.SetTemporaryCooldown(TimeSpan.FromSeconds(reducedCooldownSecs));
        }
    }
}