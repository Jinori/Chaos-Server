using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

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
            {
                if (target.Trackers.TimedEvents.HasActiveEvent("revivedrecently", out var cdtime))
                {
                    target.SendOrangeBarMessage($"You revived recently. (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }

                if ((context.Source.Id == target.Id) && !target.IsAlive)
                {
                    
                    target.Trackers.TimedEvents.AddEvent("revivedrecently", TimeSpan.FromMinutes(30), true);
                    target.IsDead = false;
                    target.StatSheet.SetHealthPct(25);
                    target.StatSheet.SetManaPct(25);
                    target.Client.SendAttributes(StatUpdateType.Vitality);
                    target.SendActiveMessage("You have self revived.");
                    target.Refresh();
                    target.Display();
                    break;
                }
            }
            return;
        }

        foreach (var target in targets)
            if (!target.IsAlive)
            {
                if (target.Trackers.TimedEvents.HasActiveEvent("revivedrecently", out var cdtime))
                {
                    target.SendOrangeBarMessage($"You revived recently. (({cdtime.Remaining.ToReadableString()}))");
                    return;
                }
                
                var reviveScript = vars.GetSourceScript().As<SubjectiveScriptBase<Spell>>();
                
                switch (reviveScript?.Subject.Template.TemplateKey)
                {
                    case "beothaich":
                        target.Trackers.TimedEvents.AddEvent("revivedrecently", TimeSpan.FromMinutes(30), true);

                        break;
                    case "revive":
                        target.Trackers.TimedEvents.AddEvent("revivedrecently", TimeSpan.FromMinutes(20), true);
                        break;
                        
                    case "resurrect": 
                        target.Trackers.TimedEvents.AddEvent("revivedrecently", TimeSpan.FromMinutes(10), true);

                        break;
                }

                target.IsDead = false;
                target.StatSheet.SetHealthPct(50);
                target.StatSheet.SetManaPct(50);
                target.Client.SendAttributes(StatUpdateType.Vitality);
                target.SendActiveMessage("You have been revived.");
                target.Refresh();
                target.Display();
            }
    }

    public interface IReviveComponentOptions
    {
        public bool ReviveSelf { get; init; }
    }
}