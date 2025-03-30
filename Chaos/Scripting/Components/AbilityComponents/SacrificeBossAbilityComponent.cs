using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct SacrificeBossAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)

            // Check if the target is a player and update their enums
            if (target is Aisling aisling)
            {
                aisling.Trackers.Enums.Set(SacrificeTrial.FinishedFifth);
                aisling.SendOrangeBarMessage("You got new enum.");
            }
    }
}