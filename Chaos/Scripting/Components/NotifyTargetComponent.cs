using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class NotifyTargetComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<INotifyTargetComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        if (options.SourceScript is not SubjectiveScriptBase<Spell> spellScript)
            return;
        
        foreach (var target in targets)
        {
            if (target is Aisling aisling)
                aisling.SendOrangeBarMessage($"{context.Source.Name} cast {spellScript.Subject.Template.Name} spell on you.");
        }
    }
    
    public interface INotifyTargetComponentOptions
    {
        IScript SourceScript { get; init; }
    }
}