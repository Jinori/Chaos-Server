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
        // Roll a chance based on the target's effective magic resistance
        if (Randomizer.RollChance(100 - context.Target.StatSheet.EffectiveMagicResistance))
            return true;
        // Check if the source is a SubjectiveScriptBase<Spell>
        if (!(source is SubjectiveScriptBase<Spell> spellScript))
            return false;
        // Notify the source Aisling if it exists
        context.SourceAisling?.SendActiveMessage($"{spellScript.Subject.Template.Name} has missed.");
        // Animate the target
        context.Target.Animate(_animation, context.Target.Id);
        return false;
    }
}