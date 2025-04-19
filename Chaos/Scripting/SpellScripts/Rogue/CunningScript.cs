using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Rogue;

public sealed class CunningScript : SpellScriptBase
{
    private readonly IEffectFactory _effectFactory;

    public CunningScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        _effectFactory = effectFactory;

    /// <summary>
    /// key   = current effect **name** on the caster (e.g. "Cunning 2")  
    /// value = (next effect **key**, next effect **name**, mana cost)
    /// </summary>
    private static readonly Dictionary<string, (string NextKey, string NextName, int ManaCost)> EFFECT_MAP = new()
    {
        ["Cunning 0"] = ("Cunning1", "Cunning 1",     4_000),
        ["Cunning 1"] = ("Cunning2", "Cunning 2",     8_000),
        ["Cunning 2"] = ("Cunning3", "Cunning 3",    16_000),
        ["Cunning 3"] = ("Cunning4", "Cunning 4",    32_000),
        ["Cunning 4"] = ("Cunning5", "Cunning 5",    64_000),
        ["Cunning 5"] = ("Cunning6", "Cunning 6",   128_000)
    };

    public override void OnUse(SpellContext context)
    {
        if (context.Source.Effects.Contains("Cunning 6"))
        {
            context.SourceAisling?.SendOrangeBarMessage("You are already in Cunning 6!");

            return;
        }
        
        (var nextKey, var nextName, var manaCost) = EFFECT_MAP["Cunning 0"];

        foreach (var kvp in EFFECT_MAP)
        {
            if (context.Source.Effects.Contains(kvp.Key))
            {
                (nextKey, nextName, manaCost) = kvp.Value;
                break;
            }
        }

        // ---------- 2. Check resources ----------
        if (context.Source.StatSheet.CurrentMp < manaCost)
        {
            context.SourceAisling?
                   .SendOrangeBarMessage($"You need {manaCost:n0} mana to cast {nextName}.");
            return;
        }

        context.Source.StatSheet.SubtractMp(manaCost);
        
        foreach (var name in EFFECT_MAP.Keys)
            context.Source.Effects.Terminate(name);

        var nextEffect = _effectFactory.Create(nextKey);
        context.Source.Effects.Apply(context.Source, nextEffect, this);
    }
}