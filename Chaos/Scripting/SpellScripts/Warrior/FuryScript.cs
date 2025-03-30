using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Warrior;

public class FuryScript(Spell subject, IEffectFactory effectFactory) : SpellScriptBase(subject)
{
    public override void OnUse(SpellContext context)
    {
        var effectMap = new Dictionary<string, string>
        {
            {
                "Fury1", "Fury2"
            },
            {
                "Fury2", "Fury3"
            },
            {
                "Fury3", "Fury4"
            },
            {
                "Fury4", "Fury5"
            },
            {
                "Fury5", "Fury6"
            },
            {
                "Fury6", "Fury6"
            }
        };

        string nextEffectName = null;

        foreach (var effect in effectMap.Keys)
            if (context.Source.Effects.Contains(effect))
            {
                nextEffectName = effectMap[effect];

                break;
            }

        // If no effects are found, start with Fury1
        nextEffectName ??= "Fury1";

        // Create and apply the effect
        var nextEffect = effectFactory.Create(nextEffectName);
        context.Source.Effects.Apply(context.Source, nextEffect);
    }
}