using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct FasSpioradAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();
        
        
        foreach (var target in targets)
        {
            var healthSacrificed = target.StatSheet.CurrentHp * .60;
            var manaToReplenish = healthSacrificed * 1.10;

            if (target.StatSheet.CurrentHp >= 1)
            {
                target.StatSheet.SubtractHp((int)healthSacrificed);
                target.StatSheet.AddMp((int)manaToReplenish);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
                context.SourceAisling?.SendOrangeBarMessage($"You replenished {(int)manaToReplenish} mana using Fas Spiorad!");
            }
            else
            {
                context.SourceAisling?.SendOrangeBarMessage("You do not have the health to convert to mana.");
            }
        }
    }
}