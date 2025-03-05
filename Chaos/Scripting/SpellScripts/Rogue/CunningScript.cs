using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Rogue;

public class CunningScript : SpellScriptBase
{
    private readonly IEffectFactory _effectFactory;

    /// <inheritdoc />
    public CunningScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) => _effectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var effectMap = new Dictionary<string, string>
        {
            { "Cunning1", "Cunning2" },
            { "Cunning2", "Cunning3" },
            { "Cunning3", "Cunning4" },
            { "Cunning4", "Cunning5" },
            { "Cunning5", "Cunning6" },
            { "Cunning6", "Cunning6" }
        };
        
        string nextEffectName = null;
        foreach (var effect in effectMap.Keys)
        {
            if (context.Source.Effects.Contains(effect))
            {
                nextEffectName = effectMap[effect];
                break;
            }
        }
        
        nextEffectName ??= "Cunning1";

        // Create and apply the effect
        var nextEffect = _effectFactory.Create(nextEffectName);
        context.Source.Effects.Apply(context.Source, nextEffect);
    }
}