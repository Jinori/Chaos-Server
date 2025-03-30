using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class AddAggroComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IAddAggroComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var aggroAmount = options.AggroAmount ?? 0;
        var aggroMultiplier = context.Source.StatSheet.GetEffectiveStat(options.AggroMultiplier ?? Stat.STR);

        foreach (var target in targets)
        {
            if (target is Aisling or Merchant)
                break;

            var monster = target as Monster;

            monster?.AggroList.AddOrUpdate(
                context.Source.Id,
                _ => aggroAmount * aggroMultiplier,
                (_, currentAggro) => currentAggro + aggroAmount * aggroMultiplier);
        }
    }

    public interface IAddAggroComponentOptions
    {
        int? AggroAmount { get; init; }
        Stat? AggroMultiplier { get; init; }
    }
}