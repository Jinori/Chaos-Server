using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Extensions.Geometry;
using Chaos.Scripting.MonsterScripts.Boss;

namespace Chaos.Scripting.Components.AbilityComponents;

public class PullAggroComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IAddAggroComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var aggroAmount = options.AggroAmount ?? 0;
        var aggroMultiplier = options.AggroMultiplier ?? Stat.STR;

        foreach (var target in targets)
        {
            if (target is Aisling or Merchant)
            {
                context.SourceAisling?.SendOrangeBarMessage("That target cannot be grasped.");
                break;
            }

            var monster = target as Monster;

            // Add or update aggro
            monster?.AggroList.AddOrUpdate(
                context.Source.Id,
                _ => aggroAmount * (int)aggroMultiplier,
                (_, currentAggro) => currentAggro + aggroAmount * (int)aggroMultiplier);

            // Pull the target to the user
            PullTargetToUser(target, context.Source);
        }
    }

    private void PullTargetToUser(Creature target, Creature source)
    {
        if (source.MapInstance == target.MapInstance)
        {
            var direction = source.Direction;
            var pointNextToSource = source.GenerateCardinalPoints()
                .WithConsistentDirectionBias(direction).ToList();

            if (target.Name.Contains("Dummy") || target.Script.Is<ThisIsABossScript>())
            {
                var player = source as Aisling;
                
                player?.SendOrangeBarMessage("That target cannot be grasped.");
                return;
            }

            if (target.WithinRange(source, 1))
                return;

            foreach (var point in pointNextToSource)
            {
                if (source.MapInstance.IsWalkable(point, CreatureType.Normal, false))
                {
                    target.WarpTo(point);
                    return;
                }
            }
        }
    }

    public interface IAddAggroComponentOptions
    {
        int? AggroAmount { get; init; }
        Stat? AggroMultiplier { get; init; }
    }
}