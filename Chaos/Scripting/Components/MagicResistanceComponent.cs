using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.Components;

public class MagicResistanceComponent
{
    private readonly Animation _animation = new()
    {
        TargetAnimation = 33,
        AnimationSpeed = 100
    };
    
    public bool TryCastSpell(ActivationContext context, IScript source)
    {
        if (Randomizer.RollChance(100 - context.Target.StatSheet.EffectiveMagicResistance)) 
            return true;

        if (source is not SubjectiveScriptBase<Spell> spellScript)
            return false;
        
        context.SourceAisling?.SendActiveMessage($"{spellScript.Subject.Template.Name} has missed.");
        context.Target.Animate(_animation, context.Target.Id);
        return false;
    }
}