using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class CooldownComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var subject = vars.GetSubject<PanelEntityBase>();

        var aisling = context.SourceAisling;

        if (aisling == null)
            return;
 
        // Get the player's cooldown reduction percentage (assuming it comes in as an integer like 10 for 10%)
        var cooldownReductionPct = aisling?.StatSheet.EffectiveCooldownReductionPct ?? 0;
        
        if (cooldownReductionPct > 60)
            cooldownReductionPct = 60;

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
            var reducedCooldownFlatSecs = (subject.Cooldown.Value.TotalSeconds - flatCooldown);
            var reducedCooldownSecs = reducedCooldownFlatSecs * (1 - reductionPct);
            var amountReduced = subject.Cooldown.Value.TotalSeconds - reducedCooldownSecs;

            if (amountReduced >= 20)
                amountReduced = 20;
            
            if (amountReduced < 0)
                amountReduced = 0;
            
            var finalCooldown = TimeSpan.FromSeconds(subject.Cooldown.Value.TotalSeconds - amountReduced);

            if (finalCooldown.TotalSeconds < 0)
                finalCooldown = TimeSpan.FromSeconds(0);
            
            // Set the new reduced cooldown (in milliseconds) for the ability
            subject.SetTemporaryCooldown(finalCooldown);
        }
    }
}