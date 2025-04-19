using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Warrior;

public sealed class FuryScript : SpellScriptBase
{
    private readonly IEffectFactory _effectFactory;

    public FuryScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        _effectFactory = effectFactory;
    
    private static readonly Dictionary<string, (string NextKey, string NextName, int HealthCost)> EFFECT_MAP = new()
    {
        ["Fury 0"] = ("Fury1", "Fury 1",     8_000),
        ["Fury 1"] = ("Fury2", "Fury 2",     16_000),
        ["Fury 2"] = ("Fury3", "Fury 3",    32_000),
        ["Fury 3"] = ("Fury4", "Fury 4",    64_000),
        ["Fury 4"] = ("Fury5", "Fury 5",    128_000),
        ["Fury 5"] = ("Fury6", "Fury 6",   256_000)
    };

    public override void OnUse(SpellContext context)
    {
        if (context.Source.Effects.Contains("Fury 6"))
        {
            context.SourceAisling?.SendOrangeBarMessage("You are already in Fury 6!");

            return;
        }
        
        (var nextKey, var nextName, var healthCost) = EFFECT_MAP["Fury 0"];

        foreach (var kvp in EFFECT_MAP)
        {
            if (context.Source.Effects.Contains(kvp.Key))
            {
                (nextKey, nextName, healthCost) = kvp.Value;
                break;
            }
        }

        // ---------- 2. Check resources ----------
        if (context.Source.StatSheet.CurrentHp < healthCost)
        {
            context.SourceAisling?
                   .SendOrangeBarMessage($"You need {healthCost:n0} health to cast {nextName}.");
            return;
        }

        context.Source.StatSheet.SubtractHp(healthCost);
        
        foreach (var name in EFFECT_MAP.Keys)
            context.Source.Effects.Terminate(name);

        var nextEffect = _effectFactory.Create(nextKey);
        context.Source.Effects.Apply(context.Source, nextEffect, this);
    }
}