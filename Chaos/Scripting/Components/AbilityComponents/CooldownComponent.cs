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
        
        if (context.SourceAisling != null && !context.SourceAisling.SpellBook.ContainsByTemplateKey(subject.Template.TemplateKey))
            return;
        
        // Get the player's cooldown reduction percentage (assuming it comes in as an integer like 10 for 10%)
        var cooldownReductionPct = context.SourceAisling?.StatSheet.EffectiveCooldownReductionPct ?? 0;

        var cooldownReduction = context.SourceAisling?.StatSheet.EffectiveCooldownReductionMs ?? 0;

        // Convert cooldownReductionPct to a percentage (divide by 100)
        var reductionPct = cooldownReductionPct / 100.0;

        var flatCooldown = cooldownReduction;

        // Ensure that PanelEntityBase and its cooldown value are valid
        if (subject.Cooldown.HasValue)
        {
            // Apply the cooldown reduction to the original cooldown value
            var reducedCooldownSecs = (subject.Cooldown.Value.Seconds - flatCooldown) * (1 - reductionPct);

            // Set the new reduced cooldown (in milliseconds) for the ability
            subject.SetTemporaryCooldown(TimeSpan.FromSeconds(reducedCooldownSecs));
        }
    }
}
