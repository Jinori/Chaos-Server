using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class CooldownComponent : IComponent
{
    // Dictionary to store the original cooldown values of each entity
    private static readonly Dictionary<PanelEntityBase, TimeSpan> OriginalCooldowns = new();

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ICooldownComponentOptions>();
        
        if (context.SourceAisling != null && !context.SourceAisling.SpellBook.ContainsByTemplateKey(options.PanelEntityBase.Template.TemplateKey))
            return;
        
        // Get the player's cooldown reduction percentage (assuming it comes in as an integer like 10 for 10%)
        var cooldownReductionPct = context.SourceAisling?.StatSheet.EffectiveCooldownReductionPct ?? 0;

        var cooldownReduction = context.SourceAisling?.StatSheet.EffectiveCooldownReductionMs ?? 0;

        // Convert cooldownReductionPct to a percentage (divide by 100)
        var reductionPct = cooldownReductionPct / 100.0;

        var flatCooldown = cooldownReduction;

        // Ensure that PanelEntityBase and its cooldown value are valid
        if (options?.PanelEntityBase != null && options.PanelEntityBase.Cooldown.HasValue)
        {
            // Check if the original cooldown is already stored
            if (!OriginalCooldowns.TryGetValue(options.PanelEntityBase, out var originalCooldown))
            {
                // If not, store the current cooldown as the original value
                originalCooldown = options.PanelEntityBase.Cooldown.Value;
                OriginalCooldowns[options.PanelEntityBase] = originalCooldown;
            }

            // Apply the cooldown reduction to the original cooldown value
            var reducedCooldownMs = originalCooldown.TotalMilliseconds * (1 - reductionPct) - flatCooldown;

            // Set the new reduced cooldown (in milliseconds) for the ability
            options.PanelEntityBase.Cooldown = TimeSpan.FromMilliseconds(reducedCooldownMs);

            // Reset the elapsed time for the cooldown
            options.PanelEntityBase.Elapsed = TimeSpan.Zero; // Assuming you want to reset the elapsed timer after applying the cooldown
        }
    }

    public interface ICooldownComponentOptions
    {
        PanelEntityBase PanelEntityBase { get; init; }
    }
}
