using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class Weaken2SpellComponent : IComponent
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
            if (target.IsGodModeEnabled())
                return;

            var halfhealth = target.StatSheet.CurrentHp / 2;

            target.StatSheet.SetHp(halfhealth);
            target.Client.SendAttributes(StatUpdateType.Vitality);
            target.SendOrangeBarMessage("Mantis weakens your body.");
            target.Animate(Animation, target.Id);
        }
    }

    public interface IWeakenSpellComponentOptions
    {
    }
}