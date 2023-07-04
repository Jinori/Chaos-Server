using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class ReviveComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IReviveComponentOptions>();
        var targets = vars.GetTargets<Aisling>();

        if (options.ReviveSelf)
        {
            foreach (var target in targets)
                if ((context.Source.Id == target.Id) && !target.IsAlive)
                {
                    target.IsDead = false;
                    target.StatSheet.SetHealthPct(25);
                    target.StatSheet.SetManaPct(25);
                    target.Client.SendAttributes(StatUpdateType.Vitality);
                    target.SendActiveMessage("You have self revived.");
                    target.Refresh();

                    break;
                }

            return;
        }

        foreach (var target in targets)
            if (!target.IsAlive)
            {
                target.IsDead = false;
                target.StatSheet.SetHealthPct(50);
                target.StatSheet.SetManaPct(50);
                target.Client.SendAttributes(StatUpdateType.Vitality);
                target.SendActiveMessage("You have been revived.");
                target.Refresh();
            }
    }

    public interface IReviveComponentOptions
    {
        public bool ReviveSelf { get; init; }
    }
}