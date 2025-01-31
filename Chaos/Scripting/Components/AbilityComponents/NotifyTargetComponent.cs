using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class NotifyTargetComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<INotifyTargetComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        if (vars.GetSourceScript() is not SubjectiveScriptBase<Spell> spellScript)
            return;
        
        foreach (var target in targets)
        {
            if (target is Aisling aisling)
                aisling.SendOrangeBarMessage($"{context.Source.Name} cast {spellScript.Subject.Template.Name} on you.");
        }
    }
    
    public interface INotifyTargetComponentOptions
    {
    }
}