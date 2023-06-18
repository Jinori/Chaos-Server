using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class DurabilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IDurabilityComponentOptions>();
        var targets = vars.GetTargets<Aisling>();

        if (options.ShouldDamageItems is false)
            return;

        foreach (var target in targets)
        {
            foreach (var item in target.Equipment)
            {
                if (item.Slot is > 1 and < 14)
                {
                    if (item.CurrentDurability >= 1) 
                        item.CurrentDurability--;

                    var percent = (200 * item.CurrentDurability + 1) / (item.Template.MaxDurability * 2);

                    var warningLevel = percent switch
                    {
                        <= 5  => 5,
                        <= 10 => 10,
                        <= 30 => 30,
                        <= 50 => 50,
                        _     => 0
                    };

                    if ((warningLevel > 0) && (warningLevel != item.LastWarningLevel))
                    {
                        target.SendActiveMessage($"Your {item.DisplayName} is at {percent}% durability.");
                        item.LastWarningLevel = warningLevel;
                    }

                    if (item is { CurrentDurability: <= 0, Template.AccountBound: false })
                    {
                        target.SendActiveMessage($"Your {item.DisplayName} broke.");
                        target.Equipment.TryGetRemove(item.Slot, out _);
                    }
                    if (item is { CurrentDurability: <= 0, Template.AccountBound: true })
                    {
                        if (target.Equipment.TryGetRemove(item.Slot, out var removedItem))
                        {
                            if (target.CanCarry(removedItem))
                                target.Inventory.TryAddToNextSlot(removedItem);
                            else
                            {
                                target.Bank.Deposit(removedItem);
                                target.SendActiveMessage($"{item.DisplayName} was nearly broke and sent to your bank.");
                            }
                        }
                    }
                }
            }
        }
    }

    public interface IDurabilityComponentOptions
    {
        bool? ShouldDamageItems { get; init; }
    }
}