using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct TransferBloodAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IHealthTransferComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        foreach (var target in targets)
        {
            // Calculate 20% of the user's health
            var healthToTransfer = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.CurrentHp, 20);

            // Ensure health to transfer is not greater than current health
            healthToTransfer = Math.Min(healthToTransfer, (int)context.Source.StatSheet.CurrentHp);

            // Reduce user's health by the calculated amount
            context.Source.StatSheet.SubtractHp(healthToTransfer);
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);

            // Check if the target is resisting the heal
            if (target.Effects.Contains("prevent heal"))
            {
                if (target.Name != context.SourceAisling?.Name)
                {
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name} is currently resisting health transfer.");
                    continue;
                }
                context.SourceAisling?.SendOrangeBarMessage($"You are currently resisting health transfer.");
                continue;
            }

            // Heal the target with the transferred health
            options.ApplyHealScript.ApplyHeal(
                context.Source,
                target,
                options.SourceScript,
                healthToTransfer);
        }
    }

    public interface IHealthTransferComponentOptions
    {
        IApplyHealScript ApplyHealScript { get; init; }
        IScript SourceScript { get; init; }
    }
}
