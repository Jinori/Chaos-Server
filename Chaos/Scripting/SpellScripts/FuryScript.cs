using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class FuryScript : SpellScriptBase
{
    private readonly IEffectFactory EffectFactory;
    /// <inheritdoc />
    public FuryScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
         var effect1 = EffectFactory.Create("Fury1");
            var effect2 = EffectFactory.Create("Fury2");
            var effect3 = EffectFactory.Create("Fury3");
            var effect4 = EffectFactory.Create("Fury4");
            var effect5 = EffectFactory.Create("Fury5");
            var effect6 = EffectFactory.Create("Fury6");
            
            if (!context.Source.Effects.Contains("Fury1")
                && !context.Source.Effects.Contains("Fury2")
                && !context.Source.Effects.Contains("Fury3")
                && !context.Source.Effects.Contains("Fury4")
                && !context.Source.Effects.Contains("Fury5")
                && !context.Source.Effects.Contains("Fury6"))
            {
                context.Source.Effects.Apply(context.Source, effect1);
                return;
            }
            if (context.Source.Effects.Contains("Fury1"))
            {
                context.Source.Effects.Apply(context.Source, effect2);
                return;
            }
            
            if (context.Source.Effects.Contains("Fury2"))
            {
                context.Source.Effects.Apply(context.Source, effect3);
                return;
            }
            if (context.Source.Effects.Contains("Fury3"))
            {
                context.Source.Effects.Apply(context.Source, effect4);
                return;
            }
            if (context.Source.Effects.Contains("Fury4"))
            {
                context.Source.Effects.Apply(context.Source, effect5);
                return;
            }
            if (context.Source.Effects.Contains("Fury5"))
            {
                context.Source.Effects.Apply(context.Source, effect6);
            }

            if (context.Source.Effects.Contains("Fury6"))
            {
                context.Source.Effects.Apply(context.Source, effect6);
            }
    }
}