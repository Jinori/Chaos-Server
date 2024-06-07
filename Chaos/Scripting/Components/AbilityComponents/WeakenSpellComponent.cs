using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class WeakenSpellComponent : IComponent
{
    private Animation Animation { get; } = new()
    {
        AnimationSpeed = 75,
        TargetAnimation = 2
    };
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Aisling>().ToList();

        foreach (var target in targets.ToList())
        {
            if (target.Trackers.Enums.HasValue(GodMode.Yes))
                return;
            
            if (target.Inventory.Contains("Silver Wolf Leather"))
                return;

            target.StatSheet.SetHp(1);
            target.Client.SendAttributes(StatUpdateType.Vitality);
            target.SendOrangeBarMessage("Mantis weakens your body.");
            target.Animate(Animation, target.Id);
        }
    }

    public interface IWeakenSpellComponentOptions
    {
    }
}